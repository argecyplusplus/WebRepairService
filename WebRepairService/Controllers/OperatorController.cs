using iText.IO.Font; 
using iText.IO.Image;
using iText.Kernel.Font; 
using iText.Kernel.Pdf; 
using iText.Layout; 
using iText.Layout.Element; 
using iText.Layout.Properties; 
using iText.Kernel.Colors; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebRepairService.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;
using iText.Kernel.Pdf.Canvas.Draw;

namespace WebRepairService.Controllers
{
    [Authorize(Roles = "Operator,Admin")]
    public class OperatorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<OperatorController> _logger;
        private const string FontPath = "wwwroot/fonts/arial.ttf";

        public OperatorController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment hostingEnvironment,
            ILogger<OperatorController> logger)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        private async Task<List<SelectListItem>> GetDeviceTypes()
        {
            return await _context.DeviceTypes
                .OrderBy(d => d.Name.Trim().ToLower() == "другое" ? 1 : 0)
                .ThenBy(d => d.Name)
                .Select(d => new SelectListItem
                {
                    Value = d.DeviceTypeId.ToString(),
                    Text = d.Name
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> GetServiceTypes()
        {
            var serviceTypes = await _context.ServiceTypes
                .Select(s => new { s.ServiceTypeId, s.Name, s.MinimalPrice })
                .ToListAsync();

            var orderedServiceTypes = serviceTypes
                .OrderBy(s => s.Name.Trim().ToLower() == "другое" ? 1 : 0)
                .ThenBy(s => s.Name)
                .Select(s => new SelectListItem
                {
                    Value = s.ServiceTypeId.ToString(),
                    Text = s.MinimalPrice > 0 ? $"{s.Name} (от {s.MinimalPrice} руб.)" : s.Name
                })
                .ToList();

            return orderedServiceTypes;
        }

        private async Task<List<SelectListItem>> GetStatuses()
        {
            return await _context.Statuses
                .Select(s => new SelectListItem
                {
                    Value = s.StatusId.ToString(),
                    Text = s.Name
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> GetEngineers()
        {
            var engineers = await _userManager.GetUsersInRoleAsync("Engineer");
            return engineers
                .OrderBy(e => e.FullName)
                .Select(e => new SelectListItem
            {
                Value = e.Id,
                Text = e.FullName ?? e.UserName
            }).ToList();
        }




        // GET: Operator/Orders - Список всех заказов
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.Status)
                .Include(o => o.DeviceType)
                .Include(o => o.ServiceType)
                .Include(o => o.Operator)
                .Include(o => o.Engineer)
                .Include(o => o.Photos)
                .OrderByDescending(o => o.CreationDate)
                .ToListAsync();

            return View(orders);
        }


        // GET: Operator/Details/ - Просмотр полной информации о заказе
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Status)
                .Include(o => o.DeviceType)
                .Include(o => o.ServiceType)
                .Include(o => o.Operator)
                .Include(o => o.Engineer)
                .Include(o => o.Photos)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Operator/Create - Форма создания заказа
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new OrderViewModel
            {
                DeviceTypes = await GetDeviceTypes(),
                ServiceTypes = await GetServiceTypes(),
                Statuses = await GetStatuses()
            };

            return View(model);
        }

        // POST: Operator/Create - Создание заказа
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel model)
        {
            // Получаем минимальную цену для выбранного типа услуги
            var serviceType = await _context.ServiceTypes.FindAsync(model.ServiceTypeId);
            if (serviceType != null && model.Price < serviceType.MinimalPrice)
            {
                ModelState.AddModelError("Price", $"Цена не может быть меньше минимальной ({serviceType.MinimalPrice} руб.)");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage))
                    .ToList();

                _logger.LogWarning("Ошибки валидации: {Errors}", string.Join(", ", errors));

                model.DeviceTypes = await GetDeviceTypes();
                model.ServiceTypes = await GetServiceTypes();
                model.Statuses = await GetStatuses();

                return View(model);
            }

            try
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var order = new Order
                {
                    ClientFullName = model.ClientFullName,
                    ClientPhone = model.ClientPhone,
                    ClientEmail = model.ClientEmail,
                    Model = model.DeviceModel,
                    Details = model.Details,
                    Price = model.Price,
                    DeviceTypeId = model.DeviceTypeId,
                    ServiceTypeId = model.ServiceTypeId,
                    StatusId = 1, //Устанавливаем при создании всегда первый статус
                    OperatorId = currentUser.Id,
                    CreationDate = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Обработка фото
                if (model.Photos != null && model.Photos.Count > 0)
                {
                    await ProcessPhotos(order.OrderId, model.Photos);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании заказа");
                ModelState.AddModelError("", "Произошла ошибка при создании заказа");

                model.DeviceTypes = await GetDeviceTypes();
                model.ServiceTypes = await GetServiceTypes();
                model.Statuses = await GetStatuses();

                return View(model);
            }
        }

        private async Task ProcessPhotos(int orderId, List<IFormFile> photos)
        {
            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "orders", orderId.ToString());
            Directory.CreateDirectory(uploadsFolder);

            foreach (var photo in photos)
            {
                if (photo.Length == 0) continue;

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                _context.Photos.Add(new Photo
                {
                    OrderId = orderId,
                    Link = $"/uploads/orders/{orderId}/{uniqueFileName}"
                });
            }

            await _context.SaveChangesAsync();
        }

        // GET: Operator/Edit/5 - Форма редактирования
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.Photos)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            var model = new OrderEditDto
            {
                OrderId = order.OrderId,
                ClientFullName = order.ClientFullName,
                ClientPhone = order.ClientPhone,
                ClientEmail = order.ClientEmail,
                DeviceModel = order.Model,
                Details = order.Details,
                Price = (int)(decimal)order.Price,
                DeviceTypeId = order.DeviceTypeId,
                ServiceTypeId = order.ServiceTypeId,
                StatusId = order.StatusId,
                DeviceTypes = await GetDeviceTypes(),
                ServiceTypes = await GetServiceTypes(),
                Statuses = await GetStatuses(),
                Photos = order.Photos.Select(p => new PhotoViewModel
                {
                    PhotoId = p.PhotoId,
                    Link = p.Link,
                    OrderId = p.OrderId
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Operator,Admin")]
        public async Task<IActionResult> Edit(int id, OrderEditDto model)
        {
            var serviceType = await _context.ServiceTypes.FindAsync(model.ServiceTypeId);
            if (serviceType != null && model.Price < serviceType.MinimalPrice)
            {
                ModelState.AddModelError("Price", $"Цена не может быть меньше минимальной ({serviceType.MinimalPrice} руб.)");
            }

            model.DeviceTypes = await GetDeviceTypes();
            model.ServiceTypes = await GetServiceTypes();
            model.Statuses = await GetStatuses();

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage))
                    .ToList();

                _logger.LogWarning("Ошибки валидации: {Errors}", string.Join(", ", errors));
                return View(model);
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            try
            {
                // Сохраняем текущий статус перед обновлением
                var currentStatusId = order.StatusId;

                order.ClientFullName = model.ClientFullName;
                order.ClientPhone = model.ClientPhone;
                order.ClientEmail = model.ClientEmail;
                order.Model = model.DeviceModel;
                order.Details = model.Details;
                order.Price = model.Price;
                order.DeviceTypeId = model.DeviceTypeId;
                order.ServiceTypeId = model.ServiceTypeId;

                // Только администратор может изменить статус
                if (User.IsInRole("Admin"))
                {
                    order.StatusId = model.StatusId;
                }
                else
                {
                    order.StatusId = currentStatusId; // Сохраняем оригинальный статус
                }

                _context.Update(order);

                if (model.NewPhotos != null && model.NewPhotos.Count > 0)
                {
                    await ProcessPhotos(order.OrderId, model.NewPhotos);
                }

                if (!string.IsNullOrEmpty(model.DeletedPhotos))
                {
                    var photoIds = model.DeletedPhotos.Split(',')
                                      .Select(int.Parse)
                                      .ToList();

                    var photosToDelete = await _context.Photos
                        .Where(p => photoIds.Contains(p.PhotoId))
                        .ToListAsync();

                    foreach (var photo in photosToDelete)
                    {
                        DeletePhotoFile(photo.Link);
                        _context.Photos.Remove(photo);
                    }
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при редактировании заказа");
                ModelState.AddModelError("", "Ошибка при сохранении");
                return View(model);
            }
        }




        // GET: Operator/Delete/5 - Подтверждение удаления
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Status)
                .Include(o => o.DeviceType)
                .Include(o => o.ServiceType)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Operator/Delete/5 - Удаление заказа
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Photos)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order != null)
            {
                foreach (var photo in order.Photos)
                {
                    DeletePhotoFile(photo.Link);
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Operator/UploadPhoto - Загрузка фото
        [HttpPost]
        public async Task<IActionResult> UploadPhoto(int orderId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран");
            }

            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("Заказ не найден");
            }

            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "orders", orderId.ToString());
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var photo = new Photo
            {
                OrderId = orderId,
                Link = $"/uploads/orders/{orderId}/{uniqueFileName}"
            };

            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();

            return Ok(new { link = photo.Link, photoId = photo.PhotoId });
        }

        // POST: Operator/DeletePhoto - Удаление фото
        [HttpPost]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            var photo = await _context.Photos.FindAsync(photoId);
            if (photo == null)
            {
                return NotFound();
            }

            DeletePhotoFile(photo.Link);
            _context.Photos.Remove(photo);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Загрузка данных для выпадающих списков
        private async Task LoadDropdowns()
        {
            ViewBag.DeviceTypes = (await _context.DeviceTypes.ToListAsync())
                .Select(d => new SelectListItem
                {
                    Value = d.DeviceTypeId.ToString(),
                    Text = d.Name
                }).ToList();

            ViewBag.ServiceTypes = (await _context.ServiceTypes.ToListAsync())
                .Select(s => new SelectListItem
                {
                    Value = s.ServiceTypeId.ToString(),
                    Text = s.Name
                }).ToList();

            ViewBag.Statuses = (await _context.Statuses.ToListAsync())
                .Select(s => new SelectListItem
                {
                    Value = s.StatusId.ToString(),
                    Text = s.Name
                }).ToList();

            ViewBag.Engineers = (await _userManager.GetUsersInRoleAsync("Engineer"))
                .Select(e => new SelectListItem
                {
                    Value = e.Id,
                    Text = e.FullName ?? e.UserName
                }).ToList();
        }

        // Проверка существования заказа
        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }

        // Удаление файла фото
        private void DeletePhotoFile(string link)
        {
            if (string.IsNullOrEmpty(link)) return;

            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, link.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        // генерация пдф-документа
        [HttpGet]
        public async Task<IActionResult> GeneratePdf(int id)
        {
            var order = await _context.Orders
                .Include(o => o.DeviceType)
                .Include(o => o.ServiceType)
                .Include(o => o.Operator)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                TempData["Error"] = "Заказ не найден";
                return RedirectToAction(nameof(Index));
            }

            // Проверка шрифта
            var fontPath = Path.Combine(_hostingEnvironment.WebRootPath, "fonts", "arial.ttf");
            if (!System.IO.File.Exists(fontPath))
            {
                TempData["Error"] = "Файл шрифта не найден";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Создаем PDF в памяти
            byte[] pdfBytes;
            using (var memoryStream = new MemoryStream())
            {
                PdfWriter writer = null;
                PdfDocument pdf = null;

                try
                {
                    writer = new PdfWriter(memoryStream);
                    pdf = new PdfDocument(writer);
                    var document = new Document(pdf);

                    // Настройка шрифта
                    var font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
                    document.SetFont(font);

                    // Шапка документа
                    document.Add(new Paragraph("СЕРВИСНЫЙ ЦЕНТР")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(14)
                        .SetBold());

                    document.Add(new Paragraph("КВИТАНЦИЯ О ПРИЕМЕ В РЕМОНТ")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(16)
                        .SetBold()
                        .SetMarginBottom(5));

                    document.Add(new Paragraph($"№ {order.OrderId} от {order.CreationDate:dd.MM.yyyy}")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(12)
                        .SetMarginBottom(15));

                    // Профессиональная горизонтальная линия
                    document.Add(new LineSeparator(new SolidLine(1f))
                        .SetMarginBottom(15)
                        .SetMarginTop(15));

                    // Основная информация
                    var table = new Table(2).UseAllAvailableWidth();
                    var headerStyle = new Style()
                        .SetBackgroundColor(WebColors.GetRGBColor("#F5F5F5"))
                        .SetBold()
                        .SetPadding(5);

                    void AddTableRow(string header, string value)
                    {
                        table.AddCell(new Cell().Add(new Paragraph(header)).AddStyle(headerStyle));
                        table.AddCell(new Cell().Add(new Paragraph(value ?? "не указано")));
                    }

                    AddTableRow("Дата приема", order.CreationDate.ToString("dd.MM.yyyy HH:mm"));
                    AddTableRow("ФИО клиента", order.ClientFullName);
                    AddTableRow("Контактный телефон", order.ClientPhone);
                    AddTableRow("Устройство", $"{order.DeviceType?.Name} {order.Model}");
                    AddTableRow("Тип услуги", order.ServiceType?.Name);
                    AddTableRow("Предварительная стоимость", $"{order.Price} руб.");

                    document.Add(table);

                    // Описание неисправности
                    document.Add(new Paragraph("Описание неисправности:")
                        .SetBold()
                        .SetMarginTop(10));
                    document.Add(new Paragraph(order.Details ?? "Не указана")
                        .SetMarginBottom(15));

                    // Условия ремонта
                    document.Add(new Paragraph("УСЛОВИЯ РЕМОНТА:")
                        .SetBold()
                        .SetMarginBottom(5));

                    var conditions = new List<string>
            {
                "1. Сервисный центр не несет ответственности за возможную потерю данных в памяти устройства.",
                "2. Сроки ремонта являются ориентировочными и могут быть изменены.",
                "3. Ориентировочная стоимость ремонта может быть изменена после диагностики.",
                "4. Устройство хранится в сервисном центре не более 30 дней после уведомления о готовности.",
                "5. Претензии по внешнему виду устройства принимаются только при его получении."
            };

                    foreach (var condition in conditions)
                    {
                        document.Add(new Paragraph(condition)
                            .SetFontSize(9)
                            .SetMarginBottom(3));
                    }

                    // Профессиональная горизонтальная линия
                    document.Add(new LineSeparator(new SolidLine(1f))
                        .SetMarginBottom(15)
                        .SetMarginTop(15));

                    // Информация о приеме
                    document.Add(new Paragraph($"Принял: {order.Operator?.FullName ?? "не указан"}")
                        .SetMarginBottom(10));

                    // Подписи
                    document.Add(new Paragraph("Подписи сторон:")
                        .SetBold()
                        .SetMarginTop(15));

                    document.Add(new Paragraph("Клиент: ___________________ (____________)")
                        .SetMarginTop(10)
                        .SetMarginBottom(5));

                    document.Add(new Paragraph("Исполнитель: ___________________ (____________)")
                        .SetMarginBottom(15));

                    document.Add(new Paragraph($"Дата формирования документа: {DateTime.Now:dd.MM.yyyy}")
                        .SetFontSize(9)
                        .SetTextAlignment(TextAlignment.RIGHT));

                    document.Close();

                    pdfBytes = memoryStream.ToArray();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при создании PDF");
                    TempData["Error"] = "Ошибка при генерации PDF документа";
                    return RedirectToAction(nameof(Details), new { id });
                }
                finally
                {
                    pdf?.Close();
                    writer?.Close();
                    writer?.Dispose();
                }
            }

            return File(pdfBytes, "application/pdf", $"Квитанция_{order.OrderId}.pdf");
        }

        // Вспомогательный метод для добавления строк в таблицу
        private void AddTableRow(Table table, string header, string value, Style headerStyle)
        {
            table.AddCell(new Cell()
                .Add(new Paragraph(header))
                .AddStyle(headerStyle));

            table.AddCell(new Cell()
                .Add(new Paragraph(value ?? "не указано")));
        }
    }
}
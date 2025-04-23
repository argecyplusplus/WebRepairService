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

namespace WebRepairService.Controllers
{
    [Authorize(Roles = "Operator")]
    public class OperatorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public OperatorController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
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

        // GET: Operator/Create - Форма создания заказа
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }

        // POST: Operator/Create - Создание заказа
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                order.OperatorId = currentUser.Id;
                order.CreationDate = DateTime.UtcNow;
                order.StatusId = 1; // Статус "Новый"

                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdowns();
            return View(order);
        }

        // GET: Operator/Edit/5 - Форма редактирования
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Photos)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            await LoadDropdowns();
            return View(order);
        }

        // POST: Operator/Edit/5 - Сохранение изменений
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdowns();
            return View(order);
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
                // Удаляем связанные фото
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

            // Создаем папку для заказов, если ее нет
            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "orders", orderId.ToString());
            Directory.CreateDirectory(uploadsFolder);

            // Генерируем уникальное имя файла
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Сохраняем файл
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Сохраняем ссылку в БД
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
    }
}
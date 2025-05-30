﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebRepairService.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebRepairService.Controllers
{
    [Authorize(Roles = "Engineer")]
    public class EngineerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EngineerController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: Engineer/AcceptOrder/5 - Принять заказ текущим инженером
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptOrder(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // Проверяем, что заказ имеет статус "Принят оператором" и не назначен инженер
            if (order.StatusId != 1 || order.EngineerId != null)
            {
                TempData["Error"] = "Невозможно принять этот заказ";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Назначаем текущего инженера и меняем статус
                order.EngineerId = currentUserId;
                order.StatusId = 2; // Статус "Принят инженером"

                _context.Update(order);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Заказ успешно принят";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ошибка при принятии заказа";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Engineer/Index - Список заказов со статусом "Принят оператором" и без инженера
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Where(o => o.StatusId == 1 && o.EngineerId == null) // Только статус 1 и без инженера
                .Include(o => o.Status)
                .Include(o => o.DeviceType)
                .Include(o => o.ServiceType)
                .Include(o => o.Operator)
                .Include(o => o.Engineer)
                .Include(o => o.Photos)
                .OrderByDescending(o => o.CreationDate)
                .ToListAsync();

            ViewBag.Statuses = await _context.Statuses.ToListAsync();

            return View(orders);
        }

        // GET: Engineer/MyOrders - Список заказов текущего инженера
        [Authorize(Roles = "Engineer")]
        public async Task<IActionResult> MyOrders()
        {
            var currentUserId = _userManager.GetUserId(User);

            var orders = await _context.Orders
                .Where(o => o.EngineerId == currentUserId)
                .Include(o => o.Status)
                .Include(o => o.DeviceType)
                .Include(o => o.ServiceType)
                .Include(o => o.Operator)
                .Include(o => o.Photos)
                .OrderByDescending(o => o.CreationDate)
                .ToListAsync();

            ViewBag.Statuses = await _context.Statuses.ToListAsync();
            ViewBag.CurrentUserId = currentUserId;

            return View(orders);
        }

        // GET: Engineer/Details/5 - Просмотр полной информации о заказе
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.CurrentUserId = _userManager.GetUserId(User);

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

        // GET: Engineer/Edit/5 - Форма редактирования (только статус и инженер)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var model = new EngineerOrderEditDto
            {
                OrderId = order.OrderId,
                StatusId = order.StatusId,
                EngineerId = order.EngineerId,
                CurrentStatus = order.Status?.Name,
                CurrentEngineer = order.Engineer?.FullName,
                Statuses = await GetStatuses(),
                Engineers = await GetEngineers()
            };

            return View(model);
        }

        // POST: Engineer/Edit/5 - Сохранение изменений
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EngineerOrderEditDto model)
        {
            if (id != model.OrderId)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                order.StatusId = model.StatusId;
                order.EngineerId = model.EngineerId;
                if (order.StatusId == 4)
                {
                    order.CompletionDate = DateTime.UtcNow;
                }

                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            model.Statuses = await GetStatuses();
            model.Engineers = await GetEngineers();
            return View(model);
        }

        // POST: Engineer/ChangeStatus/5 - Изменение статуса заказа
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, int newStatusId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var order = await _context.Orders.FindAsync(id);

            if (order == null || order.EngineerId != currentUserId)
            {
                return NotFound();
            }

            try
            {
                order.StatusId = newStatusId;
                if (newStatusId == 4) // Если статус "Завершен"
                {
                    order.CompletionDate = DateTime.UtcNow;
                }

                _context.Update(order);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Статус заказа успешно изменен";
            }
            catch (Exception)
            {
                TempData["Error"] = "Ошибка при изменении статуса";
            }

            return RedirectToAction(nameof(MyOrders));
        }

        // POST: Engineer/CancelOrder/5 - Отмена заказа
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var order = await _context.Orders.FindAsync(id);

            if (order == null || order.EngineerId != currentUserId)
            {
                return NotFound();
            }

            try
            {
                order.StatusId = 1; // Возвращаем статус "Принят оператором"
                order.EngineerId = null; // Снимаем инженера
                order.CompletionDate = null; // Сбрасываем дату завершения

                _context.Update(order);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Заказ успешно отменен";
            }
            catch (Exception)
            {
                TempData["Error"] = "Ошибка при отмене заказа";
            }

            return RedirectToAction(nameof(MyOrders));
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
            return engineers.Select(e => new SelectListItem
            {
                Value = e.Id,
                Text = e.FullName ?? e.UserName
            }).ToList();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
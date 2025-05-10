using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebRepairService.Models;

namespace WebRepairService.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext dbContext,
            ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserWithRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRolesViewModel.Add(new UserWithRoleViewModel
                {
                    Username = user.UserName,
                    UserId = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    RegistrationDate = user.RegistrationDate,
                    CurrentRole = roles.FirstOrDefault()
                });
            }

            return View(userRolesViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                CurrentRole = roles.FirstOrDefault(),
                AllRoles = allRoles
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            model.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Получаем DbContext через контекст пользователя
            var dbContext = _dbContext;

            // Начинаем транзакцию
            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                // 1. Обновляем email
                if (user.Email != model.Email)
                {
                    var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.Email);
                    var result = await _userManager.ChangeEmailAsync(user, model.Email, token);
                    if (!result.Succeeded)
                    {
                        AddErrors(result);
                        return View(model);
                    }
                }

                // 2. Обновляем username
                if (user.UserName != model.Email)
                {
                    var result = await _userManager.SetUserNameAsync(user, model.Email);
                    if (!result.Succeeded)
                    {
                        AddErrors(result);
                        return View(model);
                    }
                }

                // 3. Обновляем остальные поля
                user.FullName = model.FullName;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    AddErrors(updateResult);
                    return View(model);
                }

                // 4. Обновляем роли
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                }

                if (!string.IsNullOrEmpty(model.CurrentRole))
                {
                    await _userManager.AddToRoleAsync(user, model.CurrentRole);
                }

                // Сохраняем изменения и фиксируем транзакцию
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction("Users");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", $"Ошибка при сохранении: {ex.Message}");
                _logger.LogError(ex, "Ошибка при редактировании пользователя");
                return View(model);
            }
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return RedirectToAction("Users");
            }

            return RedirectToAction("Users");
        }
    }

    public class UserWithRoleViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; }

        public DateTime RegistrationDate { get; set; }
        public IList<string> Roles { get; set; } // Изменили на IList<string>

    }
}
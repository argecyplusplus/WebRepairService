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

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var thisViewModel = new UserRoleViewModel
                {
                    Username = user.UserName,
                    UserId = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    RegistrationDate = user.RegistrationDate,
                    Roles = await _userManager.GetRolesAsync(user),
                    ProfileImage = user.ProfileImage 
                };
                userRolesViewModel.Add(thisViewModel);
            }

            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(userRolesViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(string userId, string role, bool isSelected)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (isSelected)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            return RedirectToAction("Users");
        }


        [HttpPost]
        public async Task<IActionResult> SetRole(string userId, string selectedRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!string.IsNullOrEmpty(selectedRole))
            {
                await _userManager.AddToRoleAsync(user, selectedRole);
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser.Id == userId)
            {
                TempData["Error"] = "Вы не можете удалить свой собственный аккаунт";
                return RedirectToAction("Users");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Пользователь успешно удален";
                return RedirectToAction("Users");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToAction("Users");
        }

    }

    public class UserRoleViewModel
    {
        public string UserId { get; set; }

        public string Username {  get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; }

        public DateTime RegistrationDate { get; set; }
        public IList<string> Roles { get; set; }
        public string ProfileImage { get; set; }

    }
}
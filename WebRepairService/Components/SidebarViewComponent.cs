using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebRepairService.Models;
using Microsoft.AspNetCore.Http; 

public class SidebarViewComponent : ViewComponent
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SidebarViewComponent(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!User.Identity.IsAuthenticated)
            return Content(string.Empty);

        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null)
            return Content(string.Empty);

        var currentPath = _httpContextAccessor.HttpContext?.Request.Path.Value;

        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return View("AdminSidebar", currentPath); 

        if (await _userManager.IsInRoleAsync(user, "Operator"))
            return View("OperatorSidebar", currentPath);

        if (await _userManager.IsInRoleAsync(user, "Engineer"))
            return View("EngineerSidebar", currentPath);

        return Content(string.Empty);
    }
}
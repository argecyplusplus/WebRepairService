using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebRepairService.Models;

namespace WebRepairService.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Users", "Admin");
            }
            else if (User.IsInRole("Operator"))
            {
                return RedirectToAction("Index", "Operator");
            }
            else if (User.IsInRole("Engineer"))
            {
                return RedirectToAction("Index", "Engineer");
            }
            else
            {
                return View("Index");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

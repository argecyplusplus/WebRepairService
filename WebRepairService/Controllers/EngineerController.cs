using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebRepairService.Controllers
{
    [Authorize(Roles = "Engineer")]
    public class EngineerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Другие действия, доступные только инженеру
    }
}

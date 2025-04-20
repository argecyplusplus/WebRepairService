using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebRepairService.Controllers
{
    [Authorize(Roles = "Operator")]
    public class OperatorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Другие действия, доступные только оператору
    }
}

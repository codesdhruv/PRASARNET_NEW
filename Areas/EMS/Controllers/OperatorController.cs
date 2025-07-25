using Microsoft.AspNetCore.Mvc;

namespace PRASARNET.Areas.EMS.Controllers
{
    [Area("EMS")]
    public class OperatorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult EnterLoc()
        {
            return View();
        }
        public IActionResult ViewLoc()
        {
            return View();
        }
    }
}

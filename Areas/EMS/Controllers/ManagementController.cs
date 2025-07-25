using Microsoft.AspNetCore.Mvc;

namespace PRASARNET.Areas.EMS.Controllers
{
    [Area("EMS")]
    public class ManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

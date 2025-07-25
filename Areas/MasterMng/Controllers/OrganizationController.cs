using Microsoft.AspNetCore.Mvc;

namespace PRASARNET.Areas.MasterMng.Controllers
{
    public class OrganizationController : Controller
    {
        [Area("MasterMng")]
        public IActionResult OrganizationList()
        {
            return View();
        }
    }
}

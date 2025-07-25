using Microsoft.AspNetCore.Mvc;

namespace PRASARNET.ViewComponents
{
    public class SidebarViewComponentOLD : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var route = RouteData.Values["area"] != null ? RouteData.Values["area"].ToString() : null;

            //if (route == "MasterMng")
            //{
            //    return View("MstMngSideBar");
            //}
             if (route == "PrasarNet")
            {
                return View("PrasarNetSideBar");
            }
            else
            {
                return View("DashboardSideBar"); // sidebar for Operator user
            }
         
        }
    }
}

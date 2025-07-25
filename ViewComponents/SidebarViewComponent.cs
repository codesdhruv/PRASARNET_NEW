using Microsoft.AspNetCore.Mvc;
using PRASARNET.Repositories.Interfaces;
using PRASARNET.Services;

namespace PRASARNET.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly IUserDataRepository _userDataRepo;
        private readonly ISessionService _sessionService;
        public SidebarViewComponent(IUserDataRepository userDataRepo, ISessionService sessionService    )
        {
            _userDataRepo = userDataRepo;
            _sessionService = sessionService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int employeeId = Convert.ToInt32( _sessionService.GetEmployeeId());
            var menuItems = await _userDataRepo.FetchUserMenuByEmployeeIdAsync(employeeId);
            return View(menuItems);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRASARNET.Areas.MasterMng.Models;
using PRASARNET.Areas.MasterMng.Data;
using PRASARNET.Repositories;

namespace PRASARNET.Areas.MasterMng.Controllers
{
    [Area("MasterMng")]
    public class DashboardController : Controller
    {
        private readonly OrganizationRepository _organizationRepository;
        private readonly PermissionRepository _permissionRepository;

        public DashboardController(IConfiguration configuration)
        {
            _organizationRepository = new OrganizationRepository(configuration);
            _permissionRepository = new PermissionRepository(configuration);
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TreeView()
        {
            var allPermissions = _permissionRepository.GetAllPermissions();
            var permissionTree = BuildTree(allPermissions);
            return View(permissionTree);
        }


        public List<Permission> BuildTree(List<Permission> allPermissions, int? parentId = null)
        {
            return allPermissions
                .Where(p => p.ParentPermissionId == parentId)
                .Select(p =>
                {
                    p.Children = BuildTree(allPermissions, p.PermissionId);
                    return p;
                })
                .ToList();
        }

        public async Task<IActionResult> ManageUser()
        {
            var orgs = await _organizationRepository.getAllOrganisations();
            ViewBag.OrgList = new SelectList(orgs, "OrgId", "OrgName");
            return View();
        }

        public async Task<SelectList> GetOrganisationDetails(string selectedOrgId)
        {
            var orgs = await _organizationRepository.getAllOrganisations();
            return new SelectList(orgs, "OrgId", "OrgName");
        }


    }
}

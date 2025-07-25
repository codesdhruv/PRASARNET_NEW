using Microsoft.AspNetCore.Mvc;
using PRASARNET.Repositories;
using PRASARNET.Areas.MasterMng.Models;
using Mono.TextTemplating;

namespace PRASARNET.Areas.MasterMng.Controllers
{
    [Area("MasterMng")]
    public class RoleController : Controller
    {

        private readonly RolesRepository _roleRepo;

        public RoleController(RolesRepository roleRepo)
        {
            _roleRepo = roleRepo;
        }

        public IActionResult Index()
        {
            var roles = _roleRepo.GetAllRoles();
            return View(roles);
        }
     
        public IActionResult RoleDetails(int id)
        {
            var role = _roleRepo.GetRoleById(id);
            if (role == null)
            {
                return NotFound();
            }
            var permissions = _roleRepo.GetPermissionsByRoleId(id);
            ViewBag.rolePermissions = permissions;
            //var flatPermissions = _roleRepo.GetPermissionsByRoleId(id);
            //var permissionTree = BuildPermissionTree(flatPermissions);
            //ViewBag.PermissionTree = permissionTree;

            return View(role);
        }

        public IActionResult RoleCreate()
        {
            return View("RoleForm", new Roles());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RoleCreate([Bind("RoleName, IpAddress, IsActive")] Roles role)
        {
            ModelState.Remove("ActInctBy");
            ModelState.Remove("ActInctDate");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("UpdatedBy");
            ModelState.Remove("UpdatedAt");
            if (ModelState.IsValid)
            {
                _roleRepo.AddRole(role);
                return RedirectToAction(nameof(Index));
            }
            return View("RoleForm", role);
        }

        public IActionResult RoleEdit(int id)
        {
            var role = _roleRepo.GetRoleById(id);
            if (role == null)
            {
                return NotFound();
            }
            return View("RoleForm", role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RoleEdit(int id, [Bind("RoleID, RoleName, IpAddress, IsActive")] Roles role)
        {
            if (id != role.RoleID)
            {
                return NotFound();
            }
            ModelState.Remove("ActInctBy");
            ModelState.Remove("ActInctDate");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("UpdatedBy");
            ModelState.Remove("UpdatedAt");
            if (ModelState.IsValid)
            {
                role.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                role.UpdatedAt = DateTime.UtcNow;
                _roleRepo.UpdateRole(role);
                return RedirectToAction(nameof(Index));
            }
            return View("RoleForm", role); // Return with the current role if validation fails
        }



        public IActionResult RoleDelete(int id)
        {
            var role = _roleRepo.GetRoleById(id);
            if (role != null)
            {
                _roleRepo.DeleteRole(id);
            }
            return RedirectToAction("Index");
        }

        public List<Permission> BuildPermissionTree(List<Permission> flatList)
        {
            var lookup = flatList.ToLookup(p => p.ParentPermissionId);
            foreach (var perm in flatList)
            {
                perm.Children = lookup[perm.PermissionId].ToList();
            }
            return lookup[null].ToList(); // top-level nodes (ParentPermissionId == null)
        }

    }
}

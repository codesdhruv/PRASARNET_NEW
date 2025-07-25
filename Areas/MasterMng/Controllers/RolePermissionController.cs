using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRASARNET.Areas.MasterMng.Models;
using PRASARNET.Repositories;
using PRASARNET.Services;

namespace PRASARNET.Areas.MasterMng.Controllers
{
    [Area("MasterMng")]
    public class RolePermissionController : Controller
    {
        private readonly RolesRepository _roleRepo;

        public RolePermissionController(RolesRepository roleRepo)
        {
            _roleRepo = roleRepo;
        }

        public IActionResult Index()
        {
            var roles = _roleRepo.GetAllRoles();
            ViewBag.Roles = roles;
            ViewBag.Permissions = new List<Permission>();
            ViewBag.SelectedRoleID = null;
            return View();
        }

        [HttpGet]
        public IActionResult TreeView(int roleID)
        {
            var roles = _roleRepo.GetAllRoles();
            ViewBag.Roles = roles;

            var assignedPermissions = _roleRepo.GetPermissionsByRoleId(roleID);
            var assignedPermissionIds = assignedPermissions.Select(p => p.PermissionId).ToList();

            ViewBag.SelectedRoleID = roleID;

            var categories = GetCategoryTree(assignedPermissionIds);

            return PartialView("_CategoriesList", categories);
        }


        private List<Permission> GetCategoryTree(List<int> assignedPermissionIds)
        {
            var allCategories = _roleRepo.GetAllPermissions().ToList();
            var lookup = allCategories.ToLookup(c => c.ParentPermissionId);

            foreach (var category in allCategories)
            {
                category.Children = lookup[category.PermissionId].ToList();

                // Set IsAssigned true if PermissionId exists in assignedPermissionIds
                category.IsAssigned = assignedPermissionIds.Contains(category.PermissionId);
            }

            var rootCategories = lookup[null].ToList();
            return rootCategories;
        }

        //[HttpPost]
        //public IActionResult SubmitSelected(int roleID, List<int> selectedIds)
        //{
        //    // Clear existing permissions for the role
        //    _roleRepo.ClearPermissionsForRole(roleID);

        //    // Assign the new selected permissions
        //    _roleRepo.AssignPermissionsToRole(roleID, selectedIds);

        //    TempData["Message"] = "Permissions updated successfully!";
        //    return RedirectToAction("Index"); // or wherever you want to go
        //}



        [HttpPost]
        public IActionResult SubmitSelected(int roleId, List<int> selectedIds)
        {
            try
            {
                _roleRepo.UpdateRolePermissions(roleId, selectedIds);
                return RedirectToAction("Index");
                return Json(new { success = true, message = "Permissions updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


    }
}

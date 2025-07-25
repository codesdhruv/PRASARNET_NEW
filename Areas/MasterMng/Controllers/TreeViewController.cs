using Microsoft.AspNetCore.Mvc;
using PRASARNET.Areas.MasterMng.Models;
using PRASARNET.Repositories;

namespace PRASARNET.Areas.MasterMng.Controllers
{
    [Area("MasterMng")]
    public class TreeViewController : Controller
    {
        private readonly RolesRepository _roleRepo;

        public TreeViewController(RolesRepository roleRepo)
        {
            _roleRepo = roleRepo;
        }
        public IActionResult TreeView()
        {
            var categories = GetCategoryTree();
            return View(categories);
        }

        [HttpPost]
        public IActionResult SubmitSelected0(List<int> selectedIds)
        {
            var selectedCategories = _roleRepo.GetAllPermissions()
                                             .Where(c => selectedIds.Contains(c.PermissionId))
                                             .ToList();
            return View("SelectedCategories", selectedCategories);
        }

        private List<Permission> GetCategoryTree()
        {
            var allCategories = _roleRepo.GetAllPermissions().ToList();
            var lookup = allCategories.ToLookup(c => c.ParentPermissionId);

            foreach (var category in allCategories)
            {
                category.Children = lookup[category.PermissionId].ToList();
            }

            var rootCategories = lookup[null].ToList();
            return rootCategories;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRASARNET.Areas.MasterMng.Models;
using PRASARNET.Repositories;
using PRASARNET.Repositories.Interfaces;
using PRASARNET.Services;

namespace PRASARNET.Areas.MasterMng.Controllers
{
    [Area("MasterMng")]
    public class EmployeeController : Controller
    {
        private readonly EmployeeRepository _empRepo;
        public EmployeeController(IConfiguration config)
        {
            _empRepo = new EmployeeRepository(config);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> getEmployeeDetails(EmployeePersonalDetails empModel)
        {
            var employee = await _empRepo.GetEmployeeByCode(empModel.EmployeeCode);
            if (employee != null)
            {
                ViewBag.EmployeeDetail = employee;
                TempData["EmployeeDetails"] = JsonConvert.SerializeObject(employee);
                TempData.Keep("EmployeeDetails");
                return RedirectToAction("ManageRoles", new { employeeId = employee.EmployeeId });
            }

            TempData["Error"] = "Invalid Employee ID";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ManageRoles(int employeeId)
        {
            ViewBag.EmployeeId = employeeId;
            ViewBag.AllRoles = _empRepo.GetAllRoles();

            var empRoles = _empRepo.GetRolesByEmpId(employeeId);
            ViewBag.UserRoles = empRoles.Select(r => r.RoleID).ToList();
            ViewBag.UserAssignedRoles = empRoles.Select(r => r.RoleName).ToList();

            return View("Index");
        }

        [HttpPost]
        public IActionResult UpdateRoles(int employeeId, List<int> selectedRoleIds)
        {
            _empRepo.UpdateEmployeeRoles(employeeId, selectedRoleIds);
            TempData["Success"] = "Roles updated successfully.";
            return View("Index");

            return RedirectToAction("ManageRoles", new { employeeId = employeeId });
        }


    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using PRASARNET.Areas.TRAM.Models;
using PRASARNET.Areas.TRAM.Repositories;
using PRASARNET.Repositories;
using PRASARNET.Services;
using System.Threading.Tasks;

namespace PRASARNET.Areas.TRAM.Controllers
{
    [Area("TRAM")]
    public class TramController : Controller
    {
        private readonly ApplicantRepository _applicantRepository;
        private readonly ISessionService _sessionService;

        public TramController(IConfiguration configuration, ISessionService sessionService)
        {
            _applicantRepository = new ApplicantRepository(configuration);
            _sessionService = sessionService;
        }


        [HttpGet]
        public async Task<IActionResult> TransferApply()
        {
            int employeeId = Convert.ToInt32(_sessionService.GetEmployeeId());
            var ApplicantDetailmodel = await _applicantRepository.GetEmployeeDetailsAsync(employeeId);
           // ApplicantDetailmodel.States = await _applicantRepository.GetStateListAsync();
            //ApplicantDetailmodel.GroundsForTransfer = _applicantRepository.GetGroundsForTransfer();
            return View(ApplicantDetailmodel);
        }

        [HttpPost]
        public JsonResult TransferApply(TransferApplicationModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data" });
            }

            int empId = Convert.ToInt32(_sessionService.GetEmployeeId());
            string ip = _sessionService.GetEmployeeIP();

            //int basicDetId;
            //int result = _applicantRepository.SaveTransferApplication(model, ip, empId, out basicDetId);
            int result = 2;
            int basicDetId = 2;

            if (result > 0)
            {
                TempData["BasicDetId"] = basicDetId;
                TempData["New_old"] = "NApp";
                return Json(new { success = true, basicDetId = basicDetId });
            }

            return Json(new { success = false, message = "Save failed" });
        }


        [HttpGet]
        public JsonResult GetCities(int stateid, int? cityid1 = null, int? cityid2 = null)
        {
            var cities = _applicantRepository.GetCities(stateid, cityid1, cityid2);
            return Json(cities);
        }


        [HttpGet]
        public IActionResult GetTrainingRecords()
        {
            int empId = Convert.ToInt32(_sessionService.GetEmployeeId());
            var trainings = _applicantRepository.GetTrainingByEmployeeId(empId);
            return PartialView("_TrainingRecordsPartial", trainings);
        }

        [HttpPost]
        public JsonResult SaveTraining(TrainingModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data." });

            int empId = Convert.ToInt32(_sessionService.GetEmployeeId());
            string ip = _sessionService.GetEmployeeIP();
            int bid = Convert.ToInt32(TempData["BasicDetId"] ?? 0);

            model.EmployeeId = empId;
            bool saved = _applicantRepository.SaveTraining(model, ip, bid);

            return Json(new { success = saved });
        }

        [HttpPost]
        public JsonResult DeleteTraining(int id)
        {
            string ip = _sessionService.GetEmployeeIP();
            int bid = Convert.ToInt32(TempData["BasicDetId"] ?? 0); // or use session

            bool deleted = _applicantRepository.DeleteTraining(id, ip, bid);

            return Json(new { success = deleted });
        }


        public IActionResult GetTransferRecords()
        {
            int empId = Convert.ToInt32(HttpContext.Session.GetString("empid"));
            var transfers = _applicantRepository.GetTransferPostings(empId);
            return PartialView("_TransferRecordsPartial", transfers);
        }


    }
}

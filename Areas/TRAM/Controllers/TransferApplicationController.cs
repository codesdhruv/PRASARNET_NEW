using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Protocol.Core.Types;
using PRASARNET.Areas.MasterMng.Models;
using PRASARNET.Areas.TRAM.Models;
using PRASARNET.Areas.TRAM.Repositories;
using PRASARNET.Controllers;
using PRASARNET.Repositories;
using PRASARNET.Services;
using System.Reflection;

namespace PRASARNET.Areas.TRAM.Controllers
{
    [Area("TRAM")]
    public class TransferApplicationController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly ApplicantRepository _applicantRepository;

        public TransferApplicationController(ISessionService sessionService, ApplicantRepository applicantRepository)
        {
            _sessionService = sessionService;
            _applicantRepository = applicantRepository;
        }
        public IActionResult Index()
        {
            HttpContext.Session.Remove("basic_Det_id");
            int? userId = _sessionService.GetEmployeeId();
            if (userId == null)
            {
                // Handle unauthenticated or session timeout case
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            var apps = _applicantRepository.GetDraftedApplications(userId.Value);
            return View(apps);
        }

        [HttpGet]
        public async Task<IActionResult> Apply()
        {
            HttpContext.Session.SetInt32("basic_Det_id", -1);
            int? employeeId = _sessionService.GetEmployeeId();
            if (!employeeId.HasValue || employeeId.Value <= 0)
                return RedirectToAction("Login", "Account", new { area = "" });

            var empData = await _applicantRepository.GetEmployeeDetailsAsync(employeeId.Value);
            if (empData == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            ApplicantBasicDetailViewModel basicDetail;

            basicDetail = new ApplicantBasicDetailViewModel
            {
                // Pre-filled Basic values
                EmployeeCode = empData.EmployeeCode,
                Name_Emp = empData.Name_Emp,
                DOB = empData.DOB,
                DesignationName = empData.DesignationName,
                Mobile = empData.Mobile,
                Email = empData.Email,
                PresentStationName = empData.PresentStationName,
                DOJ_Present = empData.DOJ_Present,
                NoOfYearsServed = empData.NoOfYearsServed,
                DateSuperannuation = empData.DateSuperannuation,
                Tenure = empData.Tenure,
                TenureYear = empData.TenureYear,
                DaysCompletedOnTenure = empData.DaysCompletedOnTenure,
                DaysExceeded = empData.DaysExceeded,
                CategoryName = empData.CategoryName,
                ForwardedTo = empData.ForwardedTo,
                HOO_NameId = empData.HOO_NameId,

                // Dropdown lists
                States = await _applicantRepository.GetStateListAsync(),
                GroundsForTransfer = await _applicantRepository.GetGroundsForTransferAsync()
            };

            if (TempData["ApplicationId"] != null)
            {
                int applicationId = Convert.ToInt32(TempData["ApplicationId"]);
                var application = await _applicantRepository.GetApplicationByIdAsync(applicationId);

                // Set only the editable fields, retain dropdowns already populated
                basicDetail.ApplicationId = application.ApplicationId;
                basicDetail.ApplicationNo = application.ApplicationNo;
                basicDetail.Language = application.Language;
                basicDetail.Hometown = application.Hometown;
                basicDetail.SelectedCity1 = application.SelectedCity1;
                basicDetail.SelectedCity2 = application.SelectedCity2;
                basicDetail.SelectedCity3 = application.SelectedCity3;
                basicDetail.SelectedState1 = application.SelectedState1;
                basicDetail.SelectedState2 = application.SelectedState2;
                basicDetail.SelectedState3 = application.SelectedState3;
                basicDetail.TenureOption1 = application.TenureOption1;
                basicDetail.TenureOption2 = application.TenureOption2;
                basicDetail.TenureOption3 = application.TenureOption3;
                basicDetail.ReasonIfNone = application.ReasonIfNone;
                basicDetail.SelectedGroundId = application.SelectedGroundId;
                basicDetail.Subject = application.Subject;
                basicDetail.Details = application.Details;
                basicDetail.MedicalFacilitiesAvailable = application.MedicalFacilitiesAvailable;
                basicDetail.ForwardedTo = application.ForwardedTo;
                basicDetail.HOO_NameId = application.HOO_NameId;
            }


            //int? applicationId = HttpContext.Session.GetInt32("transfer_App_id");
            //if (applicationId.HasValue && applicationId.Value > 0)
            //{
            //    var application = await _applicantRepository.GetApplicationByIdAsync(applicationId.Value);
                
            //    // Set only the editable fields, retain dropdowns already populated
            //    basicDetail.ApplicationId = application.ApplicationId;
            //    basicDetail.ApplicationNo = application.ApplicationNo;
            //    basicDetail.Language = application.Language;
            //    basicDetail.Hometown = application.Hometown;
            //    basicDetail.SelectedCity1 = application.SelectedCity1;
            //    basicDetail.SelectedCity2 = application.SelectedCity2;
            //    basicDetail.SelectedCity3 = application.SelectedCity3;
            //    basicDetail.SelectedState1 = application.SelectedState1;
            //    basicDetail.SelectedState2 = application.SelectedState2;
            //    basicDetail.SelectedState3 = application.SelectedState3;
            //    basicDetail.TenureOption1 = application.TenureOption1;
            //    basicDetail.TenureOption2 = application.TenureOption2;
            //    basicDetail.TenureOption3 = application.TenureOption3;
            //    basicDetail.ReasonIfNone = application.ReasonIfNone;
            //    basicDetail.SelectedGroundId = application.SelectedGroundId;
            //    basicDetail.Subject = application.Subject;
            //    basicDetail.Details = application.Details;
            //    basicDetail.MedicalFacilitiesAvailable = application.MedicalFacilitiesAvailable;
            //    basicDetail.ForwardedTo = application.ForwardedTo;
            //    basicDetail.HOO_NameId = application.HOO_NameId;
            //}
            
            var dashboardModel = new ApplicantDashboardViewModel
            {
                BasicDetail = new List<ApplicantBasicDetailViewModel> { basicDetail },
                Trainings = await _applicantRepository.GetTrainingByEmployeeIdAsync(employeeId.Value),
                TransferHistory = await _applicantRepository.GetPreviousTransferPostingsAsync(employeeId.Value),
                OtherRecords = await _applicantRepository.GetOtherRecordsByEmployeeAsync(employeeId.Value),
                HealthCategories = await _applicantRepository.GetHealthCategoriesByEmployeeIdAsync(employeeId.Value),
                UploadedDocuments = await _applicantRepository.GetUploadedDocumentsByEmployeeIdAsync(employeeId.Value)
            };

            return View(dashboardModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveBasicDetail(ApplicantDashboardViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    TempData["Error"] = "Please correct the form errors.";
            //    return View("Apply", model);
            //}
            try
            {
                int empId = Convert.ToInt32(_sessionService.GetEmployeeId());
                string ip = _sessionService.GetEmployeeIP();
                int fwdToId = Convert.ToInt32(model.BasicDetail[0].HOO_NameId);

                bool isNew = model.BasicDetail[0].ApplicationId == 0;
                int basicDetailId;

                if (isNew)
                {
                    basicDetailId = await _applicantRepository.SaveBasicDetailsAsync(model.BasicDetail[0], ip, empId, fwdToId, empId);
                }
                else
                {
                    basicDetailId = await _applicantRepository.UpdateBasicDetailsAsync(model.BasicDetail[0], ip, empId, fwdToId, empId);
                }

                if (basicDetailId > 0)
                {
                    HttpContext.Session.SetInt32("basic_Det_id", basicDetailId); // Save ID in session for subsequent steps
                    return Json(new { success = true, id = basicDetailId });
                }
                return Json(new { success = false, message = "Failed to save details" });
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error in SaveBasicDetail");
                return Json(new { success = false, message = "Exception occurred" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            TempData["ApplicationId"] = id; // Application ID to pass to next action
            return RedirectToAction("Apply");
        }

        [HttpGet]
        public JsonResult GetCities(int stateid, int? cityid1 = null, int? cityid2 = null)
        {
            var cities = _applicantRepository.GetCities(stateid, cityid1, cityid2);
            return Json(cities);
        }

        public async Task<IActionResult> GetTrainingsPartial()
        {
            int empId = _sessionService.GetEmployeeId().Value;
            var trainings = await _applicantRepository.GetTrainingByEmployeeIdAsync(empId);
            return PartialView("_TrainingsPartial", trainings);
        }
        [HttpPost]
        public async Task<IActionResult> AddTraining(TrainingDetailViewModel model)
        {
            //if (!ModelState.IsValid)
            //    return RedirectToAction("Index");

            var employeeId = Convert.ToInt32(_sessionService.GetEmployeeId());
            model.EmployeeId = employeeId;
            model.CreatedBy = employeeId;
            model.Ip = _sessionService.GetEmployeeIP().ToString();
            model.BasicDetailsId = Convert.ToInt32(HttpContext.Session.GetInt32("basic_Det_id"));


            var result = await _applicantRepository.AddTrainingAsync(model);
            if (result > 0)
                return Ok(); // ← Success

            return BadRequest("Failed to add training"); // ← Error
        }
        [HttpPost]
        public async Task<IActionResult> DeleteTraining(int id)
        {
            try
            {
                string ip = _sessionService.GetEmployeeIP();
                int bid = Convert.ToInt32(HttpContext.Session.GetInt32("basic_Det_id"));
                var employeeId = Convert.ToInt32(_sessionService.GetEmployeeId());

                int result = await _applicantRepository.ExecuteDeleteTrainingAsync(id, ip, bid);

                if (result > 0)
                    return Ok(); // ← Success

                return BadRequest("Failed to Delete training"); // ← Error
            }
            catch (Exception)
            {
                return StatusCode(500, "Server error");
            }
        }


    }
}
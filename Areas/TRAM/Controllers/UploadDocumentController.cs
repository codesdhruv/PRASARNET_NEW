using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using PRASARNET.Areas.TRAM.Models;
using PRASARNET.Areas.TRAM.Repositories;
using PRASARNET.Services;

namespace PRASARNET.Areas.TRAM.Controllers
{
    [Area("TRAM")]
    public class UploadDocumentController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly ApplicantRepository _applicantRepository;

        public UploadDocumentController(ISessionService sessionService, ApplicantRepository applicantRepository)
        {
            _sessionService = sessionService;
            _applicantRepository = applicantRepository;
        }
        public async Task<IActionResult> Index()
        {
            int employeeId = Convert.ToInt32(_sessionService.GetEmployeeId());
            if (employeeId == null) return RedirectToAction("Login", "Account", new { area = "" });

            //var trainings = await _applicantRepository.GetTrainingByEmployeeIdAsync(employeeId);
            //var transferHistory = await _applicantRepository.GetPreviousTransferPostingsAsync(employeeId);
            //var OtherRrecords = await _applicantRepository.GetOtherRecordsByEmployeeAsync(employeeId);
            //var health = await _applicantRepository.GetHealthCategoriesByEmployeeIdAsync(employeeId);
            //var documents = await _applicantRepository.GetUploadedDocumentsByEmployeeIdAsync(employeeId);


            var viewModel = new ApplicantDashboardViewModel
            {
                Trainings = await _applicantRepository.GetTrainingByEmployeeIdAsync(employeeId),
                TransferHistory = await _applicantRepository.GetPreviousTransferPostingsAsync(employeeId),
                OtherRecords = await _applicantRepository.GetOtherRecordsByEmployeeAsync(employeeId),
                HealthCategories = await _applicantRepository.GetHealthCategoriesByEmployeeIdAsync(employeeId),
                UploadedDocuments = await _applicantRepository.GetUploadedDocumentsByEmployeeIdAsync(employeeId)
            };

            return View(viewModel);

        }
    }




}

using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using PRASARNET.Areas.TRAM.Models;
using PRASARNET.Areas.TRAM.Repositories;
using PRASARNET.Services;

namespace PRASARNET.Areas.TRAM.Controllers
{
    [Area("TRAM")]
    public class HandelRequestController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly HandelApplicationRepository _handelApplicationRepository;

        public HandelRequestController(ISessionService sessionService, HandelApplicationRepository handelApplicationRepository)
        {
            _sessionService = sessionService;
            _handelApplicationRepository = handelApplicationRepository;
        }
        public async Task<IActionResult> Index()
        {
            int usertypeId = 10;
            int dealingEmpId = Convert.ToInt32(_sessionService.GetEmployeeId());

            var applicationsRequested = await _handelApplicationRepository.GetPendingApplicationsAsync(usertypeId, dealingEmpId);
            return View(applicationsRequested);
        }
        public async Task<IActionResult> TrackDetails(int basicId)
        {
            var trackData = await _handelApplicationRepository.GetApplicationTrackAsync(basicId);
            return PartialView("_TrackDetailsPartial", trackData);
        }
        public IActionResult LoadCertifyModal(int appId)
        {
            var model = new CertificationViewModel
            {
                ApplicationNo = appId
            };
            return PartialView("_CertifyPartial", model);
        }
        [HttpPost]
        public async Task<IActionResult> CertifyApplication(CertificationViewModel model)
        {
            //if (!ModelState.IsValid)
            //    return RedirectToAction("Index");

            int empId = Convert.ToInt32(_sessionService.GetEmployeeId());
            string ip = _sessionService.GetEmployeeIP();

            bool result = await _handelApplicationRepository.CertifyApplicationAsync(model.ApplicationNo, empId, ip, model.Remarks);

            TempData["CertifyMessage"] = result ? "Certification successful." : "Certification failed.";
            return RedirectToAction("Index");
        }


    }
}
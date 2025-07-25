using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRASARNET.Areas.MasterMng.Data;
using PRASARNET.Areas.MasterMng.Models;

namespace PRASARNET.Areas.MasterMng.Controllers
{
    [Area("MasterMng")]
    public class StationTypeController : Controller
    {
        public readonly StationRepository _stationRepository;
        private readonly OrganizationRepository _organizationRepository;
        public StationTypeController(IConfiguration configuration)
        {
            _stationRepository = new StationRepository(configuration);
            _organizationRepository = new OrganizationRepository(configuration);
        }

        public IActionResult StationTypeList()
        {
            var list = _stationRepository.GetAllStationType();
            return View(list);
        }
        public async Task<IActionResult> Create()
        {
            var model = new StationTypeModel();
            await PopulateOrganizationList(model);
            return View("CreateEdit", model);

        }

        [HttpPost]
        public IActionResult Create(StationTypeModel model)
        {
            if (ModelState.IsValid)
            {
                _stationRepository.AddStationType(model);
                return RedirectToAction("StationTypeList");
            }
            return View("CreateEdit", model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = _stationRepository.GetStationTypeById(id);
            await PopulateOrganizationList(model);
            return View("CreateEdit", model);
        }

        [HttpPost]
        public IActionResult Edit(StationTypeModel model)
        {
            if (ModelState.IsValid)
            {
                _stationRepository.UpdateStationType(model);
                return RedirectToAction("StationTypeList");
            }
            return View("CreateEdit", model);
        }

        public IActionResult Details(int id)
        {
            var model = _stationRepository.GetStationTypeById(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var model = _stationRepository.GetStationTypeById(id);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _stationRepository.DeleteStationType(id);
            return RedirectToAction("StationTypeList");
        }


        private async Task PopulateOrganizationList(StationTypeModel model)
        {
            var organizations = await _organizationRepository.getAllOrganisations();
            model.OrganizationList = organizations
                .Select(o => new SelectListItem
                {
                    Value = o.OrgId.ToString(),
                    Text = o.OrgName
                })
                .ToList();
        }

    }
}

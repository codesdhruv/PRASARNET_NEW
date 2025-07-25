using Microsoft.AspNetCore.Mvc;
using PRASARNET.Areas.MasterMng.Models;
using PRASARNET.Services;

namespace PRASARNET.Areas.MasterMng.Controllers
{
    [Area("MasterMng")]
    public class MastersController : Controller
    {
        private readonly StateRepository _repo;

        public MastersController(IConfiguration configuration)
        {
            _repo = new StateRepository(configuration);
        }
      

        // GET: /States/
        public IActionResult StateView()
        {
            var states = _repo.GetAllStates();
            return View(states);
        }
        public IActionResult StateCreate()
        {
            return View();
        }
        [HttpPost]
        public IActionResult StateCreate(State state)
        {
            state.UpdatedBy = null;
            state.CreatedBy = null;
            if (ModelState.IsValid)
            {
                _repo.AddState(state);
                return RedirectToAction("StateView");
            }
            return View(state);
        }
        public IActionResult StateEdit(int id)
        {
            var state = _repo.GetStateById(id);
            return View("StateCreate", state);
        }
        [HttpPost]
        public IActionResult StateEdit(State state)
        {
            if (ModelState.IsValid)
            {
                _repo.UpdateState(state);
                return RedirectToAction("StateView");
            }
            return View("StateCreate", state); // Return with the current state if validation fails
        }
        public IActionResult StateDelete(int id)
        {
            var state = _repo.GetStateById(id);
            if (state != null)
            {
                _repo.DeleteState(id);
            }
            return RedirectToAction("StateView");
        }
        public IActionResult StateDetails(int id)
        {
            var state = _repo.GetStateById(id);
            return View(state);
        }

    }
}

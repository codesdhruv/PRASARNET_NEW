using Microsoft.AspNetCore.Mvc;
using PRASARNET.Areas.MasterMng.Data;
using System.Configuration;

namespace PRASARNET.Areas.MasterMng.Controllers
{
    [Area("MasterMng")]
    public class StationController : Controller
    {
        private readonly StationRepository _stationRepository;
        public StationController(IConfiguration configuration)
        {
            _stationRepository = new StationRepository(configuration);
        }
        public IActionResult Index()
        {
            var stations = _stationRepository.GetAll();
            return View(stations);
        }
    }
}

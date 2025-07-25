using Microsoft.AspNetCore.Mvc;
using System.Data;
using PRASARNET.Services;
using Microsoft.Data.SqlClient;
using PRASARNET.Areas.PrasarNet.Services;

namespace PRASARNET.Controllers;
public class HomeController : Controller
{
    private readonly ISessionService _sessionservices;
    private readonly CircularService _circularService;
    private readonly IConfiguration _configuration;
    public HomeController(SessionService sessionservices, CircularService circularService, IConfiguration configuration)
    {
        _sessionservices = sessionservices;
        _circularService = circularService;
        _configuration = configuration;
    }

    //[Authorize(Policy = "CanViewDashboard")]


    public IActionResult Index()
    {
        if (!_sessionservices.IsSessionValid())
        {
            _sessionservices.ClearSession();
            return Redirect("/Account/Logout");
        }
        string connStr = _configuration.GetConnectionString("HRISConnection");

        // Get Circular last 10
        var circulars = _circularService.GetCirculars();
        var employeecorner = _circularService.GetEmployeeCorner();
        // Get Retirement Data
        DataSet dsRetirement = new DataSet();
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("IT_MonthlyRetire_Top4", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dsRetirement);
                }
            }
        }

        // Get Birthday Data
        DataSet dsBirthday = new DataSet();
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("IT_BirthDay_RSS_top4", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dsBirthday);
                }
            }
        }

        // Pass to View
        ViewBag.Birthdays = dsBirthday;
        ViewBag.Retirements = dsRetirement;
        ViewBag.employeecorner = employeecorner;

        return View(circulars);
    }

    public IActionResult Portals()
    {
        return View();
    }

}

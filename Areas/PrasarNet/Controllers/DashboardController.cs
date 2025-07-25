using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using PRASARNET.Areas.PrasarNet.Services;
using PRASARNET.Controllers;
using PRASARNET.Models;
using System.Configuration;
using System.Data;
using System.Diagnostics;

namespace PRASARNET.Areas.PrasarNet.Controllers
{
    [Area("PrasarNet")]   
    public class DashboardController : Controller
    {        
        private readonly CircularService _circularService;
        private readonly IConfiguration _configuration;

        public DashboardController(CircularService circularService, IConfiguration configuration)
        {
            _circularService = circularService;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            string connStr = _configuration.GetConnectionString("HRISConnection");

            // Get Circular last 10
            var circulars = _circularService.GetCirculars();           

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

            return View(circulars);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadDocument(int docId)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            // Fetch metadata from DB (simulate your stored procedure logic)
            DataSet ds1, ds2;
            string urlfixedpath;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Get base path (simulate PN18_URL_get)
                using (SqlCommand cmd = new SqlCommand("PN18_URL_get", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@typeURL", 1);
                    cmd.Parameters.AddWithValue("@flag", 1);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        ds1 = new DataSet();
                        da.Fill(ds1);
                        urlfixedpath = ds1.Tables[0].Rows[0][1].ToString();
                    }
                }

                // Get document info (simulate PN18_documentsupload)
                using (SqlCommand cmd = new SqlCommand("PN18_documentsupload", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@flag", 4);
                    cmd.Parameters.AddWithValue("@docid", docId);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        ds1 = new DataSet();
                        da.Fill(ds1);
                    }
                }

                // Get document type info (simulate PN18_DocTypen)
                using (SqlCommand cmd = new SqlCommand("PN18_DocTypen", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@docid", Convert.ToInt32(ds1.Tables[0].Rows[0][4]));
                    cmd.Parameters.AddWithValue("@flag", 2);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        ds2 = new DataSet();
                        da.Fill(ds2);
                    }
                }
            }

            if (ds2.Tables[0].Rows.Count == 0)
                return NotFound("No document type info found.");

            string folder = ds2.Tables[0].Rows[0][5].ToString();
            string fileName = ds1.Tables[0].Rows[0][9].ToString();
            string filePath = urlfixedpath + folder + "/" + fileName;

            if (string.IsNullOrEmpty(fileName))
                return NotFound("No file name found.");

            string extension = Path.GetExtension(fileName).ToLower();
            if (extension != ".pdf")
                return BadRequest("Only PDF files are supported for download.");

            using (var client = new HttpClient())
            {
                try
                {
                    byte[] fileBytes = await client.GetByteArrayAsync(filePath);
                    return File(fileBytes, "application/pdf", fileName);
                }
                catch
                {
                    return NotFound("Failed to download the file.");
                }
            }
        }

        [HttpGet]
        public IActionResult AllCircular()
        {
            var categories = _circularService.GetDocumentCategories();
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        public IActionResult GetPaginatedCirculars()
        {
            var category = Request.Form["Category"].FirstOrDefault() ?? "";
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var length = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "10");
            var searchValue = Request.Form["search[value]"].FirstOrDefault() ?? "";

            int totalRecords;
            var data = _circularService.GetPaginatedCirculars(start, length, searchValue, out totalRecords, category);

            var result = new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            };

            return Json(result);
        }


    }
}

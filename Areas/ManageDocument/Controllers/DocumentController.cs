using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.FileIO;
using PRASARNET.Areas.ManageDocument.Data;
using PRASARNET.Areas.ManageDocument.Models;
using PRASARNET.Areas.PrasarNet.Models;
using PRASARNET.Services;
using System.Data;
using System.Security.Claims;

namespace PRASARNET.Areas.ManageDocument.Controllers
{
    [Area("ManageDocument")]
    [Authorize(Policy = "Permission:Upload Orders/Circulars")]
    public class DocumentController : Controller
    {
        private readonly DocumentRepository _documentRepository;
        private readonly IWebHostEnvironment _env;
        public readonly string _connectionString;

        public DocumentController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _documentRepository = new DocumentRepository(configuration);
            _env = env;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Upload(int? id)
        {
            var model = new DocumentUploadViewModel
            {
                UploadTypeList = _documentRepository.GetUploadTypes()
            };

            if (id.HasValue)
            {
                // Load from DB for editing
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM PN18_Documents WHERE Docid = @Docid", conn);
                cmd.Parameters.AddWithValue("@Docid", id.Value);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model.DocId = id;
                    model.SelectedCategory = Convert.ToInt32(reader["Doc_Typeid"]);
                    model.Title = reader["Doc_Title"].ToString();
                    model.OrderNumber = reader["Doc_OrderNo"]?.ToString();
                    model.FileNumber = reader["Doc_FileNo"]?.ToString();
                    model.DocumentDate = Convert.ToDateTime(reader["Doc_Dated"]);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(DocumentUploadViewModel model)
        {
            model.UploadTypeList = _documentRepository.GetUploadTypes();

            if (!ModelState.IsValid)
                return View(model);

            if (model.UploadedFile == null || Path.GetExtension(model.UploadedFile.FileName).ToLower() != ".pdf")
            {
                ModelState.AddModelError("UploadedFile", "Only PDF files are allowed.");
                return View(model);
            }

            string urlFixedPath = ""; // Load from DB using SP if needed
            string fileName = "";
            int result = 0;
            string subFolderName = "";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // Get subfolder for doc type
                using (SqlCommand sp = new SqlCommand("PN18_DocTypen", conn))
                {
                    sp.CommandType = CommandType.StoredProcedure;
                    sp.Parameters.AddWithValue("@docid", model.SelectedCategory);
                    sp.Parameters.AddWithValue("@flag", 2);
                    using var da = new SqlDataAdapter(sp);
                    var ds = new DataSet();
                    da.Fill(ds);
                    subFolderName = ds.Tables[0].Rows[0][5].ToString();
                }

                //string dtetm = DateTime.Now.ToString("ddoMMoyyyy_HHoMMoSS");

                string dtetm = ((Convert.ToString(DateTime.Now).Replace('/', 'o')).Replace(':', 'o')).Trim();
                fileName = $"Doc{(model.DocId ?? GetNextDocId())}_{dtetm}.pdf";


                //string savefile = "Doc" + max + "_" + dtetm + filetype;
                string wwwrootPath = Path.Combine(_env.WebRootPath, "UploadedDocuments", subFolderName);
                Directory.CreateDirectory(wwwrootPath);
                string filePath = Path.Combine(wwwrootPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.UploadedFile.CopyToAsync(stream);
                }

                // Call SP to insert/update
                SqlCommand cmd = new SqlCommand("PN18_documentsupload", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@orgid", 3); // Replace with session value
                cmd.Parameters.AddWithValue("@usertypeid", 12); // Replace
                cmd.Parameters.AddWithValue("@doctypeid", model.SelectedCategory);
                cmd.Parameters.AddWithValue("@doctitle", model.Title);
                cmd.Parameters.AddWithValue("@docdated", model.DocumentDate);
                cmd.Parameters.AddWithValue("@orderNo", model.OrderNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@fileNo", model.FileNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@docname", fileName);
                cmd.Parameters.AddWithValue("@createdby", User.Identity.Name ?? "admin");
                cmd.Parameters.AddWithValue("@flag", model.DocId.HasValue ? 5 : 1);
                if (model.DocId.HasValue)
                    cmd.Parameters.AddWithValue("@docid", model.DocId.Value);

                // Optional station/section check
                if (User.IsInRole("Station"))
                    cmd.Parameters.AddWithValue("@stationid", 58);
                else
                    cmd.Parameters.AddWithValue("@sectionid", 99); // Sample

                result = cmd.ExecuteNonQuery();
            }

            if (result > 0)
            {
                TempData["SuccessMessage"] = model.DocId.HasValue ? "Updated successfully." : "Uploaded successfully.";
                return RedirectToAction("Upload");
            }

            ModelState.AddModelError("", "Upload failed. Try again.");
            return View(model);
        }

        private int GetNextDocId()
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand("SELECT ISNULL(MAX(Docid), 0) + 1 FROM PN18_Documents", conn);
            return (int)cmd.ExecuteScalar();
        }

        //[HttpGet]
        //public IActionResult DownloadDocument(int docId)
        //{
        //    var document = ""; // GetDocumentById(docId);

        //    if (document == null || string.IsNullOrEmpty(document.FileName))
        //    {
        //        return NotFound("Document not found.");
        //    }

        //    // Construct file path
        //    string subfolder = document.SubFolder; // e.g., "Circulars"
        //    string fileName = document.FileName;   // e.g., "Doc60884_202505291200.pdf"

        //    string basePath = "";//_configuration.GetValue<string>("DocumentUploadBasePath");
        //    string filePath = Path.Combine(basePath, subfolder, fileName);

        //    if (!System.IO.File.Exists(filePath))
        //    {
        //        return NotFound("File not found on server.");
        //    }

        //    var fileBytes = System.IO.File.ReadAllBytes(filePath);
        //    return File(fileBytes, "application/pdf", fileName);
        //}

        //private DocumentInfo GetDocumentById(int docId)
        //{
        //    // Example stub. Replace with actual DB fetch logic.
        //    return new DocumentInfo
        //    {
        //        DocId = docId,
        //        FileName = $"Doc{docId}_202505291200.pdf",
        //        SubFolder = "Circulars"
        //    };
        //}
    }
}

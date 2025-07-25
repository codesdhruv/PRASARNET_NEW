using Microsoft.AspNetCore.Mvc;
using PRASARNET.Areas.PrasarNet.Models;

namespace PRASARNET.Areas.PrasarNet.Controllers
{
    [Area("PrasarNet")]
    public class DocumentController : Controller
    {
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(UploadViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.DocumentFile != null && model.DocumentFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var filePath = Path.Combine(uploadsFolder, model.DocumentFile.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.DocumentFile.CopyToAsync(stream);
                    }

                    TempData["Success"] = "Document uploaded successfully!";
                    return RedirectToAction("Upload");
                }
            }

            TempData["Error"] = "Upload failed. Please try again.";
            return View(model);
        }
    }
}

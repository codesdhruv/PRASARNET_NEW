using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PRASARNET.Areas.ManageDocument.Models
{
    public class DocumentUploadViewModel
    {
        public int? DocId { get; set; }

        [Required]
        public int SelectedCategory { get; set; }

        [Required]
        public string Title { get; set; }

        public string? OrderNumber { get; set; }
        public string? FileNumber { get; set; }

        [Required]
        public DateTime DocumentDate { get; set; }

        [Required]
        public IFormFile UploadedFile { get; set; }

        public string UploadButtonText => DocId.HasValue ? "Update" : "Upload";

        public List<SelectListItem>? UploadTypeList { get; set; }


    }
}

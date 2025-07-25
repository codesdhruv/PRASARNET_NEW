using System.ComponentModel.DataAnnotations;

namespace PRASARNET.Areas.PrasarNet.Models
{
    public class UploadViewModel
    {
        [Required]
        public string DocumentTitle { get; set; }

        [Required]
        public IFormFile DocumentFile { get; set; }
    }
}

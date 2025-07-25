using Microsoft.AspNetCore.Mvc.Rendering;

namespace PRASARNET.Areas.MasterMng.Models
{
    public class StationTypeModel
    {
        public int StationTypeId { get; set; }
        public int OrganizationId { get; set; }
 
        public string StationType { get; set; }
        public string? Abbreviation { get; set; }
        public string? Remarks { get; set; }

        // Audit and system info
        public string? IpAddress { get; set; }
        public string? HostName { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? ActInctBy { get; set; }
        public DateTime? ActInctDate { get; set; }
        public bool IsActive { get; set; }


        // Organization info
        public string? OrganizationName { get; set; }
        // Dropdown List for Organizations
        public List<SelectListItem> OrganizationList { get; set; } = new List<SelectListItem>();

    }
}

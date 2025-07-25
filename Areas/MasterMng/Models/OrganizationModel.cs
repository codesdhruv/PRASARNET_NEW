namespace PRASARNET.Areas.MasterMng.Models
{
    public class OrganizationModel
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string Abbreviation { get; set; }
        public string Remarks { get; set; }
        public string IpAddress { get; set; }
        public string HostName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ActInctBy { get; set; }
        public DateTime? ActInctDate { get; set; }
        public bool IsActive { get; set; }
    }
}

namespace PRASARNET.Areas.MasterMng.Models
{
    public class Roles
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string IpAddress { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? ActInctBy { get; set; }
        public DateTime? ActInctDate { get; set; }
        public bool IsActive { get; set; }
    }
}

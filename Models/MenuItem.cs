namespace PRASARNET.Models
{
    public class MenuItem
    {
        public int ParentPermissionId { get; set; }
        public string ParentPermissionName { get; set; }
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public int EmployeeId { get; set; }
        public string ActionMethod { get; set; }
        public string ControllerClass { get; set; }
        public string Area { get; set; }
    }
}

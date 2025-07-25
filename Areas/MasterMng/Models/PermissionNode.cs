namespace PRASARNET.Areas.MasterMng.Models
{
    public class PermissionNode
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public List<PermissionNode> Children { get; set; } = new List<PermissionNode>();
    }
}

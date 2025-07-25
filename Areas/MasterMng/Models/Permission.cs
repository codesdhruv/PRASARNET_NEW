using Humanizer;

namespace PRASARNET.Areas.MasterMng.Models
{
    public class Permission
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public string PermissionDescr { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? ParentPermissionId { get; set; }

        public bool IsAssigned { get; set; }

        //Permission object needs to hold a list of its children — which are also Permission objects.
        public List<Permission> Children { get; set; } = new List<Permission>();
    }
}

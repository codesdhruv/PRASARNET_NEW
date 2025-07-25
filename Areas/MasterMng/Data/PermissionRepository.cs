using Microsoft.Data.SqlClient;
using PRASARNET.Areas.MasterMng.Models;

namespace PRASARNET.Areas.MasterMng.Data
{
    public class PermissionRepository
    {
        private readonly string _connectionString;
        public PermissionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public List<Permission> GetAllPermissions()
        {
            var permissions = new List<Permission>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT PermissionId, PermissionName, PermissionDescr, CreatedAt, UpdatedAt, ParentPermissionId FROM PN25_Trn_Permissions";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        permissions.Add(new Permission
                        {
                            PermissionId = reader.GetInt32(0),
                            PermissionName = reader.IsDBNull(1) ? null : reader.GetString(1),
                            PermissionDescr = reader.IsDBNull(2) ? null : reader.GetString(2),
                            CreatedAt = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                            UpdatedAt = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                            ParentPermissionId = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5)
                        });
                    }
                }
            }

            return permissions;
        }
    }
}

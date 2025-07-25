using Microsoft.Data.SqlClient;
using PRASARNET.Areas.MasterMng.Models;
using System.Data;

namespace PRASARNET.Repositories
{
    public class RolesRepository
    {
        private readonly string _connectionString;

        public RolesRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Roles> GetAllRoles()
        {
            var roles = new List<Roles>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM PN25_Mst_Role";
                SqlCommand cmd = new(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    roles.Add(new Roles
                    {
                        RoleID = Convert.ToInt32(reader["RoleID"]),
                        RoleName = reader["RoleName"].ToString(),
                        IpAddress = reader["IpAddress"].ToString(),
                        CreatedBy = reader["CreatedBy"]?.ToString(),
                        CreatedAt = reader["CreatedAt"] as DateTime?,
                        UpdatedBy = reader["UpdatedBy"]?.ToString(),
                        UpdatedAt = reader["UpdatedAt"] as DateTime?,
                        ActInctBy = reader["ActInctBy"]?.ToString(),
                        ActInctDate = reader["ActInctDate"] as DateTime?,
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                    });
                }
            }
            return roles;
        }

        public Roles GetRoleById(int id)
        {
            Roles role = null;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM PN25_Mst_Role WHERE RoleID = @id", con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                var rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    role = new Roles
                    {
                        RoleID = Convert.ToInt32(rdr["RoleID"]),
                        RoleName = rdr["RoleName"].ToString(),
                        IpAddress = rdr["IpAddress"].ToString(),
                        CreatedBy = rdr["CreatedBy"]?.ToString(),
                        CreatedAt = rdr["CreatedAt"] as DateTime?,
                        UpdatedBy = rdr["UpdatedBy"]?.ToString(),
                        UpdatedAt = rdr["UpdatedAt"] as DateTime?,
                        ActInctBy = rdr["ActInctBy"]?.ToString(),
                        ActInctDate = rdr["ActInctDate"] as DateTime?,
                        IsActive = Convert.ToBoolean(rdr["IsActive"])
                    };
                }
            }
            return role;
        }

        public void AddRole(Roles role)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
            INSERT INTO PN25_Mst_Role (RoleName, IpAddress, IsActive, CreatedBy, CreatedAt)
            VALUES (@RoleName, @IpAddress, @IsActive, @CreatedBy, @CreatedAt);
        ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@RoleName", role.RoleName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IpAddress", role.IpAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", role.IsActive);
                    cmd.Parameters.AddWithValue("@CreatedBy", /* your user ID or 0 */ 0);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateRole(Roles role)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"
                UPDATE PN25_Mst_Role 
                SET RoleName = @RoleName,
                    IpAddress = @IpAddress,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt,
                    IsActive = @IsActive
                WHERE RoleID = @RoleID", con);

                cmd.Parameters.AddWithValue("@RoleID", role.RoleID);
                cmd.Parameters.AddWithValue("@RoleName", role.RoleName);
                cmd.Parameters.AddWithValue("@IpAddress", role.IpAddress ?? "");
                cmd.Parameters.AddWithValue("@UpdatedBy", role.UpdatedBy ?? "");
                cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                cmd.Parameters.AddWithValue("@IsActive", role.IsActive);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteRole(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM PN25_Mst_Role WHERE RoleID = @id", con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Permission> GetPermissionsByRoleId(int roleId)
        {
            List<Permission> permissions = new List<Permission>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
            SELECT p.PermissionId, p.PermissionName, p.PermissionDescr, p.ParentPermissionId
            FROM PN25_Trn_Permissions p
            INNER JOIN PN25_Mst_RolePermission rp ON p.PermissionId = rp.PermissionId
            WHERE rp.RoleID = @RoleID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RoleID", roleId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        permissions.Add(new Permission
                        {
                            PermissionId = Convert.ToInt32(reader["PermissionId"]),
                            PermissionName = reader["PermissionName"].ToString(),
                            PermissionDescr = reader["PermissionDescr"].ToString(),
                            ParentPermissionId = reader["ParentPermissionId"] != DBNull.Value
                                ? Convert.ToInt32(reader["ParentPermissionId"])
                                : (int?)null
                        });
                    }
                }
            }

            return permissions;
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

        public bool RoleExists(int roleId)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            string query = "SELECT COUNT(*) FROM PN25_Mst_Role WHERE RoleID = @RoleID";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@RoleID", roleId);
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        public void UpdateRolePermissions(int roleId, List<int> permissionIds)
        {
            if (!RoleExists(roleId))
            {
                throw new ArgumentException($"RoleID {roleId} does not exist in Mst_Role table.");
            }

            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            using SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                // 1. Delete existing permissions for the role
                string deleteQuery = "DELETE FROM PN25_Mst_RolePermission WHERE RoleID = @RoleID";
                using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn, transaction))
                {
                    deleteCmd.Parameters.AddWithValue("@RoleID", roleId);
                    deleteCmd.ExecuteNonQuery();
                }

                // 2. Insert new permissions
                string insertQuery = @"INSERT INTO PN25_Mst_RolePermission (RoleID, PermissionId, CreatedAt)
                               VALUES (@RoleID, @PermissionId, GETDATE())";

                foreach (int permissionId in permissionIds)
                {
                    using SqlCommand insertCmd = new SqlCommand(insertQuery, conn, transaction);
                    insertCmd.Parameters.AddWithValue("@RoleID", roleId);
                    insertCmd.Parameters.AddWithValue("@PermissionId", permissionId);
                    insertCmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw; // rethrow to caller
            }
        }


    }
}

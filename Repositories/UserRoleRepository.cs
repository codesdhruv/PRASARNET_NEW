using Microsoft.Data.SqlClient;
using PRASARNET.Areas.MasterMng.Models;
using PRASARNET.Models;
using PRASARNET.Repositories.Interfaces;

namespace PRASARNET.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly IConfiguration _config;
        public UserRoleRepository(IConfiguration config) => _config = config;

        private SqlConnection GetConnection() => new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        public async Task<List<string>> GetRolesForUserAsync(string username)
        {
            var roles = new List<string>();

            var query = @"select R.RoleName
                        FROM [HRIS].[dbo].[TRN_EmployeePersonalDetails] Emp
                        join  [PRASARNET].[dbo].[PN25_Mst_EmployeeRoles] ER on Emp.EmployeeId = ER.EmployeeId
                        join [PRASARNET].[dbo].[PN25_Mst_Role] R on ER.RoleId = R.RoleId
                        WHERE Emp.IT_NICEmail = @username;";

            using var conn = GetConnection();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserName", username);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                roles.Add(reader.GetString(0));

            return roles;
        }

        public async Task<List<string>> GetPermissionsForUserAsync(string username)
        {
            var permissions = new List<string>();
            string query = @"
                            SELECT DISTINCT p.PermissionName
                            FROM HRIS.dbo.Trn_EmployeePersonalDetails e
                            JOIN PN25_Mst_EmployeeRoles er ON e.EmployeeId = er.EmployeeId
                            JOIN PN25_Mst_Role r ON er.RoleId = r.RoleID
                            JOIN PN25_Mst_RolePermission rp ON r.RoleID = rp.RoleID
                            JOIN PN25_Trn_Permissions p ON rp.PermissionId = p.PermissionId
                            WHERE e.IT_NICEmail = @username;";

            using var conn = GetConnection();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserName", username);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                permissions.Add(reader.GetString(0));

            return permissions;
        }

    }

}

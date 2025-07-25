using Microsoft.Data.SqlClient;
using PRASARNET.Areas.MasterMng.Models;
using PRASARNET.Models;
using PRASARNET.Repositories.Interfaces;
using System.Data;

namespace PRASARNET.Repositories
{
    public class UserDataRepository : IUserDataRepository
    {
        private readonly string _connectionString;
        private readonly string _hrisConnectionString;
        public UserDataRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _hrisConnectionString = configuration.GetConnectionString("HRISConnection");
        }

        public async Task<EmployeePersonalDetails?> FetchEmployeeByEmailAsync(string emailId)
        {
            EmployeePersonalDetails? employee = null;

            using (SqlConnection con = new SqlConnection(_hrisConnectionString))
            {
                string query = @"SELECT * FROM Trn_EmployeePersonalDetails WHERE IT_NICEmail = @EMailId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EMailId", emailId);

                    await con.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            employee = new EmployeePersonalDetails
                            {
                                EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                                EmployeeCode = reader["EmployeeCode"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Designation = reader["Designation"].ToString(),
                                MobileNo = reader["MobileNo"].ToString(),
                                EMailId = reader["EMailId"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                DateOfBirthChrist = reader["DateOfBirthChrist"] as DateTime?,
                                StationId = reader["StationId"] as int?,
                                IsActive = reader["IsActive"] as bool?,
                                IT_NICEmail = reader["IT_NICEmail"].ToString()
                                // ➕ Add other fields as needed
                            };
                        }
                    }
                }
            }

            return employee;
        }

        public async Task<List<MenuItem>> FetchUserMenuByEmployeeIdAsync(int employeeId)
        {
            var menuItems = new List<MenuItem>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PN25_FetchUserMenuByEmployeeId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    cmd.Parameters.AddWithValue("@flag", 1);
                    cmd.Parameters.AddWithValue("@NIC_Email", string.Empty);

                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            menuItems.Add(new MenuItem
                            {
                                ParentPermissionId = reader.GetInt32(0),
                                ParentPermissionName = reader.GetString(1),
                                PermissionId = reader.GetInt32(2),
                                PermissionName = reader.GetString(3),
                                EmployeeId = reader.GetInt32(4),
                                ActionMethod = reader.IsDBNull(5) ? "PageNotFound" : reader.GetString(5),
                                ControllerClass = reader.IsDBNull(6) ? "Account" : reader.GetString(6),
                                Area = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            });
                        }
                    }
                }
            }

            return menuItems;
        }

        public async Task<List<string>> GetPermissionsForUserAsync(string emailId)
        {
            var permissions = new List<string>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("PN25_FetchUserMenuByEmployeeId", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmployeeId", DBNull.Value); // Not needed for flag 2
                cmd.Parameters.AddWithValue("@flag", 2); // Permissions
                cmd.Parameters.AddWithValue("@NIC_Email", emailId);

                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        permissions.Add(reader.GetString(0)); // Only PermissionName returned
                    }
                }
            }

            return permissions;
        }

        public async Task<List<string>> GetRolesForUserAsync(string emailId)
        {
            var roles = new List<string>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("PN25_FetchUserMenuByEmployeeId", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmployeeId", DBNull.Value); // Not needed for flag 3
                cmd.Parameters.AddWithValue("@flag", 3); // Roles
                cmd.Parameters.AddWithValue("@NIC_Email", emailId);

                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        roles.Add(reader.GetString(0)); // Only RoleName returned
                    }
                }
            }

            return roles;
        }

    }

}

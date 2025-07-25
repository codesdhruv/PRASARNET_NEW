using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PRASARNET.Areas.MasterMng.Models;
using PRASARNET.Repositories.Interfaces;

namespace PRASARNET.Repositories
{
    public class EmployeeRepository
    {
        private readonly string _connectionString;
        private readonly string _hrisConnectionString;
        public EmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _hrisConnectionString = configuration.GetConnectionString("HRISConnection");
        }
        public List<Roles> GetAllRoles()
        {
            List<Roles> roles = new();

            using SqlConnection conn = new(_connectionString);
            conn.Open();

            string query = "SELECT RoleID, RoleName FROM PN25_Mst_Role WHERE IsActive = 1";

            SqlCommand cmd = new(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                roles.Add(new Roles
                {
                    RoleID = Convert.ToInt32(reader["RoleID"]),
                    RoleName = reader["RoleName"].ToString()
                });
            }

            return roles;
        }
        public async Task<EmployeePersonalDetails?> GetEmployeeByCode(string employeeCode)
        {
            EmployeePersonalDetails? employee = null;

            using (SqlConnection con = new SqlConnection(_hrisConnectionString))
            {
                string query = @"SELECT * FROM Trn_EmployeePersonalDetails WHERE EmployeeCode = @EmployeeCode";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);

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
        public List<Roles> GetRolesByEmpId(int employeeId)
        {
            List<Roles> roles = new();

            using SqlConnection conn = new(_connectionString);
            conn.Open();

            string query = @"
            SELECT r.RoleID, r.RoleName
            FROM PN25_Mst_EmployeeRoles er
            INNER JOIN PN25_Mst_Role r ON er.RoleId = r.RoleID
            WHERE er.EmployeeId = @EmployeeId AND r.IsActive = 1";

            SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                roles.Add(new Roles
                {
                    RoleID = Convert.ToInt32(reader["RoleID"]),
                    RoleName = reader["RoleName"].ToString()
                });
            }

            return roles;
        }
        public void UpdateEmployeeRoles(int employeeId, List<int> roleIds)
        {
            using SqlConnection conn = new(_connectionString);
            conn.Open();

            // Remove old roles

            SqlCommand deleteCmd = new("DELETE FROM PN25_Mst_EmployeeRoles WHERE EmployeeId = @EmployeeId", conn);
            deleteCmd.Parameters.AddWithValue("@EmployeeId", employeeId);
            deleteCmd.ExecuteNonQuery();

            // Insert new roles
            foreach (int roleId in roleIds)
            {
                SqlCommand insertCmd = new("INSERT INTO PN25_Mst_EmployeeRoles (EmployeeId, RoleId) VALUES (@EmployeeId, @RoleId)", conn);
                insertCmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                insertCmd.Parameters.AddWithValue("@RoleId", roleId);
                insertCmd.ExecuteNonQuery();
            }
        }

    }
}

using Microsoft.Data.SqlClient;
using PRASARNET.Areas.MasterMng.Models;
using PRASARNET.Models;


namespace PRASARNET.Areas.MasterMng.Data
{
    public class OrganizationRepository
    {
        public readonly string _connectionString;
        public OrganizationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("HRISConnection");
        }
        public IEnumerable<OrganizationModel> GetAll()
        {
            var list = new List<OrganizationModel>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM LKP_Organization";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new OrganizationModel
                    {
                        OrganizationId = Convert.ToInt32(rdr["OrganizationId"]),
                        OrganizationName = rdr["OrganizationName"].ToString(),
                        Abbreviation = rdr["Abbreviation"].ToString(),
                        Remarks = rdr["Remarks"].ToString(),
                        IpAddress = rdr["IpAddress"].ToString(),
                        HostName = rdr["HostName"].ToString(),
                        CreatedBy = rdr["CreatedBy"].ToString(),
                        CreatedDate = rdr["CreatedDate"] as DateTime?,
                        UpdatedBy = rdr["UpdatedBy"].ToString(),
                        UpdatedDate = rdr["UpdatedDate"] as DateTime?,
                        ActInctBy = rdr["ActInctBy"].ToString(),
                        ActInctDate = rdr["ActInctDate"] as DateTime?,
                        IsActive = Convert.ToBoolean(rdr["IsActive"])
                    });
                }
            }
            return list;
        }

        public OrganizationModel GetById(int id)
        {
            OrganizationModel org = null;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM LKP_Organization WHERE OrganizationId = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    org = new OrganizationModel
                    {
                        OrganizationId = Convert.ToInt32(rdr["OrganizationId"]),
                        OrganizationName = rdr["OrganizationName"].ToString(),
                        Abbreviation = rdr["Abbreviation"].ToString(),
                        Remarks = rdr["Remarks"].ToString(),
                        IpAddress = rdr["IpAddress"].ToString(),
                        HostName = rdr["HostName"].ToString(),
                        CreatedBy = rdr["CreatedBy"].ToString(),
                        CreatedDate = rdr["CreatedDate"] as DateTime?,
                        UpdatedBy = rdr["UpdatedBy"].ToString(),
                        UpdatedDate = rdr["UpdatedDate"] as DateTime?,
                        ActInctBy = rdr["ActInctBy"].ToString(),
                        ActInctDate = rdr["ActInctDate"] as DateTime?,
                        IsActive = Convert.ToBoolean(rdr["IsActive"])
                    };
                }
            }
            return org;
        }

        public void Add(OrganizationModel org)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO LKP_Organization 
                             (OrganizationName, Abbreviation, Remarks, IpAddress, HostName, CreatedBy, CreatedDate, IsActive)
                             VALUES 
                             (@OrganizationName, @Abbreviation, @Remarks, @IpAddress, @HostName, @CreatedBy, GETDATE(), @IsActive)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@OrganizationName", org.OrganizationName);
                cmd.Parameters.AddWithValue("@Abbreviation", org.Abbreviation ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Remarks", org.Remarks ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IpAddress", org.IpAddress ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@HostName", org.HostName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", org.CreatedBy);
                cmd.Parameters.AddWithValue("@IsActive", org.IsActive);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(OrganizationModel org)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE LKP_Organization SET 
                             OrganizationName = @OrganizationName,
                             Abbreviation = @Abbreviation,
                             Remarks = @Remarks,
                             IpAddress = @IpAddress,
                             HostName = @HostName,
                             UpdatedBy = @UpdatedBy,
                             UpdatedDate = GETDATE(),
                             IsActive = @IsActive
                             WHERE OrganizationId = @OrganizationId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@OrganizationId", org.OrganizationId);
                cmd.Parameters.AddWithValue("@OrganizationName", org.OrganizationName);
                cmd.Parameters.AddWithValue("@Abbreviation", org.Abbreviation ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Remarks", org.Remarks ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IpAddress", org.IpAddress ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@HostName", org.HostName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UpdatedBy", org.UpdatedBy);
                cmd.Parameters.AddWithValue("@IsActive", org.IsActive);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM LKP_Organization WHERE OrganizationId = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public async Task<List<Organisation>> getAllOrganisations()
        {
            var orgList = new List<Organisation>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT OrgId, OrgName FROM Mst_Org";
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (await reader.ReadAsync())
                    {
                        orgList.Add(new Organisation
                        {
                            OrgId = Convert.ToInt32(reader["OrgId"]),
                            OrgName = reader["OrgName"].ToString(),
                        });
                    }

                }
            }
            return orgList;
        }
    }
}

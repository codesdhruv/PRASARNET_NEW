using Microsoft.Data.SqlClient;
using PRASARNET.Areas.MasterMng.Models;
using static System.Collections.Specialized.BitVector32;

namespace PRASARNET.Areas.MasterMng.Data
{
    public class StationRepository
    {
        public readonly string _connectionString;
        public StationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("HRISConnection");
        }


        public IEnumerable<Stations> GetAll()
        {
            var stations = new List<Stations>();
            using (var con = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT * FROM MST_Station", con);
                con.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    stations.Add(new Stations
                    {
                        StationID = Convert.ToInt32(reader["StationID"]),
                        StationName = reader["StationName"].ToString(),
                        OrganizationID = Convert.ToInt32(reader["OrganizationID"]),
                        // ... map other fields
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                    });
                }
            }
            return stations;
        }

        public Stations GetById(int id)
        {
            Stations station = null;
            using (var con = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT * FROM MST_Station WHERE StationID = @StationID", con);
                cmd.Parameters.AddWithValue("@StationID", id);
                con.Open();
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    station = new Stations
                    {
                        StationID = Convert.ToInt32(reader["StationID"]),
                        StationName = reader["StationName"].ToString(),
                        // ... map other fields
                    };
                }
            }
            return station;
        }

        public void Add(Stations station)
        {
            using var con = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("INSERT INTO MST_Station (StationID, StationName, OrganizationID, IsActive, CreatedBy, CreatedDate) VALUES (@StationID, @StationName, @OrganizationID, @IsActive, @CreatedBy, @CreatedDate)", con);
            cmd.Parameters.AddWithValue("@StationID", station.StationID);
            cmd.Parameters.AddWithValue("@StationName", station.StationName);
            cmd.Parameters.AddWithValue("@OrganizationID", station.OrganizationID);
            cmd.Parameters.AddWithValue("@IsActive", station.IsActive);
            cmd.Parameters.AddWithValue("@CreatedBy", station.CreatedBy);
            cmd.Parameters.AddWithValue("@CreatedDate", station.CreatedDate);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        public void Update(Stations station)
        {
            using var con = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("UPDATE MST_Station SET StationName = @StationName, OrganizationID = @OrganizationID, UpdatedBy = @UpdatedBy, UpdatedDate = @UpdatedDate WHERE StationID = @StationID", con);
            cmd.Parameters.AddWithValue("@StationID", station.StationID);
            cmd.Parameters.AddWithValue("@StationName", station.StationName);
            cmd.Parameters.AddWithValue("@OrganizationID", station.OrganizationID);
            cmd.Parameters.AddWithValue("@UpdatedBy", station.UpdatedBy);
            cmd.Parameters.AddWithValue("@UpdatedDate", station.UpdatedDate);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("DELETE FROM MST_Station WHERE StationID = @StationID", con);
            cmd.Parameters.AddWithValue("@StationID", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }








        public List<StationTypeModel> GetAllStationType()
        {
            var list = new List<StationTypeModel>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"SELECT st.*, org.OrganizationName 
                             FROM MST_StationType st
                             INNER JOIN LKP_Organization org ON st.OrganizationId = org.OrganizationId";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new StationTypeModel
                    {
                        StationTypeId = Convert.ToInt32(dr["StationTypeId"]),
                        OrganizationId = Convert.ToInt32(dr["OrganizationId"]),
                        OrganizationName = dr["OrganizationName"].ToString(),
                        StationType = dr["StationType"].ToString(),
                        Abbreviation = dr.IsDBNull(3) ? null : dr["Abbreviation"].ToString(),
                        Remarks = dr.IsDBNull(4) ? null : dr["Remarks"].ToString(),
                        IpAddress = dr.IsDBNull(5) ? null : dr["IpAddress"].ToString(),
                        HostName = dr.IsDBNull(6) ? null : dr["HostName"].ToString(),
                        CreatedBy = dr.IsDBNull(7) ? null : dr["CreatedBy"].ToString(),
                        //CreatedDate = dr.IsDBNull(8) ? null : Convert.ToDateTime(dr["CreatedDate"]),
                        CreatedDate = dr.IsDBNull(8) ? null : dr["CreatedDate"] as DateTime?,

                        UpdatedBy = dr.IsDBNull(9) ? null : dr["UpdatedBy"].ToString(),
                        UpdatedDate = dr.IsDBNull(10) ? null : dr["UpdatedDate"] as DateTime?,
                        ActInctBy = dr.IsDBNull(11) ? null : dr["ActInctBy"].ToString(),
                        ActInctDate = dr.IsDBNull(12) ? null : dr["ActInctDate"] as DateTime?,
                        IsActive = Convert.ToBoolean(dr["IsActive"])
                    });
                }
            }

            return list;
        }

        public StationTypeModel GetStationTypeById(int id)
        {
            StationTypeModel model = null;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"SELECT st.*, org.OrganizationName 
                             FROM MST_StationType st
                             INNER JOIN LKP_Organization org ON st.OrganizationId = org.OrganizationId
                             WHERE st.StationTypeId = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    model = new StationTypeModel
                    {
                        StationTypeId = Convert.ToInt32(dr["StationTypeId"]),
                        OrganizationId = Convert.ToInt32(dr["OrganizationId"]),
                        OrganizationName = dr["OrganizationName"].ToString(),
                        StationType = dr["StationType"].ToString(),
                        Abbreviation = dr["Abbreviation"].ToString(),
                        Remarks = dr["Remarks"].ToString(),
                        IpAddress = dr["IpAddress"].ToString(),
                        HostName = dr["HostName"].ToString(),
                        CreatedBy = dr["CreatedBy"].ToString(),
                        CreatedDate = Convert.ToDateTime(dr["CreatedDate"]),
                        UpdatedBy = dr["UpdatedBy"].ToString(),
                        UpdatedDate = dr["UpdatedDate"] as DateTime?,
                        ActInctBy = dr["ActInctBy"].ToString(),
                        ActInctDate = dr["ActInctDate"] as DateTime?,
                        IsActive = Convert.ToBoolean(dr["IsActive"])
                    };
                }
            }

            return model;
        }

        public void AddStationType(StationTypeModel model)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO MST_StationType
                            (OrganizationId, StationType, Abbreviation, Remarks, IpAddress, HostName, CreatedBy, CreatedDate, IsActive)
                             VALUES
                            (@OrganizationId, @StationType, @Abbreviation, @Remarks, @IpAddress, @HostName, @CreatedBy, @CreatedDate, @IsActive)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@OrganizationId", model.OrganizationId);
                cmd.Parameters.AddWithValue("@StationType", model.StationType);
                cmd.Parameters.AddWithValue("@Abbreviation", model.Abbreviation ?? "");
                cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? "");
                cmd.Parameters.AddWithValue("@IpAddress", model.IpAddress ?? "");
                cmd.Parameters.AddWithValue("@HostName", model.HostName ?? "");
                cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy ?? "system");
                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@IsActive", model.IsActive);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateStationType(StationTypeModel model)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE MST_StationType SET
                             OrganizationId = @OrganizationId,
                             StationType = @StationType,
                             Abbreviation = @Abbreviation,
                             Remarks = @Remarks,
                             IpAddress = @IpAddress,
                             HostName = @HostName,
                             UpdatedBy = @UpdatedBy,
                             UpdatedDate = @UpdatedDate,
                             IsActive = @IsActive
                             WHERE StationTypeId = @StationTypeId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@StationTypeId", model.StationTypeId);
                cmd.Parameters.AddWithValue("@OrganizationId", model.OrganizationId);
                cmd.Parameters.AddWithValue("@StationType", model.StationType);
                cmd.Parameters.AddWithValue("@Abbreviation", model.Abbreviation ?? "");
                cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? "");
                cmd.Parameters.AddWithValue("@IpAddress", model.IpAddress ?? "");
                cmd.Parameters.AddWithValue("@HostName", model.HostName ?? "");
                cmd.Parameters.AddWithValue("@UpdatedBy", model.UpdatedBy ?? "system");
                cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@IsActive", model.IsActive);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteStationType(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM MST_StationType WHERE StationTypeId = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }                         


    }
}

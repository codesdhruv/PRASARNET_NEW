using Microsoft.Data.SqlClient;
using PRASARNET.Areas.MasterMng.Models;
using System.Data;
public class StateRepository
{
    private readonly string _connectionString;

    public StateRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public List<State> GetAllStates()
    {
        List<State> states = new List<State>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            string sql = "SELECT * FROM PN25_Mst_State";
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                states.Add(new State
                {
                    StateID = Convert.ToInt32(reader["StateID"]),
                    LGD_Code = Convert.ToInt32(reader["LGD_Code"]),
                    StateName = reader["StateName"].ToString(),
                    IsState = reader["IsState"].ToString(),
                    ZoneID = reader["ZoneID"].ToString(),
                    CencusCode_2001 = Convert.ToInt32(reader["CencusCode_2001"]),
                    CencusCode_2011 = Convert.ToInt32(reader["CencusCode_2011"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    CreatedBy = reader["CreatedBy"]?.ToString(),
                    CreatedAt = reader["CreatedAt"] as DateTime?,
                    UpdatedBy = reader["UpdatedBy"]?.ToString(),
                    UpdatedAt = reader["UpdatedAt"] as DateTime?
                });
            }
        }

        return states;
    }

    public void AddState(State state)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            string sql = @"INSERT INTO PN25_Mst_State 
                (StateID, LGD_Code, StateName, IsState, ZoneID, CencusCode_2001, CencusCode_2011, IsActive, CreatedBy, CreatedAt) 
                VALUES 
                (@StateID, @LGD_Code, @StateName, @IsState, @ZoneID, @CencusCode_2001, @CencusCode_2011, @IsActive, @CreatedBy, @CreatedAt)";

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@StateID", state.StateID);
            cmd.Parameters.AddWithValue("@LGD_Code", state.LGD_Code);
            cmd.Parameters.AddWithValue("@StateName", state.StateName);
            cmd.Parameters.AddWithValue("@IsState", state.IsState);
            cmd.Parameters.AddWithValue("@ZoneID", state.ZoneID);
            cmd.Parameters.AddWithValue("@CencusCode_2001", state.CencusCode_2001);
            cmd.Parameters.AddWithValue("@CencusCode_2011", state.CencusCode_2011);
            cmd.Parameters.AddWithValue("@IsActive", state.IsActive);
            cmd.Parameters.AddWithValue("@CreatedBy", state.CreatedBy ?? "admin");
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public void UpdateStateold(State state)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            string sql = @"UPDATE PN25_Mst_State SET 
                            LGD_Code = @LGD_Code,
                            StateName = @StateName,
                            IsState = @IsState,
                            ZoneID = @ZoneID,
                            CencusCode_2001 = @CencusCode_2001,
                            CencusCode_2011 = @CencusCode_2011,
                            IsActive = @IsActive,
                            UpdatedBy = @UpdatedBy,
                            UpdatedAt = @UpdatedAt
                        WHERE StateID = @StateID";

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@StateID", state.StateID);
            cmd.Parameters.AddWithValue("@LGD_Code", state.LGD_Code);
            cmd.Parameters.AddWithValue("@StateName", state.StateName);
            cmd.Parameters.AddWithValue("@IsState", state.IsState);
            cmd.Parameters.AddWithValue("@ZoneID", state.ZoneID);
            cmd.Parameters.AddWithValue("@CencusCode_2001", state.CencusCode_2001);
            cmd.Parameters.AddWithValue("@CencusCode_2011", state.CencusCode_2011);
            cmd.Parameters.AddWithValue("@IsActive", state.IsActive);
            cmd.Parameters.AddWithValue("@UpdatedBy", state.UpdatedBy ?? "admin");
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
    public void UpdateState(State state)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE PN25_Mst_State SET 
                            LGD_Code = @LGD_Code,
                            StateName = @StateName,
                            IsState = @IsState,
                            ZoneID = @ZoneID,
                            CencusCode_2001 = @CencusCode_2001,
                            CencusCode_2011 = @CencusCode_2011,
                            IsActive = @IsActive,
                            UpdatedBy = @UpdatedBy,
                            UpdatedAt = @UpdatedAt
                        WHERE StateID = @StateID";

                SqlCommand cmd = new SqlCommand(sql, conn);

                // Use Add method with explicit types for safety, especially for nullable types
                cmd.Parameters.Add("@StateID", SqlDbType.Int).Value = state.StateID;
                cmd.Parameters.AddWithValue("@LGD_Code", state.LGD_Code);
                cmd.Parameters.AddWithValue("@StateName", state.StateName);
                cmd.Parameters.Add("@IsState", SqlDbType.Bit).Value = state.IsState;
                cmd.Parameters.AddWithValue("@ZoneID", state.ZoneID);
                cmd.Parameters.AddWithValue("@CencusCode_2001", state.CencusCode_2001);
                cmd.Parameters.AddWithValue("@CencusCode_2011", state.CencusCode_2011);
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = state.IsActive;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.VarChar, 50).Value = state.UpdatedBy ?? "admin"; // Default "admin" if null
                cmd.Parameters.Add("@UpdatedAt", SqlDbType.DateTime).Value = DateTime.Now;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            // Log the error (you can replace this with a proper logging mechanism)
            Console.WriteLine($"Error updating state: {ex.Message}");
            // Optionally, rethrow the exception if you want to propagate it
            throw;
        }
    }

    public void DeleteState(int stateId)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            string sql = "DELETE FROM PN25_Mst_State WHERE StateID = @StateID";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@StateID", stateId);

            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public State GetStateById(int id)
    {
        State state = null;

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            string sql = "SELECT * FROM PN25_Mst_State WHERE StateID = @StateID";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@StateID", id);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                state = new State
                {
                    StateID = Convert.ToInt32(reader["StateID"]),
                    LGD_Code = Convert.ToInt32(reader["LGD_Code"]),
                    StateName = reader["StateName"].ToString(),
                    IsState = reader["IsState"].ToString(),
                    ZoneID = reader["ZoneID"].ToString(),
                    CencusCode_2001 = Convert.ToInt32(reader["CencusCode_2001"]),
                    CencusCode_2011 = Convert.ToInt32(reader["CencusCode_2011"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    CreatedBy = reader["CreatedBy"]?.ToString(),
                    CreatedAt = reader["CreatedAt"] as DateTime?,
                    UpdatedBy = reader["UpdatedBy"]?.ToString(),
                    UpdatedAt = reader["UpdatedAt"] as DateTime?
                };
            }
        }

        return state;
    }
}

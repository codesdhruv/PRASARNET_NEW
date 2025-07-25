using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace PRASARNET.Utility
{
    public class DbHelper
    {
        private readonly string _connectionString;
        public DbHelper( IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        private DataTable ExecuteProcedure(string procName, Dictionary<string, object> parameters)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(procName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }

            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        // 🔹 Get Organization List
        public DataTable GetOrganizations()
        {
            return ExecuteProcedure("PN21_Trans_TPosting", new()
            {
                { "@flag", 3 }
            });
        }

        // 🔹 Get Designations by Wing ID
        public DataTable GetDesignationsByWing(int wingId)
        {
            return ExecuteProcedure("PN21_Trans_TPosting", new()
            {
                { "@flag", 2 },
                { "@wingid", wingId }
            });
        }

        // 🔹 Get Transfer Grid Data by Employee ID
        public DataTable GetTransferGrid(int employeeId)
        {
            return ExecuteProcedure("PN21_Trans_TPosting", new()
            {
                { "@flag", 5 },
                { "@employeeid", employeeId }
            });
        }
    }
}

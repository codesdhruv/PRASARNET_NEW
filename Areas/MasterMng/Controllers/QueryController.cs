using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using PRASARNET.Areas.MasterMng.Models;
using System.Text.RegularExpressions;

namespace PRASARNET.Areas.MasterMng.Controllers
{
    [Area("MasterMng")]
    public class QueryController : Controller
    {
        private readonly string _connectionString = "Data Source=172.16.14.251;Initial Catalog=PRASARNET;Integrated Security=False;User ID=sa;Password=changeme;Encrypt=False;";

        [HttpGet]
        public IActionResult Index()
        {
            return View(new QueryModel());
        }

        [HttpPost]
        public IActionResult Index(QueryModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SqlQuery) || !model.SqlQuery.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", "Only SELECT queries are allowed.");
                return View(model);
            }
            if (!IsSafeSelectQuery(model.SqlQuery))
            {
                ModelState.AddModelError("", "Only safe SELECT queries are allowed.");
                return View(model);
            }

            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(model.SqlQuery, conn);
                using var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);
                model.Result = dt;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error executing query: " + ex.Message);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult DownloadCsv(string sqlQuery)
        {
            if (string.IsNullOrWhiteSpace(sqlQuery) || !sqlQuery.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only SELECT queries are allowed.");
            }

            var dt = new DataTable();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sqlQuery, conn);
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);

            var sb = new StringBuilder();
            for (int i = 0; i < dt.Columns.Count; i++)
                sb.Append(dt.Columns[i].ColumnName + (i < dt.Columns.Count - 1 ? "," : ""));

            sb.AppendLine();

            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                    sb.Append(row[i].ToString()?.Replace(",", " ") + (i < dt.Columns.Count - 1 ? "," : ""));

                sb.AppendLine();
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "QueryResults.csv");
        }

        private bool IsSafeSelectQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return false;

            query = query.Trim().ToUpperInvariant();

            // Must start with SELECT or WITH (for CTEs)
            if (!query.StartsWith("SELECT") && !query.StartsWith("WITH"))
                return false;

            // Disallow multiple statements
            if (query.Contains(";"))
                return false;

            // Disallow dangerous SQL keywords
            string[] forbidden = { "INSERT", "UPDATE", "DELETE", "DROP", "ALTER", "EXEC", "MERGE", "TRUNCATE" };
            foreach (var word in forbidden)
            {
                if (Regex.IsMatch(query, $@"\b{word}\b", RegexOptions.IgnoreCase))
                    return false;
            }

            return true;
        }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using PRASARNET.Areas.PrasarNet.Models;
using System.Data;
using System.Data.SqlClient;

namespace PRASARNET.Areas.PrasarNet.Services
{
    public class CircularService
    {
        private readonly string _prasarNetConn;
        private readonly string _hrisConn;


        public CircularService(IConfiguration configuration)
        {
            _prasarNetConn = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Circular> GetCirculars()
        {
            var list = new List<Circular>();

            try
            {

                using (var conn = new SqlConnection(_prasarNetConn))
                using (var cmd = new SqlCommand("PN18_documentsupload", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@flag", 7);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Circular
                            {
                                UploaderName = reader["UploaderName"].ToString(),
                                Doc_Title = reader["Doc_Title"].ToString(),
                                Createdon = reader["Createdon"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["Createdon"]),
                                Doc_Type = reader["Doc_Type"].ToString(),
                                Doc_OrderNo = reader["Doc_OrderNo"].ToString(),
                                Doc_Dated = reader["Doc_Dated"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["Doc_Dated"]),
                                Doc_Typeid = reader["Doc_Typeid"].ToString(),
                                Doc_Name = reader["Doc_Name"].ToString(),
                                folderName = reader["folderName"].ToString(),
                                Docid = reader["Docid"].ToString()

                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // You can log this using ILogger or any logging framework
                throw new Exception("SQL error while fetching circulars.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching circulars.", ex);
            }

            return list;
        }

        public List<Circular> GetAllCirculars()
        {
            var list = new List<Circular>();
            try
            {
                using (var conn = new SqlConnection(_prasarNetConn))
                using (var cmd = new SqlCommand("PN18_documentsupload", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@flag", 6);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Circular
                            {
                                UploaderName = reader["UploaderName"].ToString(),
                                Doc_Title = reader["Doc_Title"].ToString(),
                                Createdon = reader["Createdon"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["Createdon"]),
                                Doc_Type = reader["Doc_Type"].ToString(),
                                Doc_OrderNo = reader["Doc_OrderNo"].ToString(),
                                Doc_Dated = reader["Doc_Dated"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["Doc_Dated"]),
                                Doc_Typeid = reader["Doc_Typeid"].ToString(),
                                Doc_Name = reader["Doc_Name"].ToString(),
                                folderName = reader["folderName"].ToString(),
                                Docid = reader["Docid"].ToString()
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // You can log this using ILogger or any logging framework
                throw new Exception("SQL error while fetching circulars.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching circulars.", ex);
            }

            return list;
        }

        public List<Circular> GetPaginatedCirculars(int start, int length, string searchValue, out int totalRecords, string category)
        {
            var list = new List<Circular>();
            totalRecords = 0;

            using (var conn = new SqlConnection(_prasarNetConn))
            using (var cmd = new SqlCommand("PN25_documentsupload", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@flag", 6);
                cmd.Parameters.AddWithValue("@Start", start);
                cmd.Parameters.AddWithValue("@Length", length);
                cmd.Parameters.AddWithValue("@SearchValue", searchValue ?? "");
                cmd.Parameters.AddWithValue("@SortColumn", "Createdon");
                cmd.Parameters.AddWithValue("@SortDirection", "desc");
                cmd.Parameters.AddWithValue("@Category", category ?? ""); // pass category if used in filtering

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    // First result set: data
                    while (reader.Read())
                    {
                        list.Add(new Circular
                        {
                            UploaderName = reader["UploaderName"]?.ToString(),
                            Doc_Title = reader["Doc_Title"]?.ToString(),
                            Createdon = reader["Createdon"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["Createdon"]),
                            Doc_Type = reader["Doc_Type"]?.ToString(),
                            Doc_OrderNo = reader["Doc_OrderNo"]?.ToString(),
                            Doc_Dated = reader["Doc_Dated"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["Doc_Dated"]),
                            Doc_Typeid = reader["Doc_Typeid"]?.ToString(),
                            Doc_Name = reader["Doc_Name"]?.ToString(),
                            folderName = reader["folderName"]?.ToString(),
                            Docid = reader["Docid"]?.ToString()
                        });
                    }

                    // Second result set: total count
                    if (reader.NextResult() && reader.Read())
                    {
                        totalRecords = Convert.ToInt32(reader["TotalCount"]);
                    }
                }
            }

            return list;
        }


        /* Employee corder */

        public List<Circular> GetEmployeeCorner()
        {
            var list = new List<Circular>();
            try
            {
                using (var conn = new SqlConnection(_prasarNetConn))
                using (var cmd = new SqlCommand("PN18_documentsupload", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@flag", 13);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Circular
                            {
                                UploaderName = reader["UploaderName"].ToString(),
                                Doc_Title = reader["Doc_Title"].ToString(),
                                Createdon = reader["Createdon"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["Createdon"]),
                                Doc_Type = reader["Doc_Type"].ToString(),
                                Doc_OrderNo = reader["Doc_OrderNo"].ToString(),
                                Doc_Dated = reader["Doc_Dated"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["Doc_Dated"]),
                                Doc_Typeid = reader["Doc_Typeid"].ToString(),
                                Doc_Name = reader["Doc_Name"].ToString(),
                                folderName = reader["folderName"].ToString(),
                                Docid = reader["Docid"].ToString()
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // You can log this using ILogger or any logging framework
                throw new Exception("SQL error while fetching circulars.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching circulars.", ex);
            }

            return list;
        }

        public List<SelectListItem> GetDocumentCategories()
        {
            List<SelectListItem> categories = new List<SelectListItem>();
            categories.Add(new SelectListItem { Text = "--All--", Value = "" });

            using (SqlConnection conn = new SqlConnection(_prasarNetConn))
            {
                using (SqlCommand cmd = new SqlCommand("PN18_DocTypen", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@flag", 1);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new SelectListItem
                            {
                                Text = reader["Doc_Type"].ToString(),
                                Value = reader["Docid"].ToString()
                            });
                        }
                    }
                }
            }

            return categories;
        }
    }
}

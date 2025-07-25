using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using PRASARNET.Areas.ManageDocument.Models;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace PRASARNET.Areas.ManageDocument.Data
{
    public class DocumentRepository
    {
        public readonly string _connectionString;
          
        public DocumentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<SelectListItem> GetUploadTypes()
        {
            var list = new List<SelectListItem>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("PN18_DocTypen", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@flag", SqlDbType.Int).Value = 1;

                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new SelectListItem
                                {
                                    Value = reader["DocID"]?.ToString(),
                                    Text = reader["Doc_Type"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching document types", ex);
            }


            return list;
        }




        //public List<DocumentListModel> GetDocumentsByUserId()
        //{
        //    var documents = new List<DocumentListModel>();

        //    using (SqlConnection conn = new SqlConnection(_connectionString))
        //    using (SqlCommand cmd = new SqlCommand("PN18_documentsupload", conn))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        // Accessing Session variables (classic ASP.NET)
        //        int usertype = Convert.ToInt32(HttpContext.Current.Session["Usertype"]);
        //        cmd.Parameters.AddWithValue("@usertypeid", usertype);
        //        cmd.Parameters.AddWithValue("@flag", 2);

        //        // Add usertype-based parameters
        //        if (usertype == 11 || usertype == 20 || usertype == 22 || usertype == 23 || usertype == 24)
        //        {
        //            cmd.Parameters.AddWithValue("@stationid", Convert.ToInt32(HttpContext.Current.Session["Station_SectionID"]));
        //        }
        //        else if (usertype == 12 || usertype == 25 || usertype == 26)
        //        {
        //            cmd.Parameters.AddWithValue("@sectionid", Convert.ToInt32(HttpContext.Current.Session["Station_SectionID"]));
        //        }

        //        conn.Open();

        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                var doc = new DocumentListModel
        //                {
        //                    DocId = Convert.ToInt32(reader["Docid"]),
        //                    Doc_Title = reader["Doc_Title"].ToString(),
        //                    CreatedOn = reader["Createdon"] != DBNull.Value ? Convert.ToDateTime(reader["Createdon"]) : (DateTime?)null,
        //                    Doc_Type = reader["Doc_Type"].ToString(),
        //                    Doc_Typeid = reader["Doc_Typeid"] != DBNull.Value ? Convert.ToInt32(reader["Doc_Typeid"]) : 0,
        //                    Doc_OrderNo = reader["Doc_OrderNo"].ToString(),
        //                    Doc_Dated = reader["Doc_Dated"] != DBNull.Value ? Convert.ToDateTime(reader["Doc_Dated"]) : (DateTime?)null
        //                    // Add more properties as needed
        //                };

        //                documents.Add(doc);
        //            }
        //        }
        //    }

        //    return documents;
        //}

    }
}

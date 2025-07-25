using Microsoft.Data.SqlClient;
using PRASARNET.Areas.TRAM.Models;
using System.Data;


namespace PRASARNET.Areas.TRAM.Repositories
{
    public class HandelApplicationRepository
    {
        private readonly string _connectionString;

        public HandelApplicationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<HandelApplicationViewModel>> GetPendingApplicationsAsync(int dealingUserTypeId, int dealingEmpId)
        {
            var applications = new List<HandelApplicationViewModel>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("PN21_trans_Action", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@flag", 1);
                    cmd.Parameters.AddWithValue("@dealingUsertypeid", dealingUserTypeId);
                    cmd.Parameters.AddWithValue("@dealingOffc", dealingEmpId);

                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            applications.Add(new HandelApplicationViewModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Dealing_StnSec_empId = reader.GetInt32(reader.GetOrdinal("Dealing_StnSec_empId")),
                                DealingUsertypeId = reader.GetInt32(reader.GetOrdinal("DealingUsertypeId")),
                                Name_emp = reader["Name_emp"].ToString(),
                                ApplicationNo = reader["ApplicationNo"].ToString(),
                                Emp_Designation = reader["Emp_Designation"].ToString(),
                                Emp_Present_Place = reader["emp_present_place"].ToString(),
                                Emp_ContactNo = reader["emp_contactno"].ToString(),
                                Emp_Email = reader["emp_Email"].ToString(),
                                ForwardedTo = reader["forwardedto"].ToString(),
                                Action_Taken = reader["Action_taken"].ToString(),
                                FileUpload = reader["fileupload"].ToString(),
                                FileUploadId = reader["fileuploadid"].ToString(),
                                PrimaryEmail = reader["primaryemail"].ToString(),
                                CcEmail = reader["ccemail"].ToString(),
                                FinalStatus = reader["FinalStatus"].ToString(),
                                CurrentStatusId = reader["currenstatusid"] != DBNull.Value ? Convert.ToInt32(reader["currenstatusid"]) : 0,
                                CurrentStatus = reader["currentstatus"].ToString(),
                                FinalStatusId = reader["finalstatusid"] != DBNull.Value ? Convert.ToInt32(reader["finalstatusid"]) : 0,
                                BasicId = reader["basicid"] != DBNull.Value ? Convert.ToInt32(reader["basicid"]) : 0,
                                SubmittedOn = reader["submittedon"].ToString(),
                                SerialNo = reader["serialno"].ToString(),
                                IsApproved = reader["isapproved"] != DBNull.Value && Convert.ToBoolean(reader["isapproved"])
                            });
                        }
                    }
                }

                foreach (var app in applications)
                {
                    var approval = await GetCertificationStatusAsync(app.BasicId);
                    app.IsApproved = approval.IsApproved;
                    app.ApprovedBy = approval.ApprovedBy;
                    app.CertifyRemarks = approval.Remarks;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle as needed
                throw new ApplicationException("Error loading pending applications", ex);
            }

            return applications;
        }

        public async Task<List<HandelApplicationViewModel>> GetPendingApplicationsAsync0(int dealingUserTypeId, int dealingEmpId)
        {
            var applications = new List<HandelApplicationViewModel>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("PN21_trans_Action", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@flag", 1);
                cmd.Parameters.AddWithValue("@dealingUsertypeid", dealingUserTypeId);
                cmd.Parameters.AddWithValue("@dealingOffc", dealingEmpId);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        applications.Add(new HandelApplicationViewModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Dealing_StnSec_empId = reader.GetInt32(reader.GetOrdinal("Dealing_StnSec_empId")),
                            DealingUsertypeId = reader.GetInt32(reader.GetOrdinal("DealingUsertypeId")),
                            Name_emp = reader["Name_emp"].ToString(),
                            ApplicationNo = reader["ApplicationNo"].ToString(),
                            Emp_Designation = reader["Emp_Designation"].ToString(),
                            Emp_Present_Place = reader["emp_present_place"].ToString(),
                            Emp_ContactNo = reader["emp_contactno"].ToString(),
                            Emp_Email = reader["emp_Email"].ToString(),
                            ForwardedTo = reader["forwardedto"].ToString(),
                            Action_Taken = reader["Action_taken"].ToString(),
                            FileUpload = reader["fileupload"].ToString(),
                            FileUploadId = reader["fileuploadid"].ToString(),
                            PrimaryEmail = reader["primaryemail"].ToString(),
                            CcEmail = reader["ccemail"].ToString(),
                            FinalStatus = reader["FinalStatus"].ToString(),
                            CurrentStatusId = Convert.ToInt32(reader["currenstatusid"]),
                            CurrentStatus = reader["currentstatus"].ToString(),
                            FinalStatusId = Convert.ToInt32(reader["finalstatusid"]),
                            BasicId = Convert.ToInt32(reader["basicid"]),
                            SubmittedOn = reader["submittedon"].ToString(),
                            SerialNo = reader["serialno"].ToString(),
                            IsApproved = Convert.ToBoolean(reader["isapproved"])
                        });
                    }
                }
            }

            foreach (var app in applications)
            {
                var approval = await GetCertificationStatusAsync(app.BasicId);
                app.IsApproved = approval.IsApproved;
                app.ApprovedBy = approval.ApprovedBy;
                app.CertifyRemarks = approval.Remarks;
            }

            return applications;
        }

        public async Task<List<TrackApplicationViewModel>> GetApplicationTrackAsync(int basicId)
        {
            var list = new List<TrackApplicationViewModel>();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("PN21_Trans_Track", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@flag", 1);
                cmd.Parameters.AddWithValue("@basid", basicId);
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new TrackApplicationViewModel
                        {
                            TrnId = Convert.ToInt32(reader["trnid"]),
                            AppId = Convert.ToInt32(reader["appId"]),
                            Status = reader["Status"].ToString(),
                            DealingOffc = reader["DealingOffc"].ToString(),
                            AppSubmitOn = reader["AppSubmitOn"].ToString(),
                            Fwd_clsd_Remarks = reader["Fwd_clsd_Remarks"].ToString(),
                            Fwd_clsd_File = reader["Fwd_clsd_File"].ToString(),
                            FileName = reader["FileName"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public async Task<bool> CertifyApplicationAsync(int applicationId, int empId, string ip, string remarks)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PN21_trans_Action", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@approvedbyid", empId);
                    cmd.Parameters.AddWithValue("@Ipfromactiontaken", ip);
                    cmd.Parameters.AddWithValue("@approvedbyUsrTypID", 10);
                    cmd.Parameters.AddWithValue("@HOO_rmk_certify", remarks);
                    cmd.Parameters.AddWithValue("@flag", 7);
                    cmd.Parameters.AddWithValue("@appId", applicationId);

                    await con.OpenAsync();
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<CertificationStatusViewModel> GetCertificationStatusAsync(int applicationId)
        {
            var model = new CertificationStatusViewModel();

            using (var conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PN21_trans_Action", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@flag", 8);
                    cmd.Parameters.AddWithValue("@appid", applicationId);

                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            model.IsApproved = Convert.ToBoolean(reader.GetValue(0));
                            model.ApprovedBy = reader.IsDBNull(1) ? "--" : reader.GetString(1);
                            model.Remarks = reader.IsDBNull(2) ? "" : reader.GetString(2);
                        }
                    }
                }
            }

            return model;
        }


    }
}

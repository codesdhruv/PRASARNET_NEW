using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using PRASARNET.Areas.ManageDocument.Models;
using PRASARNET.Areas.TRAM.Models;
using System.Data;
using System.Net;

namespace PRASARNET.Areas.TRAM.Repositories
{
    public class ApplicantRepository
    {
        private readonly string _connectionString;

        public ApplicantRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<TransferApplicationViewModel> GetDraftedApplications(int employeeId)
        {
            var list = new List<TransferApplicationViewModel>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("PN21_Tran_EmpAppliaction", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@flag", 1);         // ✅ correct flag
                cmd.Parameters.AddWithValue("@empid", employeeId);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string forwardedToRaw = reader["forwardedto"]?.ToString();
                        string forwardedToCode = "";
                        string forwardedToName = "";

                        if (!string.IsNullOrWhiteSpace(forwardedToRaw))
                        {
                            int idx = forwardedToRaw.IndexOf(" (");
                            if (idx != -1)
                            {
                                forwardedToCode = forwardedToRaw.Substring(0, idx).Trim();
                                forwardedToName = forwardedToRaw.Substring(idx + 2);
                            }
                            else
                            {
                                forwardedToCode = forwardedToRaw;
                            }
                        }
                        list.Add(new TransferApplicationViewModel
                        {
                            ApplicationId = (int)reader["id"],
                            ApplicationNo = reader["ApplicationNo"]?.ToString(),
                            ForwardedToCode = forwardedToCode,
                            ForwardedToName = forwardedToName,

                            Option1 = reader["C_1"]?.ToString(),
                            Option2 = reader["C_2"]?.ToString(),
                            Option3 = reader["C_3"]?.ToString(),

                            TenureOption1 = reader["CT_1"]?.ToString(),
                            TenureOption2 = reader["CT_2"]?.ToString(),
                            TenureOption3 = reader["CT_3"]?.ToString(),

                            IsFinalSubmit = Convert.ToInt32(reader["isfinalsubmit"]) == 1,
                            SubmissionStatus = reader["App_Status"]?.ToString(),
                            ProcessStatus = reader["App_process_status"]?.ToString(),

                            FinalAppSubmitOn = reader["finalAppSubmitOn"]?.ToString(),

                            LastDraftedDate = reader["maxdate"]?.ToString()
                            // Add more fields as needed
                        });
                    }
                }
            }

            return list;
        }

        public async Task<ApplicantBasicDetailViewModel> GetApplicationByIdAsync(int id)
        {
            var model = new ApplicantBasicDetailViewModel();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("PN21_Tran_EmpAppliaction", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@flag", 2);
                    cmd.Parameters.AddWithValue("@id", id);

                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        {
                            if (await reader.ReadAsync())
                            {
                                model.ApplicationId = (int)reader["id"];
                                model.ApplicationNo = reader["ApplicationNo"] != DBNull.Value ? Convert.ToInt64(reader["ApplicationNo"]) : 0;
                                model.Name_Emp = reader["Name_Emp"]?.ToString();
                                model.Language = reader["LanguageKnown"]?.ToString();
                                model.Hometown = reader["HomeTown"]?.ToString();

                                model.SelectedCity1 = reader["seekingOp1"] != DBNull.Value ? Convert.ToInt32(reader["seekingOp1"]) : null;
                                model.SelectedCity2 = reader["seekingOp2"] != DBNull.Value ? Convert.ToInt32(reader["seekingOp2"]) : null;
                                model.SelectedCity3 = reader["seekingOp3"] != DBNull.Value ? Convert.ToInt32(reader["seekingOp3"]) : null;

                                model.SelectedState1 = reader["op1stid"] != DBNull.Value ? Convert.ToInt32(reader["op1stid"]) : null;
                                model.SelectedState2 = reader["op2stid"] != DBNull.Value ? Convert.ToInt32(reader["op2stid"]) : null;
                                model.SelectedState3 = reader["op3st3"] != DBNull.Value ? Convert.ToInt32(reader["op3st3"]) : null;

                                model.TenureOption1 = reader["Ten_seekingOP1"] != DBNull.Value ? Convert.ToInt32(reader["Ten_seekingOP1"]) : null;
                                model.TenureOption2 = reader["Ten_seekingOP2"] != DBNull.Value ? Convert.ToInt32(reader["Ten_seekingOP2"]) : null;
                                model.TenureOption3 = reader["Ten_seekingOP3"] != DBNull.Value ? Convert.ToInt32(reader["Ten_seekingOP3"]) : null;

                                model.ReasonIfNone = reader["Reason_If_SelectedNone"]?.ToString();
                                model.SelectedGroundId = reader["Grnd_TransReqd"] != DBNull.Value ? Convert.ToInt32(reader["Grnd_TransReqd"]) : null;
                                model.Subject = reader["Subject_Grnd"]?.ToString();
                                model.Details = reader["Details_Grnd"]?.ToString();
                                model.MedicalFacilitiesAvailable = reader["medicalfacilitiesAvailbale"]?.ToString();
                                model.WantTransferBenefit = reader["WantTrans_Benefit"]?.ToString();
                                model.ForwardedTo = reader["forwardedto"]?.ToString();
                                model.HOO_NameId = reader["ForwardedtoID"] != DBNull.Value ? Convert.ToInt32(reader["ForwardedtoID"]) : null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception (optional)
                //_logger.LogError(ex, "Error fetching application data for ID: {ApplicationId}", id);
                throw; // or return null, or handle accordingly
            }

            return model;
        }


        public async Task<ApplicantBasicDetailViewModel?> GetEmployeeDetailsAsync(int empId)
        {
            ApplicantBasicDetailViewModel? employee = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("PN21_Trans_EmpDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@flag", 1);
                    cmd.Parameters.AddWithValue("@empid", empId);

                    await conn.OpenAsync();

                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                        {
                            employee = new ApplicantBasicDetailViewModel
                            {
                                EmployeeCode = dr["EmployeeCode"].ToString(),             // Index 1
                                Name_Emp = dr["Name_Emp"].ToString(),                     // Index 2
                                DOB = dr["DOB"].ToString(),                               // Index 3
                                DesignationName = dr["DesignationName"].ToString(),       // Index 4
                                Mobile = dr["mobile"].ToString(),                         // Index 5
                                Email = dr["email"].ToString(),                           // Index 6
                                PresentStationName = dr["Present_stationname"].ToString(), // Index 7
                                DOJ_Present = dr["DOJ_Present"].ToString(),               // Index 8
                                NoOfYearsServed = dr["NoOfYearsServed"].ToString(),       // Index 9
                                DateSuperannuation = dr["Date_superannuation"].ToString(),// Index 10
                                Tenure = dr["Tenure"].ToString(),                         // Index 11
                                TenureYear = dr["ten_years"] != DBNull.Value ? Convert.ToInt32(dr["ten_years"]) : (int?)null, // Index 12
                                DaysCompletedOnTenure = dr["DaysComp_on_difficultTenure"].ToString(), // Index 13
                                DaysExceeded = dr["Days_AfterTenureComp"].ToString(),     // Index 14
                                CategoryName = dr["CategoryName"].ToString(),             // Index 16
                                ForwardedTo = dr["HOOName"].ToString(),                   // Index 17
                                //HOO_NameId = Convert.ToInt32(dr["EmployeeId"]) // Index 18
                                HOO_NameId = dr.IsDBNull(18) ? 0 : dr.GetInt32(18)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching employee details: {ex.Message}");
                // Optionally: throw; or handle gracefully
            }

            return employee;
        }

        public async Task<List<SelectListItem>> GetStateListAsync()
        {
            var list = new List<SelectListItem>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("PN21_Trans_State_Sp", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@flag", 1);

                    await conn.OpenAsync();

                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            list.Add(new SelectListItem
                            {
                                Value = dr["stateid"].ToString(),
                                Text = dr["StateName"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error (you can use any logging mechanism here)
                Console.WriteLine($"Error fetching state list: {ex.Message}");
                // Optionally rethrow or handle gracefully
                // throw;
            }

            return list;
        }

        public List<SelectListItem> GetCities(int stateId, int? cityId1 = null, int? cityId2 = null)
        {
            var cities = new List<SelectListItem>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("PN21_Trans_City_Sp", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@flag", 1);
                cmd.Parameters.AddWithValue("@stateid", stateId);
                cmd.Parameters.AddWithValue("@cityid1", cityId1 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@cityid2", cityId2 ?? (object)DBNull.Value);

                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        cities.Add(new SelectListItem
                        {
                            Value = dr["cityid"].ToString(),
                            Text = dr["cityname"].ToString()
                        });
                    }
                }
            }

            return cities;
        }

        public List<SelectListItem> GetGroundsForTransfer()
        {
            var items = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "--Select--" }
                };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PN21_Trans_ReasonforTrans", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@flag", 1);

                    conn.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Value = rdr["id"].ToString(),
                                Text = rdr["conditions_transfer"].ToString()
                            });
                        }
                    }
                }
            }

            return items;
        }

        public async Task<List<SelectListItem>> GetGroundsForTransferAsync()
        {
            var items = new List<SelectListItem>
    {
        new SelectListItem { Value = "", Text = "--Select--" }
    };

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("PN21_Trans_ReasonforTrans", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@flag", 1);

                    await conn.OpenAsync();

                    using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            items.Add(new SelectListItem
                            {
                                Value = rdr["id"].ToString(),
                                Text = rdr["conditions_transfer"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error (use a logging framework in production)
                Console.WriteLine($"Error loading grounds for transfer: {ex.Message}");
                // Optionally: rethrow or handle gracefully
            }

            return items;
        }


        public int SaveTransferApplication(TransferApplicationModel model, string ipAddress, int empId, out int basicDetId)
        {
            int rowsAffected = 0;
            basicDetId = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("PN21_Trans_basic_sp", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Employeeid", empId);
                cmd.Parameters.AddWithValue("@LanguageKnown", model.Language);
                cmd.Parameters.AddWithValue("@HomeTown", model.Hometown);
                cmd.Parameters.AddWithValue("@FwdtoUsertypeid", 10);
                cmd.Parameters.AddWithValue("@ForwardedtoID", model.HOO_NameId);
                cmd.Parameters.AddWithValue("@seekingOp1", model.SelectedCity1);
                cmd.Parameters.AddWithValue("@seekingOp2", model.SelectedCity2 ?? 0);
                cmd.Parameters.AddWithValue("@seekingOp3", model.SelectedCity3 ?? 0);
                cmd.Parameters.AddWithValue("@Reason_If_SelectedNone", model.ReasonIfNone ?? "");
                cmd.Parameters.AddWithValue("@Ten_seekingOP1", model.TenureOption1 ?? 0);
                cmd.Parameters.AddWithValue("@Ten_seekingOP2", model.TenureOption2 ?? 0);
                cmd.Parameters.AddWithValue("@Ten_seekingOP3", model.TenureOption3 ?? 0);
                cmd.Parameters.AddWithValue("@Grnd_TransReqd", model.SelectedGroundId);
                cmd.Parameters.AddWithValue("@Subject_Grnd", model.Subject ?? "");
                cmd.Parameters.AddWithValue("@Details_Grnd", model.Details ?? "");
                cmd.Parameters.AddWithValue("@medicalfacilitiesAvailbale", model.MedicalFacilitiesAvailable ?? "");
                cmd.Parameters.AddWithValue("@WantTrans_Benefit", model.WantTransferBenefit ?? "");
                cmd.Parameters.AddWithValue("@ip", ipAddress);
                cmd.Parameters.AddWithValue("@createdby", empId);
                cmd.Parameters.AddWithValue("@flag", 1);

                var outputParam = new SqlParameter("@idout", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParam);

                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
                basicDetId = Convert.ToInt32(outputParam.Value);
            }

            return rowsAffected;
        }


        public async Task<int> SaveBasicDetailsAsync(ApplicantBasicDetailViewModel model, string ipAddress, int employeeId, int forwardedToId, int createdBy)
        {
            int basicDetailId = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("PN21_Trans_basic_sp", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@flag", 1);
                    cmd.Parameters.AddWithValue("@Employeeid", employeeId);
                    cmd.Parameters.AddWithValue("@LanguageKnown", model.Language ?? "");
                    cmd.Parameters.AddWithValue("@HomeTown", model.Hometown ?? "");
                    cmd.Parameters.AddWithValue("@FwdtoUsertypeid", 10);
                    cmd.Parameters.AddWithValue("@ForwardedtoID", forwardedToId);
                    cmd.Parameters.AddWithValue("@seekingOp1", model.SelectedCity1 ?? 0);
                    cmd.Parameters.AddWithValue("@seekingOp2", model.SelectedCity2 ?? 0);
                    cmd.Parameters.AddWithValue("@seekingOp3", model.SelectedCity3 ?? 0);
                    cmd.Parameters.AddWithValue("@Reason_If_SelectedNone", model.ReasonIfNone ?? "");
                    cmd.Parameters.AddWithValue("@Ten_seekingOP1", model.TenureOption1 ?? 0);
                    cmd.Parameters.AddWithValue("@Ten_seekingOP2", model.TenureOption2 ?? 0);
                    cmd.Parameters.AddWithValue("@Ten_seekingOP3", model.TenureOption3 ?? 0);
                    cmd.Parameters.AddWithValue("@Grnd_TransReqd", model.SelectedGroundId ?? 0);
                    cmd.Parameters.AddWithValue("@Subject_Grnd", model.Subject ?? "");
                    cmd.Parameters.AddWithValue("@Details_Grnd", model.Details ?? "");
                    cmd.Parameters.AddWithValue("@medicalfacilitiesAvailbale", model.MedicalFacilitiesAvailable ?? "");
                    cmd.Parameters.AddWithValue("@WantTrans_Benefit", model.WantTransferBenefit ?? "");
                    cmd.Parameters.AddWithValue("@ip", ipAddress ?? "");
                    cmd.Parameters.AddWithValue("@createdby", createdBy);

                    var outputParam = new SqlParameter("@idout", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    basicDetailId = Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                // Log the exception or rethrow
                // _logger.LogError(ex, "Error saving basic details");
                throw new Exception("An error occurred while saving basic details.", ex);
            }

            return basicDetailId;
        }



        public async Task<int> UpdateBasicDetailsAsync(ApplicantBasicDetailViewModel model, string ip, int empId, int forwardedToId, int createdBy)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("PN21_Trans_basic_sp", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@flag", 2); // For Update
                            cmd.Parameters.AddWithValue("@id", model.ApplicationId);
                            cmd.Parameters.AddWithValue("@Employeeid", empId);
                            cmd.Parameters.AddWithValue("@LanguageKnown", model.Language ?? "");
                            cmd.Parameters.AddWithValue("@HomeTown", model.Hometown ?? "");
                            cmd.Parameters.AddWithValue("@FwdtoUsertypeid", 10);
                            cmd.Parameters.AddWithValue("@ForwardedtoID", forwardedToId);
                            cmd.Parameters.AddWithValue("@seekingOp1", model.SelectedCity1 ?? 0);
                            cmd.Parameters.AddWithValue("@seekingOp2", model.SelectedCity2 ?? 0);
                            cmd.Parameters.AddWithValue("@seekingOp3", model.SelectedCity3 ?? 0);
                            cmd.Parameters.AddWithValue("@Reason_If_SelectedNone", model.ReasonIfNone ?? "");
                            cmd.Parameters.AddWithValue("@Ten_seekingOP1", model.TenureOption1 ?? 0);
                            cmd.Parameters.AddWithValue("@Ten_seekingOP2", model.TenureOption2 ?? 0);
                            cmd.Parameters.AddWithValue("@Ten_seekingOP3", model.TenureOption3 ?? 0);
                            cmd.Parameters.AddWithValue("@Grnd_TransReqd", model.SelectedGroundId ?? 0);
                            cmd.Parameters.AddWithValue("@Subject_Grnd", model.Subject ?? "");
                            cmd.Parameters.AddWithValue("@Details_Grnd", model.Details ?? "");
                            cmd.Parameters.AddWithValue("@medicalfacilitiesAvailbale", model.MedicalFacilitiesAvailable ?? "");
                            cmd.Parameters.AddWithValue("@WantTrans_Benefit", model.WantTransferBenefit ?? "");
                            cmd.Parameters.AddWithValue("@ip", ip ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@createdby", createdBy);
                            //cmd.Parameters.AddWithValue("@lastsaveddate_", DateTime.Now);


                        await con.OpenAsync();
                        int result = await cmd.ExecuteNonQueryAsync();

                        return model.ApplicationId;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error (optional: inject logger or write to a log file)
                Console.WriteLine($"UpdateBasicDetailsAsync Error: {ex.Message}");

                // Return -1 or throw, depending on how you want to handle errors
                return -1;
            }
        }

        public List<TrainingModel> GetTrainingByEmployeeId(int empId)
        {
            List<TrainingModel> trainings = new List<TrainingModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("PN21_Trans_Training_SP", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@flag", 2);
                cmd.Parameters.AddWithValue("@employeeid", empId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        trainings.Add(new TrainingModel
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            EmployeeId = Convert.ToInt32(reader["employeeid"]),
                            FromDate = Convert.ToDateTime(reader["Fromdate"]),
                            ToDate = Convert.ToDateTime(reader["ToDate"]),
                            CourseName = reader["CourseName"].ToString(),
                            HostInstitute = reader["Host_Institute"].ToString(),
                            AnyRelevantInfo = reader["AnyRelevantInfo"].ToString(),
                            CreatedBy = reader["createdby"].ToString(),
                            CreatedOn = Convert.ToDateTime(reader["createdon"]),
                            Ip = reader["ip"].ToString(),
                            IsActive = Convert.ToBoolean(reader["isactive"])
                        });
                    }
                }
            }

            return trainings;
        }

        public async Task<List<TrainingDetailViewModel>> GetTrainingByEmployeeIdAsync(int empId)
        {
            var trainings = new List<TrainingDetailViewModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("PN21_Trans_Training_SP", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@flag", 2);
                cmd.Parameters.AddWithValue("@employeeid", empId);

                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        trainings.Add(new TrainingDetailViewModel
                        {
                            Id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0,
                            EmployeeId = reader["employeeid"] != DBNull.Value ? Convert.ToInt32(reader["employeeid"]) : 0,
                            FromDate = reader["Fromdate"] != DBNull.Value ? Convert.ToDateTime(reader["Fromdate"]) : (DateTime?)null,
                            ToDate = reader["ToDate"] != DBNull.Value ? Convert.ToDateTime(reader["ToDate"]) : (DateTime?)null,
                            CourseName = reader["CourseName"]?.ToString(),
                            Host_Institute = reader["Host_Institute"]?.ToString(),
                            AnyRelevantInfo = reader["AnyRelevantInfo"]?.ToString(),
                            CreatedBy = reader["createdby"] != DBNull.Value ? Convert.ToInt32(reader["createdby"]) : (int?)null,
                            CreatedOn = reader["createdon"] != DBNull.Value ? Convert.ToDateTime(reader["createdon"]) : (DateTime?)null,
                            IsActive = reader["isactive"] != DBNull.Value && Convert.ToBoolean(reader["isactive"]),
                            Ip = reader["ip"]?.ToString()
                        });
                    }
                }
            }

            return trainings;
        }

        public async Task<int> AddTrainingAsync(TrainingDetailViewModel model)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("PN21_Trans_Training_SP", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@employeeid", model.EmployeeId);
                    cmd.Parameters.AddWithValue("@fromdate", model.FromDate);
                    cmd.Parameters.AddWithValue("@todate", model.ToDate);
                    cmd.Parameters.AddWithValue("@CourseName", model.CourseName);
                    cmd.Parameters.AddWithValue("@Host_Institute", model.Host_Institute);
                    cmd.Parameters.AddWithValue("@AnyRelevantInfo", model.AnyRelevantInfo ?? "");
                    cmd.Parameters.AddWithValue("@ip", model.Ip ?? "");
                    cmd.Parameters.AddWithValue("@createdby", model.CreatedBy);
                    cmd.Parameters.AddWithValue("@bid", model.BasicDetailsId);
                    cmd.Parameters.AddWithValue("@flag", 1);

                    await conn.OpenAsync();
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                // Log exception (optional): Console.WriteLine or log using a logger
                Console.WriteLine($"Error in AddTrainingAsync: {ex.Message}");
                return 0; // Or rethrow, or return a custom code as needed
            }
        }

        public async Task<int> ExecuteDeleteTrainingAsync(int id, string ip, int bid)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("PN21_Trans_Training_SP", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@ip", ip);
                        cmd.Parameters.AddWithValue("@bid", bid);
                        cmd.Parameters.AddWithValue("@flag", 3);

                        await conn.OpenAsync();
                        return await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }



        public async Task<List<DocumentsUploadViewModel>> GetUploadedDocumentsByEmployeeIdAsync(int employeeId)
        {
            var documents = new List<DocumentsUploadViewModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("PN21_Trans_Documents_Sp", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@flag", 2);
                cmd.Parameters.AddWithValue("@employeeid", employeeId);

                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        documents.Add(new DocumentsUploadViewModel
                        {
                            Id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0,
                            EmployeeId = reader["employeeid"] != DBNull.Value ? Convert.ToInt32(reader["employeeid"]) : 0,
                            DocTypeId = reader["DocTypeid"] != DBNull.Value ? Convert.ToInt32(reader["DocTypeid"]) : (int?)null,
                            DocType = reader["doctype"]?.ToString(),
                            Subject = reader["Subject"]?.ToString(),
                            Remarks = reader["Remarks"]?.ToString(),
                            Filename = reader["Filename"]?.ToString(),
                            CreatedBy = reader["createdby"] != DBNull.Value ? Convert.ToInt32(reader["createdby"]) : (int?)null,
                            CreatedOn = reader["createdon"] != DBNull.Value ? Convert.ToDateTime(reader["createdon"]) : (DateTime?)null,
                            Ip = reader["ip"]?.ToString(),
                            IsActive = reader["isactive"] != DBNull.Value && Convert.ToBoolean(reader["isactive"])
                        });
                    }
                }
            }

            return documents;
        }

        public async Task<List<HealthCategoryViewModel>> GetHealthCategoriesByEmployeeIdAsync(int employeeId)
        {
            var list = new List<HealthCategoryViewModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("PN21_Trans_THealthcategoryPWDSD", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@flag", "1");
                cmd.Parameters.AddWithValue("@employeeid", employeeId);

                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new HealthCategoryViewModel
                        {
                            Id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0,
                            EmployeeId = reader["employeeid"] != DBNull.Value ? Convert.ToInt32(reader["employeeid"]) : 0,
                            TypeOfDisability = reader["Typeof_disability"] != DBNull.Value ? Convert.ToInt32(reader["Typeof_disability"]) : (int?)null,
                            TypeofDisabilityName = reader["TypeofDisability"]?.ToString(),
                            Self_Family = reader["Self_Family"]?.ToString(),
                            Name = reader["Name"]?.ToString(),
                            Relation = reader["relation"]?.ToString(),
                            Age = reader["age"] != DBNull.Value ? Convert.ToInt32(reader["age"]) : (int?)null,
                            IsActive = reader["isactive"] != DBNull.Value && Convert.ToBoolean(reader["isactive"]),
                            Ip = reader["Ip"]?.ToString(),
                            CreatedBy = reader["createdby"] != DBNull.Value ? Convert.ToInt32(reader["createdby"]) : (int?)null,
                            CreatedOn = reader["createdon"] != DBNull.Value ? Convert.ToDateTime(reader["createdon"]) : (DateTime?)null,
                            UpdatedBy = reader["Updatedby"] != DBNull.Value ? Convert.ToInt32(reader["Updatedby"]) : (int?)null,
                            UpdatedOn = reader["Updatedon"] != DBNull.Value ? Convert.ToDateTime(reader["Updatedon"]) : (DateTime?)null,
                            DeletedBy = reader["Deletedby"] != DBNull.Value ? Convert.ToInt32(reader["Deletedby"]) : (int?)null,
                            DeletedOn = reader["deletedon"] != DBNull.Value ? Convert.ToDateTime(reader["deletedon"]) : (DateTime?)null,
                            Additional_IfAny = reader["Additional_IfAny"]?.ToString()
                        });
                    }
                }
            }

            return list;
        }

        public async Task<List<TransferHistoryViewModel>> GetPreviousTransferPostingsAsync(int employeeId)
        {
            var result = new List<TransferHistoryViewModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PN21_Trans_TPosting", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@flag", 5);
                    cmd.Parameters.AddWithValue("@employeeid", employeeId);

                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new TransferHistoryViewModel
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                EmployeeId = reader["employeeid"].ToString(),
                                Wing = reader["Wing"].ToString(),
                                DesignationName = reader["DesignationName"].ToString(),
                                FromDate = Convert.ToDateTime(reader["FromDate"]),
                                ToDate = reader["ToDate"].ToString(),
                                TransferByDepOwn = reader["Transfer_byDep_Own"].ToString(),
                                OrganizationName = reader["OrganizationName"].ToString(),
                                StationName = reader["StationName"].ToString(),
                                TenureId = Convert.ToInt32(reader["TenureId"]),
                                CreatedBy = reader["createdby"].ToString(),
                                CreatedOn = Convert.ToDateTime(reader["createdon"]),
                                UpdatedBy = reader["Updatedby"]?.ToString(),
                                UpdatedOn = reader["Updatedon"] as DateTime?,
                                DeletedBy = reader["Deletedby"]?.ToString(),
                                DeletedOn = reader["deletedon"] as DateTime?,
                                IsActive = Convert.ToBoolean(reader["isactive"]),
                                AddInfo = reader["AddInfo"].ToString()
                            });
                        }
                    }
                }
            }

            return result;
        }

        public async Task<List<OtherRecordViewModel>> GetOtherRecordsByEmployeeAsync(int employeeId)
        {
            var records = new List<OtherRecordViewModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("PN21_Trans_Records_SP", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@employeeid", employeeId);
                cmd.Parameters.AddWithValue("@flag", 2);

                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        records.Add(new OtherRecordViewModel
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            EmployeeId = Convert.ToInt32(reader["employeeid"]),
                            TypeId = Convert.ToInt32(reader["typeid"]),
                            Type = reader["type"].ToString(),
                            FromDate = Convert.ToDateTime(reader["Fromdate"]),
                            ToDate = reader["ToDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ToDate"]),
                            PlaceDuringAbsence = reader["PlaceDuringAbsence"].ToString(),
                            SanctioningAuthority = reader["SanctioningAuthority"].ToString(),
                            AdditionalInfo = reader["AdditionalInfo"].ToString(),
                            CreatedBy = reader["createdby"].ToString(),
                            CreatedOn = Convert.ToDateTime(reader["createdon"]),
                            IP = reader["ip"].ToString(),
                            IsActive = Convert.ToBoolean(reader["isactive"])
                        });
                    }
                }
            }

            return records;
        }


        public bool SaveTraining(TrainingModel model, string ip, int bid)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("PN21_Trans_Training_SP", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@flag", 1);
                cmd.Parameters.AddWithValue("@employeeid", model.EmployeeId);
                cmd.Parameters.AddWithValue("@fromdate", model.FromDate);
                cmd.Parameters.AddWithValue("@todate", model.ToDate);
                cmd.Parameters.AddWithValue("@CourseName", model.CourseName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Host_Institute", model.HostInstitute ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AnyRelevantInfo", model.AnyRelevantInfo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@createdby", "System"); // Replace with actual username/session if needed
                cmd.Parameters.AddWithValue("@Ip", ip ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@bid", bid);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }


        public bool DeleteTraining(int id, string ip, int bid)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("PN21_Trans_Training_SP", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@flag", 3);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@Ip", ip);
                cmd.Parameters.AddWithValue("@bid", bid);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }

        public List<TransferPostingModel> GetTransferPostings(int employeeId)
        {
            List<TransferPostingModel> list = new List<TransferPostingModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PN21_Trans_TPosting", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@flag", 5);
                    cmd.Parameters.AddWithValue("@employeeid", employeeId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new TransferPostingModel
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                EmployeeId = Convert.ToInt32(reader["employeeid"]),
                                Wing = reader["Wing"].ToString(),
                                DesignationName = reader["DesignationName"].ToString(),
                                FromDate = Convert.ToDateTime(reader["FromDate"]),
                                ToDate = reader["ToDate"].ToString(),
                                TransferByDepOwn = reader["Transfer_byDep_Own"].ToString(),
                                OrganizationName = reader["OrganizationName"].ToString(),
                                StationName = reader["StationName"].ToString(),
                                TenureId = reader["TenureId"] as int?,
                                CreatedBy = reader["createdby"].ToString(),
                                CreatedOn = Convert.ToDateTime(reader["createdon"]),
                                UpdatedBy = reader["Updatedby"].ToString(),
                                UpdatedOn = reader["Updatedon"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["Updatedon"]),
                                DeletedBy = reader["Deletedby"].ToString(),
                                DeletedOn = reader["deletedon"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["deletedon"]),
                                IsActive = Convert.ToBoolean(reader["isactive"]),
                                AddInfo = reader["AddInfo"].ToString()
                            });
                        }
                    }
                }
            }

            return list;
        }



    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PRASARNET.Areas.TRAM.Models
{
    public class TransferApplicationModel
    {
        public string Name_Emp { get; set; }
        public string EmployeeCode { get; set; }
        public string CategoryName { get; set; }
        public string DOB { get; set; }
        public string DesignationName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string PresentStationName { get; set; }
        public string DOJ_Present { get; set; }
        public string NoOfYearsServed { get; set; }
        public string DateSuperannuation { get; set; }
        public string Tenure { get; set; }
        public int? TenureYear { get; set; }
        public string DaysCompletedOnTenure { get; set; }
        public string DaysExceeded { get; set; }

        [Required(ErrorMessage = "Please enter the language known.")]
        public string Language { get; set; }
        public string ForwardedTo { get; set; }
        public int? HOO_NameId { get; set; }
        public string DisplayNameWithCodeAndCategory =>
       $"{Name_Emp}/{EmployeeCode}/{CategoryName}";

        public int? SelectedState1 { get; set; }

        [Required(ErrorMessage = "Please select a city for Option 1.")]
        public int? SelectedCity1 { get; set; }
        public int? SelectedState2 { get; set; }

        [Required(ErrorMessage = "Please select a city for Option 2.")]
        public int? SelectedCity2 { get; set; }
        public int? SelectedState3 { get; set; }

        [Required(ErrorMessage = "Please select a city for Option 3.")]
        public int? SelectedCity3 { get; set; }
        public List<SelectListItem> States { get; set; } = new();
        public List<SelectListItem> Cities1 { get; set; } = new();
        public List<SelectListItem> Cities2 { get; set; } = new();
        public List<SelectListItem> Cities3 { get; set; } = new();
        public int? TenureOption1 { get; set; }
        public int? TenureOption2 { get; set; }
        public int? TenureOption3 { get; set; }
        public string? ReasonIfNone { get; set; }

        public List<SelectListItem>? GroundsForTransfer { get; set; }

        [Required(ErrorMessage = "Please select a reason for transfer.")]
        public int? SelectedGroundId { get; set; }

        [Required(ErrorMessage = "Hometown is required.")]
        public string Hometown { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [MaxLength(200, ErrorMessage = "Subject cannot exceed 200 characters.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Details are required.")]
        [MaxLength(3000, ErrorMessage = "Details cannot exceed 3000 characters.")]
        public string Details { get; set; }

        [Required(ErrorMessage = "Please specify if medical facilities are available.")]
        public string MedicalFacilitiesAvailable { get; set; }

        [Required(ErrorMessage = "Please indicate whether you are ready to forgo transfer benefits.")]
        public string WantTransferBenefit { get; set; }
        public List<SelectListItem> YesNoOptions { get; set; } = new()
            {
                new SelectListItem { Text = "-- Select --", Value = "" },
                new SelectListItem { Text = "Yes", Value = "Yes" },
                new SelectListItem { Text = "No", Value = "No" }
            };

    }

    public class ApplicantBasicDetailViewModel
    {
        public int ApplicationId { get; set; }
        public long ApplicationNo { get; set; }
        public string Name_Emp { get; set; }
        public string EmployeeCode { get; set; }
        public string CategoryName { get; set; }
        public string DOB { get; set; }
        public string DesignationName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string PresentStationName { get; set; }
        public string DOJ_Present { get; set; }
        public string NoOfYearsServed { get; set; }
        public string DateSuperannuation { get; set; }
        public string Tenure { get; set; }
        public int? TenureYear { get; set; }
        public string DaysCompletedOnTenure { get; set; }
        public string DaysExceeded { get; set; }

        [Required(ErrorMessage = "Please enter the language known.")]
        public string Language { get; set; }
        public string ForwardedTo { get; set; }
        public int? HOO_NameId { get; set; }
        public string DisplayNameWithCodeAndCategory =>
       $"{Name_Emp}/{EmployeeCode}/{CategoryName}";

        public int? SelectedState1 { get; set; }

        [Required(ErrorMessage = "Please select a city for Option 1.")]
        public int? SelectedCity1 { get; set; }
        public int? SelectedState2 { get; set; }

        [Required(ErrorMessage = "Please select a city for Option 2.")]
        public int? SelectedCity2 { get; set; }
        public int? SelectedState3 { get; set; }

        [Required(ErrorMessage = "Please select a city for Option 3.")]
        public int? SelectedCity3 { get; set; }
        public List<SelectListItem> States { get; set; } = new();
        public List<SelectListItem> Cities1 { get; set; } = new();
        public List<SelectListItem> Cities2 { get; set; } = new();
        public List<SelectListItem> Cities3 { get; set; } = new();
        public int? TenureOption1 { get; set; }
        public int? TenureOption2 { get; set; }
        public int? TenureOption3 { get; set; }
        public string? ReasonIfNone { get; set; }

        public List<SelectListItem>? GroundsForTransfer { get; set; }

        [Required(ErrorMessage = "Please select a reason for transfer.")]
        public int? SelectedGroundId { get; set; }

        [Required(ErrorMessage = "Hometown is required.")]
        public string Hometown { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [MaxLength(200, ErrorMessage = "Subject cannot exceed 200 characters.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Details are required.")]
        [MaxLength(3000, ErrorMessage = "Details cannot exceed 3000 characters.")]
        public string Details { get; set; }

        [Required(ErrorMessage = "Please specify if medical facilities are available.")]
        public string MedicalFacilitiesAvailable { get; set; }

        [Required(ErrorMessage = "Please indicate whether you are ready to forgo transfer benefits.")]
        public string WantTransferBenefit { get; set; }
        public List<SelectListItem> YesNoOptions { get; set; } = new()
            {
                new SelectListItem { Text = "-- Select --", Value = "" },
                new SelectListItem { Text = "Yes", Value = "Yes" },
                new SelectListItem { Text = "No", Value = "No" }
            };

    }

    public class TrainingDetailViewModel
    {
        public int Id { get; set; }
        public int BasicDetailsId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Please enter the training start date.")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [Required(ErrorMessage = "Please enter the training end date.")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        [Required(ErrorMessage = "Please enter the course name.")]
        [StringLength(200)]
        public string CourseName { get; set; }

        [Required(ErrorMessage = "Please enter the host institute.")]
        [StringLength(200)]
        public string Host_Institute { get; set; }

        [StringLength(500)]
        public string AnyRelevantInfo { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }

        public bool IsActive { get; set; }

        [StringLength(50)]
        public string Ip { get; set; }
    }
    public class TransferHistoryViewModel
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string Wing { get; set; }
        public string DesignationName { get; set; }
        public DateTime FromDate { get; set; }
        public string ToDate { get; set; } // Already formatted in SQL
        public string TransferByDepOwn { get; set; }
        public string OrganizationName { get; set; }
        public string StationName { get; set; }
        public int TenureId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsActive { get; set; }
        public string AddInfo { get; set; }
    }
    public class OtherRecordViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string PlaceDuringAbsence { get; set; }
        public string SanctioningAuthority { get; set; }
        public string AdditionalInfo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string IP { get; set; }
        public bool IsActive { get; set; }
    }
    public class HealthCategoryViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int? TypeOfDisability { get; set; }
        public string TypeofDisabilityName { get; set; }
        public string Self_Family { get; set; }
        public string Name { get; set; }
        public string Relation { get; set; }
        public int? Age { get; set; }
        public bool IsActive { get; set; }
        public string Ip { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string Additional_IfAny { get; set; }
    }
    public class DocumentsUploadViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int? DocTypeId { get; set; }
        public string DocType { get; set; }
        public string Subject { get; set; }
        public string Remarks { get; set; }
        public string Filename { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string Ip { get; set; }
        public bool IsActive { get; set; }
    }
 
    public class ApplicantDashboardViewModel
    {
        public List<ApplicantBasicDetailViewModel> BasicDetail { get; set; }
        public List<TrainingDetailViewModel> Trainings { get; set; }
        public List<TransferHistoryViewModel> TransferHistory { get; set; }
        public List<OtherRecordViewModel> OtherRecords { get; set; }
        public List<HealthCategoryViewModel> HealthCategories { get; set; }
        public List<DocumentsUploadViewModel> UploadedDocuments { get; set; }
    }


    //old
    public class TrainingModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string CourseName { get; set; }
        public string HostInstitute { get; set; }
        public string AnyRelevantInfo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Ip { get; set; }
        public bool IsActive { get; set; }
    }
    public class TransferPostingModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Wing { get; set; }
        public string DesignationName { get; set; }
        public DateTime FromDate { get; set; }
        public string ToDate { get; set; }  // already formatted as string in SP
        public string TransferByDepOwn { get; set; }
        public string OrganizationName { get; set; }
        public string StationName { get; set; }
        public int? TenureId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsActive { get; set; }
        public string AddInfo { get; set; }
    }

}

namespace PRASARNET.Areas.TRAM.Models
{
    public class HandelApplicationViewModel
    {
        public int Id { get; set; }
        public int Dealing_StnSec_empId { get; set; }
        public int DealingUsertypeId { get; set; }
        public string Name_emp { get; set; }
        public string ApplicationNo { get; set; }
        public string Emp_Designation { get; set; }
        public string Emp_Present_Place { get; set; }
        public string Emp_ContactNo { get; set; }
        public string Emp_Email { get; set; }
        public string ForwardedTo { get; set; }
        public string Action_Taken { get; set; }
        public string FileUpload { get; set; }
        public string FileUploadId { get; set; }
        public string PrimaryEmail { get; set; }
        public string CcEmail { get; set; }
        public string FinalStatus { get; set; }
        public int CurrentStatusId { get; set; }
        public string CurrentStatus { get; set; }
        public int FinalStatusId { get; set; }
        public int BasicId { get; set; }
        public string SubmittedOn { get; set; }
        public string SerialNo { get; set; }
        public string ApprovedBy { get; set; }
        public string CertifyRemarks { get; set; }
        public bool IsApproved { get; set; }

    }

    public class TrackApplicationViewModel
    {
        public int TrnId { get; set; }
        public int AppId { get; set; }
        public string Status { get; set; }
        public string DealingOffc { get; set; }
        public string AppSubmitOn { get; set; }
        public string Fwd_clsd_Remarks { get; set; }
        public string Fwd_clsd_File { get; set; }
        public string FileName { get; set; }
    }

    public class CertificationViewModel
    {
        public int ApplicationNo { get; set; }
        public string Remarks { get; set; }
    }

    public class CertificationStatusViewModel
    {
        public bool IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public string Remarks { get; set; }
    }

}

namespace PRASARNET.Areas.TRAM.Models
{
    public class TransferApplicationViewModel
    {
        public int ApplicationId { get; set; }
        public string ApplicationNo { get; set; }
        public string ForwardedToCode { get; set; }
        public string ForwardedToName { get; set; }

        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }

        public string TenureOption1 { get; set; }
        public string TenureOption2 { get; set; }
        public string TenureOption3 { get; set; }

        public bool IsFinalSubmit { get; set; }
        public string SubmissionStatus { get; set; }
        public string ProcessStatus { get; set; }
        public string FinalAppSubmitOn { get; set; }
        public string LastDraftedDate { get; set; }

        //unused 
        public string ForwardedTo { get; set; }
        public string LastSavedMessage { get; set; }
    }


}

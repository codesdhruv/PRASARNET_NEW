namespace PRASARNET.Areas.ManageDocument.Models
{
    public class DocumentListModel
    {
        public int DocId { get; set; }
        public string Doc_Title { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string Doc_Type { get; set; }
        public int Doc_Typeid { get; set; }
        public string Doc_OrderNo { get; set; }
        public DateTime? Doc_Dated { get; set; }
        // Add other fields from the DB as needed
    }
}

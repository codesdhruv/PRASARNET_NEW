namespace PRASARNET.Models
{
    public class LOCField
    {
        public int Id { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; } // "text", "number", "dropdown"
        public string Label { get; set; }
        public bool IsRequired { get; set; }
        public bool IsSumColumn { get; set; } // If true, this column stores summed values
    }
}

using System.Data;
namespace PRASARNET.Areas.MasterMng.Models
{
    public class QueryModel
    {
        public string? SqlQuery { get; set; }
        public DataTable? Result { get; set; }
    }
}

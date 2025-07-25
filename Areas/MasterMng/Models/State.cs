using System.ComponentModel.DataAnnotations;

namespace PRASARNET.Areas.MasterMng.Models
{
    public class State
    {
        [Required]
        public int StateID { get; set; }
        [Required]
        public int LGD_Code { get; set; }
        [Required]
        public string StateName { get; set; }
        [Required]
        public string IsState { get; set; }
        [Required]
        public string ZoneID { get; set; }
        [Required]
        public int CencusCode_2001 { get; set; }
        [Required]
        public int CencusCode_2011 { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

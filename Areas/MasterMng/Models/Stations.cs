namespace PRASARNET.Areas.MasterMng.Models
{
    public class Stations
    {
        public int StationID { get; set; }
        public int OrganizationID { get; set; }
        public string StationName { get; set; }
        public int StationTypeID { get; set; }
        public int TenureId { get; set; }
        public string Address1 { get; set; }
        public int CityId { get; set; }
        public int DistrictID { get; set; }
        public int StateID { get; set; }
        public int CountryID { get; set; }
        public string Zip { get; set; }
        public string EmailId { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

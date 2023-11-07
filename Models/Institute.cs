namespace School_Login_SignUp.Models
{
    public class Institute
    {
        public int SchoolId { get; set; }

        public string InstitutionName { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Contact { get; set; }
        public string ZipCodes { get; set; }
        public string Url { get; set; }
        public string AvailableExams { get; set; }
        public string SelectedExams { get; set; }
        public string AvailableFacility { get; set; }
        public string SelectedFacility { get; set; }
        public string SchoolCode { get; set; }
        public bool VerificationStatus { get; set; }
    }
}

namespace Clinic.Models.DTOs
{
    public class UpdatePatientDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? BloodType { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public string? Address { get; set; }
        public string? Allergies { get; set; }
        public string? PhoneNumber { get; set; }
    }
}

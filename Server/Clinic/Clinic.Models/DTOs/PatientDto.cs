namespace Clinic.Models.DTOs
{
    public class PatientDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Age { get; set; }
        public string? BloodType { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public string? Address { get; set; }
        public string? Allergies { get; set; }
        public string? PhoneNumber { get; set; }
        public decimal Balance { get; set; }
        public bool IsProfileComplete { get; set; }
    }
}

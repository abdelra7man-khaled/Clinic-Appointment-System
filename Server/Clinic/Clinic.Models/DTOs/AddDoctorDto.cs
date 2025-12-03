namespace Clinic.Models.DTOs
{
    public class AddDoctorDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Biography { get; set; }
        public List<int> SpecialtyIds { get; set; }
    }

}

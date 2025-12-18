namespace Clinic.Models.DTOs
{
    public class UpdateDoctorDto
    {
        public string FullName { get; set; }
        public string Biography { get; set; }
        public int ExperienceYears { get; set; }
        public decimal ConsultationFee { get; set; }
    }

}

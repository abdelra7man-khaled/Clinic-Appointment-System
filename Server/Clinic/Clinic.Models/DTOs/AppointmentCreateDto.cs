namespace Clinic.Models.DTOs
{
    public class AppointmentCreateDto
    {
        public int DoctorId;
        public DateTime StartTime;
        public DateTime EndTime;
        public decimal Fee;
        public string Notes;
    }
}

using Clinic.Models.Enums;

namespace Clinic.Models.DTOs
{
    public class AppointmentCreateDto
    {
        public int DoctorId;
        public DateTime StartTime;
        public DateTime EndTime;
        public AppointmentType AppointmentType;
        public decimal Fee;
        public string Notes;
    }
}

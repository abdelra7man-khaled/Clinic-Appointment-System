using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public string FullName { get; set; }
        public string? Biography { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalPatients { get; set; }
        public int ExperienceYears { get; set; }
        public decimal ConsultationFee { get; set; }
        public decimal Balance { get; set; }
        public string? PhotoUrl { get; set; }
        public ICollection<DoctorSpecialty> DoctorSpecialties { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<DoctorSchedule> Schedules { get; set; }
    }
}

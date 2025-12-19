using System;
using System.Collections.Generic;

namespace Clinic.Models.DTOs
{
    public class DoctorProfileDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Biography { get; set; }
        public int ExperienceYears { get; set; }
        public decimal ConsultationFee { get; set; }
        public string PhotoUrl { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalPatients { get; set; }
        public List<string> Specialties { get; set; } = new List<string>();
        public List<DoctorScheduleDto> Schedules { get; set; } = new List<DoctorScheduleDto>();
    }

    public class DoctorScheduleDto
    {
        public string Day { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public bool IsBlocked { get; set; }
    }
}

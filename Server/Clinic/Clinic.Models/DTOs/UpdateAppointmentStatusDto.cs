using Clinic.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.DTOs
{
    public class UpdateAppointmentStatusDto
    {
        [Required]
        public AppointmentStatus Status { get; set; }
    }
}

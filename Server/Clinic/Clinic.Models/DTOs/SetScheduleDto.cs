using System;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.DTOs
{
    public class SetScheduleDto
    {
        [Required]
        public DayOfWeek Day { get; set; }
        [Required]
        public string Start { get; set; } // "HH:mm"
        [Required]
        public string End { get; set; } // "HH:mm"
        public bool IsBlocked { get; set; }
    }
}

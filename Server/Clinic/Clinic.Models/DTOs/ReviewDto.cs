using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.DTOs
{
    public class ReviewDto
    {
        [Required]
        public int DoctorId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}

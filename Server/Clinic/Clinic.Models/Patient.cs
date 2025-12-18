using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        [Required]
        public decimal Balance { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}

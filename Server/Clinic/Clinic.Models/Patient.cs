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
        public string Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public Appointment Appointment { get; set; }
    }
}

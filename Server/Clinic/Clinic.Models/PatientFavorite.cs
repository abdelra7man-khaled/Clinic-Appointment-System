using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.Models
{
    public class PatientFavorite
    {
        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }

        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public Doctor Doctor { get; set; }
    }
}

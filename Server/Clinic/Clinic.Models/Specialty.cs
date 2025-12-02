namespace Clinic.Models
{
    public class Specialty
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<DoctorSpecialty> DoctorSpecialties { get; set; }

    }
}

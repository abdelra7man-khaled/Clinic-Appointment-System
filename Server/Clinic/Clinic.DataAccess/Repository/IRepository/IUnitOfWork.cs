using Clinic.Models;

namespace Clinic.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<ApplicationUser> Users { get; }
        IRepository<Patient> Patients { get; }
        IRepository<Doctor> Doctors { get; }
        IRepository<Specialty> Specialties { get; }
        IRepository<DoctorSpecialty> DoctorSpecialties { get; }
        IRepository<Appointment> Appointments { get; }
        IRepository<Payment> Payments { get; }
        Task<int> SaveChangesAsync();
    }
}

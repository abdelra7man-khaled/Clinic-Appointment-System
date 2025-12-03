using Clinic.DataAccess.Data;
using Clinic.DataAccess.Repository.IRepository;
using Clinic.Models;

namespace Clinic.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IRepository<ApplicationUser> Users { get; }
        public IRepository<Patient> Patients { get; }
        public IRepository<Doctor> Doctors { get; }
        public IRepository<Specialty> Specialties { get; }
        public IRepository<DoctorSpecialty> DoctorSpecialties { get; }
        public IRepository<Appointment> Appointments { get; }

        public IRepository<Payment> Payments { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Users = new Repository<ApplicationUser>(_context);
            Patients = new Repository<Patient>(_context);
            Doctors = new Repository<Doctor>(_context);
            Specialties = new Repository<Specialty>(_context);
            DoctorSpecialties = new Repository<DoctorSpecialty>(_context);
            Appointments = new Repository<Appointment>(_context);
            Payments = new Repository<Payment>(_context);
        }
        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}

using Clinic.Models;
using Clinic.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Clinic.DataAccess.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<DoctorSpecialty> DoctorSpecialties { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DoctorSpecialty>()
                .HasKey(ds => new { ds.DoctorId, ds.SpecialtyId });

            modelBuilder.Entity<DoctorSpecialty>()
                .HasOne(ds => ds.Doctor)
                .WithMany(d => d.DoctorSpecialties)
                .HasForeignKey(ds => ds.DoctorId);

            modelBuilder.Entity<DoctorSpecialty>()
                .HasOne(ds => ds.Specialty)
                .WithMany()
                .HasForeignKey(ds => ds.SpecialtyId);

            modelBuilder.Entity<DoctorSchedule>()
                         .Property(ds => ds.Day)
                         .HasConversion<string>();

            modelBuilder.Entity<Doctor>()
                        .Property(d => d.Biography)
                        .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<Appointment>()
                        .HasOne(a => a.Doctor)
                        .WithMany(d => d.Appointments)
                        .HasForeignKey(a => a.DoctorId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Appointment>()
                         .HasOne(a => a.Patient)
                         .WithMany(p => p.Appointments)
                         .HasForeignKey(a => a.PatientId)
                         .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<ApplicationUser>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<ApplicationUser>().HasIndex(u => u.Email).IsUnique();

            LoadUsers(modelBuilder);
            LoadPatients(modelBuilder);
            LoadDoctors(modelBuilder);
            LoadSpecialties(modelBuilder);
            LoadDoctorSpecialties(modelBuilder);
            LoadAppointments(modelBuilder);
            LoadPayments(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private static void LoadUsers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@clinic.com",
                    PasswordHash = "admin-hash",
                    Role = Role.Admin
                },
                new ApplicationUser
                {
                    Id = 2,
                    Username = "abdelrahman",
                    Email = "abdelrahman@clinic.com",
                    PasswordHash = "123456",
                    Role = Role.Doctor
                },
                new ApplicationUser
                {
                    Id = 3,
                    Username = "ahmed",
                    Email = "ahmed@clinic.com",
                    PasswordHash = "123456",
                    Role = Role.Patient
                },
                 new ApplicationUser
                 {
                     Id = 4,
                     Username = "amr",
                     Email = "amr@clinic.com",
                     PasswordHash = "123456",
                     Role = Role.Doctor
                 },
                 new ApplicationUser
                 {
                     Id = 5,
                     Username = "samy",
                     Email = "samy@clinic.com",
                     PasswordHash = "123456",
                     Role = Role.Doctor
                 }
            );
        }

        private static void LoadPatients(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().HasData(
                new Patient
                {
                    Id = 1,
                    UserId = 3,
                    FullName = "Samy Mohamed",
                    PhoneNumber = "01123456789",
                    Balance = 3700m
                },
                new Patient
                {
                    Id = 2,
                    UserId = 5,
                    FullName = "Ahmed Ali",
                    PhoneNumber = "01123856109",
                    Balance = 2500m
                }
            );
        }

        private static void LoadDoctors(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>().HasData(
                new Doctor
                {
                    Id = 1,
                    UserId = 2,
                    FullName = "Abdelrahman Khaled",
                    Biography = "Senior Cardiologist"
                },
                new Doctor
                {
                    Id = 2,
                    UserId = 4,
                    FullName = "Amr Sobhy",
                    Biography = "Senior Dermatologist"
                }
            );
        }

        private static void LoadSpecialties(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Specialty>().HasData(
                new Specialty { Id = 1, Name = "Cardiology" },
                new Specialty { Id = 2, Name = "Dermatology" },
                new Specialty { Id = 3, Name = "Neurology" }
            );
        }

        private static void LoadDoctorSpecialties(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DoctorSpecialty>().HasData(
                new DoctorSpecialty
                {
                    DoctorId = 1,
                    SpecialtyId = 1
                },
                new DoctorSpecialty
                {
                    DoctorId = 2,
                    SpecialtyId = 2
                }
            );
        }

        private static void LoadAppointments(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>().HasData(
                new Appointment
                {
                    Id = 1,
                    PatientId = 1,
                    DoctorId = 1,
                    StartTime = new DateTime(2025, 1, 20, 10, 0, 0),
                    EndTime = new DateTime(2025, 1, 20, 10, 30, 0),
                    Status = AppointmentStatus.Pending,
                    AppointmentType = AppointmentType.Regular,
                    Fee = 300m,
                    Notes = "Initial checkup"
                },
                new Appointment
                {
                    Id = 2,
                    PatientId = 2,
                    DoctorId = 2,
                    StartTime = new DateTime(2025, 5, 17, 8, 0, 0),
                    EndTime = new DateTime(2025, 5, 17, 8, 30, 0),
                    Status = AppointmentStatus.Pending,
                    AppointmentType = AppointmentType.FollowUp,
                    Fee = 100m,
                    Notes = "Follow Up Appointment"
                }
            );
        }

        private static void LoadPayments(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>().HasData(
                new Payment
                {
                    Id = 1,
                    AppointmentId = 1,
                    Amount = 300m,
                    PaymentMethod = PaymentMethod.Cash,
                    PaidAt = new DateTime(2025, 1, 20, 9, 55, 0)
                },
                new Payment
                {
                    Id = 2,
                    AppointmentId = 2,
                    Amount = 100m,
                    PaymentMethod = PaymentMethod.CreditCard,
                    PaidAt = new DateTime(2025, 5, 15, 6, 40, 0)
                }
            );
        }


    }
}

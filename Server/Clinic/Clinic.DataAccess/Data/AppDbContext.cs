using Clinic.Models;
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
                         .WithOne(p => p.Appointment)
                         .HasForeignKey<Appointment>(a => a.PatientId)
                         .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<ApplicationUser>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<ApplicationUser>().HasIndex(u => u.Email).IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}

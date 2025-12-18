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
        public DbSet<Review> Reviews { get; set; }

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

            modelBuilder.Entity<Patient>()
                        .Property(p => p.Balance)
                        .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Appointment>()
                        .Property(a => a.Fee)
                        .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                        .Property(p => p.Amount)
                        .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Patient)
                .WithMany()
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Doctor)
                .WithMany()
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}

using Clinic.Models;
using Clinic.Models.Enums;

namespace Clinic.Services.Appointments
{
    public class AppointmentFactory
    {
        public static Appointment CreateAppointment(
            AppointmentType type,
            int patientId,
            int doctorId,
            DateTime start,
            DateTime end,
            decimal baseFee,
            string notes = "")
        {
            decimal finalFee = type switch
            {
                AppointmentType.Emergency => baseFee * 1.5m,
                AppointmentType.FollowUp => baseFee * 0.8m,
                _ => baseFee,
            };

            return new Appointment
            {
                PatientId = patientId,
                DoctorId = doctorId,
                StartTime = start,
                EndTime = end,
                Fee = finalFee,
                Status = AppointmentStatus.Pending,
                AppointmentType = type,
                Notes = notes
            };
        }
    }

}

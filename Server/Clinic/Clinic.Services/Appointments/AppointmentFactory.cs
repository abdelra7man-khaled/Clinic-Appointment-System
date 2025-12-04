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
            decimal finalFee = baseFee;

            switch (type)
            {
                case AppointmentType.Emergency:
                    finalFee *= 1.5m;
                    break;
                case AppointmentType.FollowUp:
                    finalFee *= 0.8m;
                    break;
                default:
                    break;
            }

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

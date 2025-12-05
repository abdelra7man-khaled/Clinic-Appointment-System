using Clinic.Models;
using Clinic.Models.DTOs;
using Clinic.Services.Logging;

namespace Clinic.Services.Payments
{
    public class CashPaymentStrategy : IPaymentStrategy
    {
        public bool Pay(decimal totalAmount, Patient patient, PaymentDetails? PaymentDetails = null)
        {
            if (patient.Balance < totalAmount)
            {
                Logger.Instance.LogWarning("Insufficient wallet balance.");
                return false;
            }

            patient.Balance -= totalAmount;

            Logger.Instance.LogInfo($"Cash payment successful. New Balance: {patient.Balance}");
            return true;
        }
    }
}

using Clinic.Models;

namespace Clinic.Services.Payments
{
    public interface IPaymentStrategy
    {
        bool Pay(decimal totalAmount, Patient patient, PaymentDetails? PaymentDetails = null);
    }
}

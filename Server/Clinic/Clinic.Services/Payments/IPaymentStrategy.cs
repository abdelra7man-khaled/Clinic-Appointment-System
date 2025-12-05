using Clinic.Models;
using Clinic.Models.DTOs;

namespace Clinic.Services.Payments
{
    public interface IPaymentStrategy
    {
        bool Pay(decimal totalAmount, Patient patient, PaymentDetails? PaymentDetails = null);
    }
}

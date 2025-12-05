using Clinic.Models;
using Clinic.Models.DTOs;
using Clinic.Services.Logging;

namespace Clinic.Services.Payments
{
    public class CreditCardPaymentStrategy : IPaymentStrategy
    {

        public bool Pay(decimal totalAmount, Patient patient, PaymentDetails PaymentDetails)
        {
            patient.Balance -= totalAmount;
            Logger.Instance.LogInfo("Credit card payment successful");
            return true;
        }


    }


}

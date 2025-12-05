using Clinic.Models;

namespace Clinic.Services.Payments
{
    public class PaymentContext
    {
        private IPaymentStrategy _strategy;

        public PaymentContext(IPaymentStrategy strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(IPaymentStrategy strategy)
        {
            _strategy = strategy;
        }

        public bool Pay(decimal amount, Patient patient, PaymentDetails details = null)
        {
            return _strategy.Pay(amount, patient, details);
        }
    }

}

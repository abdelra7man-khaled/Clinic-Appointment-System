using Clinic.Models;
using Clinic.Models.DTOs;
using Clinic.Services.Logging;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Clinic.Services.Payments
{
    public class CreditCardPaymentStrategyProxy : IPaymentStrategy
    {
        private readonly IPaymentStrategy _creditCardPaymentStrategy;

        private ReadOnlyCollection<ActualCreditCards> CreditCards = new ReadOnlyCollection<ActualCreditCards>(
                 new List<ActualCreditCards>
                 {
                    new ActualCreditCards("1234567812345678", "887", new DateTime(2027, 11, 1)),
                    new ActualCreditCards("8765432187654321", "123", new DateTime(2026, 5, 1)) ,
                    new ActualCreditCards("1111222233334444", "444" , new DateTime(2029 , 8 , 1))
                 }
             );

        public bool Pay(decimal totalAmount, Patient patient, PaymentDetails? PaymentDetails = null)
        {
            if (ValidateCreditCard(PaymentDetails))
            {
                Logger.Instance.LogWarning("Invalid Credit Card");
                return false;
            }

            Logger.Instance.LogInfo("Valid Credit Card payment");
            return _creditCardPaymentStrategy.Pay(totalAmount, patient, PaymentDetails);
        }


        public bool ValidateCreditCard(PaymentDetails PaymentDetails)
        {
            DateTime expiryDate;
            bool isValid = DateTime.TryParseExact(
                PaymentDetails.ExpiryDate,
                "MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out expiryDate
            );

            if (!isValid)
            {
                return false;
            }

            if (PaymentDetails.CardNumber.Length != 16 ||
                PaymentDetails.CVV.Length != 3 ||
                expiryDate < DateTime.Now
                )
            {
                return false;
            }

            foreach (var card in CreditCards)
            {
                if (PaymentDetails.CardNumber == card.CardNumber &&
                    PaymentDetails.CVV == card.CVV &&
                    expiryDate == card.ExpiryDate
                   )
                {
                    return true;
                }
            }
            return false;
        }


        public class ActualCreditCards
        {
            public string CardNumber;
            public string CVV;
            public DateTime ExpiryDate;

            public ActualCreditCards(string CardNumber, string CVV, DateTime ExpiryDate)
            {
                this.CardNumber = CardNumber;
                this.CVV = CVV;
                this.ExpiryDate = ExpiryDate;
            }
        }
    }
}

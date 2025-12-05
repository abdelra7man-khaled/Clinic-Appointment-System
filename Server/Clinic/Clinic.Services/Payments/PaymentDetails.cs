namespace Clinic.Services.Payments
{
    public class PaymentDetails
    {
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string ExpiryDate { get; set; }
    }
}

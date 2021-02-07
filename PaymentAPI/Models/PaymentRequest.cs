namespace PaymentAPI.Models
{
    using System;

    public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public string CreditCardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string CVV { get; set; }

        public string Currency { get; set; }
    }
}

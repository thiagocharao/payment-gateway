namespace PaymentAPI.Domain
{
    using System;

    public class Payment : Document
    {
        public Guid UserId { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string CreditCardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string Cvv { get; set; }
        public string PaymentStatus { get; set; }
    }
}

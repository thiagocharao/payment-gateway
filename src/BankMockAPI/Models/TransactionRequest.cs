namespace BankMockAPI.Models
{
    using System;

    public class TransactionRequest
    {
        public Guid PaymentId { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string CreditCardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string Cvv { get; set; }
    }
}

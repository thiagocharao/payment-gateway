using System;

namespace PaymentGateway.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        
        public string CreditCardNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string CVV { get; set; }
        
        public string Currency { get; set; }
    }
}

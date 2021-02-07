namespace PaymentAPI.Models
{
    using System;

    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentStatus { get; set; }
    }
}

namespace PaymentAPI.Models
{
    using System;

    public class PaymentResponse
    {
        private readonly string _creditCardNumber;
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentStatus { get; set; }
        public string CreditCardNumber
        {
            get => _creditCardNumber.Length > 4 ? _creditCardNumber[^4..] : _creditCardNumber;
            init => _creditCardNumber = value;
        }
    }
}

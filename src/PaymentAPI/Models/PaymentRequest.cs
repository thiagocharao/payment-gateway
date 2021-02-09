namespace PaymentAPI.Models
{
    using System.ComponentModel.DataAnnotations;

    public class PaymentRequest
    {
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string CreditCardNumber { get; set; }
        [Required]
        public int ExpiryMonth { get; set; }
        [Required]
        public int ExpiryYear { get; set; }
        [Required]
        public string Cvv { get; set; }
        [Required]
        public string Currency { get; set; }
    }
}

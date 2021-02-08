namespace PaymentAPI.Models
{
    using System.ComponentModel.DataAnnotations;

    public class TokenRequest
    {
        [Required]
        [MinLength(3)]
        public string Credentials { get; set; }
    }
}

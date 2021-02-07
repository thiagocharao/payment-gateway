namespace PaymentAPI.Domain
{
    using System;
    using System.Text.Json.Serialization;

    public class BankingTransactionResponse
    {
        //[JsonPropertyName("paymentId")]
        public Guid PaymentId { get; set; }

        //[JsonPropertyName("paymentStatus")]
        public string PaymentStatus { get; set; }
    }
}

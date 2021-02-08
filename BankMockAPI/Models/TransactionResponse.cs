namespace BankMockAPI.Models
{
    using System.Text.Json.Serialization;
    using System;

    public class TransactionResponse
    {
        public Guid PaymentId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentStatus PaymentStatus { get; set; }
    }

    public enum PaymentStatus
    {
        Approved, Declined
    }
}

namespace BankMockAPI.Models
{
    using System;
    using System.Text.Json.Serialization;

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

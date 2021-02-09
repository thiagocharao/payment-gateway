namespace PaymentAPI.Domain
{
    using System;

    public class BankingTransactionResponse
    {
        public Guid PaymentId { get; set; }

        public string PaymentStatus { get; set; }
    }
}

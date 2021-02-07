namespace PaymentAPI.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;

    public interface IBankingService
    {
        Task<Payment> ProcessPaymentAsync(Payment payment, CancellationToken ct);
    }
}

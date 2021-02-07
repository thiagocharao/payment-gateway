namespace PaymentAPI.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;

    public interface IPaymentService
    {
        Task<IEnumerable<Payment>> GetUserPaymentsAsync(Guid userId, CancellationToken ct);
        Task<Payment> GetUserPaymentAsync(Guid userId, Guid paymentId, CancellationToken ct);
        Task<Payment> CreateUserPaymentAsync(Guid userId, Payment payment, CancellationToken ct);
    }
}

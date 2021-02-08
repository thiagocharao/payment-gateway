namespace PaymentAPI.Domain.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    using Domain;

    public interface IBankingService
    {
        Task<Payment> ProcessPaymentAsync(Payment payment, string processingEndpoint, CancellationToken ct);
    }
}

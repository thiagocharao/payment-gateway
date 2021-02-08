namespace PaymentAPI.Domain.Services
{
    using System.Net.Http;
    using System.Text.Json;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading;

    using Domain;

    using Repositories;

    public class BankingService : IBankingService
    {
        private readonly HttpClient _httpClient;
        private readonly IRepository<Payment> _paymentRepository;

        public BankingService(HttpClient client, IRepository<Payment> paymentRepository)
        {
            _httpClient = client;
            _paymentRepository = paymentRepository;
        }

        public async Task<Payment> ProcessPaymentAsync(
            Payment payment, string processingEndpoint, CancellationToken ct)
        {
            var paymentJson = new StringContent(JsonSerializer.Serialize(payment), Encoding.UTF8, "application/json");

            using var httpResponse =
                await _httpClient.PostAsync(processingEndpoint, paymentJson, ct);

            httpResponse.EnsureSuccessStatusCode();

            var responseStream = await httpResponse.Content.ReadAsStreamAsync(ct);
            var transactionResponse = await JsonSerializer.DeserializeAsync<BankingTransactionResponse>(
                responseStream,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                },
                cancellationToken: ct);

            payment.PaymentStatus = transactionResponse?.PaymentStatus ?? "Processing";

            await _paymentRepository.ReplaceOneAsync(payment, ct);

            return payment;
        }
    }
}

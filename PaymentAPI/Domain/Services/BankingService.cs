namespace PaymentAPI.Domain.Services
{
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Domain;
    using Microsoft.Extensions.Configuration;
    using Repositories;

    public class BankingService : IBankingService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Payment> _paymentRepository;

        public BankingService(HttpClient client, IConfiguration configuration, IRepository<Payment> paymentRepository)
        {
            _httpClient = client;
            _configuration = configuration;
            _paymentRepository = paymentRepository;
        }

        public async Task<Payment> ProcessPaymentAsync(Payment payment, CancellationToken ct)
        {
            var paymentJson = new StringContent(JsonSerializer.Serialize(payment), Encoding.UTF8, "application/json");

            using var httpResponse =
                await _httpClient.PostAsync(_configuration["BankingPaymentProcessEndpoint"], paymentJson, ct);

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

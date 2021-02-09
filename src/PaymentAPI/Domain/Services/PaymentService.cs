namespace PaymentAPI.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Domain;

    using Repositories;

    public class PaymentService : IPaymentService
    {
        private readonly IRepository<Payment> _paymentRepository;

        public PaymentService(IRepository<Payment> paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<IEnumerable<Payment>> GetUserPaymentsAsync(Guid userId, CancellationToken ct)
        {
            return await _paymentRepository.FilterByAsync(x => x.UserId.Equals(userId), ct);
        }

        public async Task<Payment> GetUserPaymentAsync(Guid userId, Guid paymentId, CancellationToken ct)
        {
            return await _paymentRepository.FindOneAsync(x => x.UserId.Equals(userId) && x.Id.Equals(paymentId), ct);
        }

        public async Task<Payment> CreateUserPaymentAsync(Guid userId, Payment payment, CancellationToken ct)
        {
            payment.UserId = userId;
            return await _paymentRepository.InsertOneAsync(payment, ct);
        }
    }
}

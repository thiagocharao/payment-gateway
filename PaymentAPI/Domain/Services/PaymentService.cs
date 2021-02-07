namespace PaymentAPI.Domain.Services
{
    using System;
    using System.Collections.Generic;
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

        public async Task<IEnumerable<Payment>> GetUserPayments(Guid merchantId)
        {
            return await _paymentRepository.FilterByAsync(x => x.MerchantId.Equals(merchantId));
        }
    }
}

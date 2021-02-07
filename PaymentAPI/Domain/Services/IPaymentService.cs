namespace PaymentAPI.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain;

    public interface IPaymentService
    {
        Task<IEnumerable<Payment>> GetUserPayments(Guid merchantId);
    }
}

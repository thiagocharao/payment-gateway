namespace BankMockAPI.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        [HttpPost]
        public async Task<TransactionResponse> Post(TransactionRequest transaction)
        {
            var rng = new Random();
            return new TransactionResponse
            {
                PaymentId = transaction.PaymentId,
                PaymentStatus = RandomEnumValue<PaymentStatus>(rng)
            };
        }

        private static T RandomEnumValue<T>(Random rng)
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(rng.Next(v.Length));
        }
    }
}

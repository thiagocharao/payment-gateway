namespace BankMockAPI.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [ApiController]
    [Route("transactions")]
    public class TransactionController : ControllerBase
    {
        [HttpPost]
        public TransactionResponse Post(TransactionRequest transaction)
        {
            var random = new Random();
            var enumValues = Enum.GetValues(typeof(PaymentStatus));
            return new TransactionResponse
            {
                PaymentId = transaction.PaymentId,
                PaymentStatus = Enum.Parse<PaymentStatus>(random.Next(enumValues.Length).ToString())
            };
        }
    }
}

namespace PaymentAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Repositories;
    using Domain.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models;

    [ApiController]
    [Route("payments")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymentService _paymentService;

        public PaymentController(
            ILogger<PaymentController> logger,
            IPaymentService paymentService)
        {
            _logger = logger;
            _paymentService = paymentService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<Payment>> GetAll()
        {
            var userId = this.User.FindFirst("userid")?.Value;

            var payments = await _paymentService.GetUserPayments(Guid.Parse(userId ?? string.Empty));

            // Apply automapper
            return payments.Select(x => new Payment { Amount = x.Amount });
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<Payment>> Get(Guid id) => throw new NotImplementedException();

        [HttpPost]
        public async Task<Payment> Create(Payment payment) => throw new NotImplementedException();
    }
}

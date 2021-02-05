using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGateway.Models;

namespace PaymentGateway.Controllers
{
    [ApiController]
    [Route("payments")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<Payment>> GetAll()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<Payment>> Get(Guid id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<Payment> Create(Payment payment)
        {
            throw new NotImplementedException();
        }
    }
}

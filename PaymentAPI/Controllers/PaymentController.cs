namespace PaymentAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly IBankingService _bankingService;

        public PaymentController(
            ILogger<PaymentController> logger,
            IPaymentService paymentService,
            IMapper mapper, IBankingService bankingService)
        {
            _logger = logger;
            _paymentService = paymentService;
            _mapper = mapper;
            _bankingService = bankingService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<PaymentResponse>> GetAllAsync(CancellationToken ct)
        {
            var userId = this.User.FindFirst("userid")?.Value;

            var payments = await _paymentService.GetUserPaymentsAsync(Guid.Parse(userId ?? string.Empty), ct);

            return _mapper.Map<IEnumerable<PaymentResponse>>(payments);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<PaymentResponse> GetAsync(Guid id, CancellationToken ct)
        {
            var userId = this.User.FindFirst("userid")?.Value;

            var payment = await _paymentService.GetUserPaymentAsync(Guid.Parse(userId), id, ct);

            return _mapper.Map<PaymentResponse>(payment);
        }

        [Authorize]
        [HttpPost]
        public async Task<PaymentResponse> Create(PaymentRequest paymentRequest, CancellationToken ct)
        {
            var userId = Guid.Parse(this.User.FindFirst("userid")?.Value);

            var payment =
                await _paymentService.CreateUserPaymentAsync(userId, _mapper.Map<Domain.Payment>(paymentRequest), ct);
            await _bankingService.ProcessPaymentAsync(payment, ct);
            payment = await _bankingService.ProcessPaymentAsync(payment, ct);

            return _mapper.Map<PaymentResponse>(payment);
        }
    }
}

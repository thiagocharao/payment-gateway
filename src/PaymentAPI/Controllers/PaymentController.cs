namespace PaymentAPI.Controllers
{
    using System.Collections.Generic;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using System.Threading;
    using System;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using AutoMapper;

    using Domain.Services;

    using Models;

    [ApiController]
    [Route("payments")]
    [Authorize(Policy = "UserIdRequired")]
    public class PaymentController : ControllerBase
    {
        private Guid UserId => Guid.Parse(User.FindFirst("userid")?.Value!);

        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IBankingService _bankingService;

        public PaymentController(
            ILogger<PaymentController> logger,
            IPaymentService paymentService,
            IMapper mapper,
            IBankingService bankingService, IConfiguration configuration)
        {
            _logger = logger;
            _paymentService = paymentService;
            _mapper = mapper;
            _bankingService = bankingService;
            _configuration = configuration;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PaymentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllAsync(CancellationToken ct = default)
        {
            var payments = await _paymentService.GetUserPaymentsAsync(UserId, ct);

            return Ok(_mapper.Map<IEnumerable<PaymentResponse>>(payments));
        }

        [HttpGet("{id}", Name = "GetById")]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var payment = await _paymentService.GetUserPaymentAsync(UserId, id, ct);

            if (payment == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PaymentResponse>(payment));
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateAsync(PaymentRequest paymentRequest, CancellationToken ct = default)
        {
            var payment =
                await _paymentService.CreateUserPaymentAsync(UserId, _mapper.Map<Domain.Payment>(paymentRequest), ct);

            payment = await _bankingService.ProcessPaymentAsync(
                payment, _configuration["BankingPaymentProcessEndpoint"], ct);

            return CreatedAtAction(
                actionName: "GetById",
                routeValues: new
                {
                    id = payment.Id
                },
                value: _mapper.Map<PaymentResponse>(payment));
        }
    }
}

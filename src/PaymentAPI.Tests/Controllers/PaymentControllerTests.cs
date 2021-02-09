namespace PaymentAPI.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Threading;
    using System;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using AutoMapper;

    using FluentAssertions;

    using Models;

    using Moq;

    using PaymentAPI.Controllers;
    using PaymentAPI.Domain.Services;
    using PaymentAPI.Domain;

    using Xunit;


    public class PaymentControllerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<PaymentController>> _loggerMock;
        private readonly Mock<IPaymentService> _paymentServiceMock;
        private readonly Mock<IBankingService> _bankingServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly PaymentController _controller;

        private static Guid UserId => new("aeb9fd74-fb35-4767-82bf-4dbdff7200f9");

        public PaymentControllerTests()
        {
            _mapper = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            }).CreateMapper();

            _loggerMock = new Mock<ILogger<PaymentController>>();
            _paymentServiceMock = new Mock<IPaymentService>();
            _bankingServiceMock = new Mock<IBankingService>();
            _configurationMock = new Mock<IConfiguration>();
            _controller = new PaymentController(_loggerMock.Object, _paymentServiceMock.Object, _mapper, _bankingServiceMock.Object,
                _configurationMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            _controller.ControllerContext.HttpContext.User = GetUserWithClaims(UserId);
        }

        [Fact]
        public async Task GetAll_ReturnsEmptyListWhenNothingIsFound()
        {
            // Arrange
            _paymentServiceMock
                .Setup(x => x.GetUserPaymentsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<Payment>());

            // Act
            var response = await _controller.GetAllAsync() as OkObjectResult;

            // Assert
            response?.StatusCode.Should().Be(StatusCodes.Status200OK);
            var values = response?.Value as IEnumerable<PaymentResponse>;
            values.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_Returns200HidingCreditCardNumber()
        {
            // Arrange
            _paymentServiceMock
                .Setup(x => x.GetUserPaymentsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(GetDomainPayments());

            // Act
            var response = await _controller.GetAllAsync() as OkObjectResult;

            // Assert
            response?.StatusCode.Should().Be(StatusCodes.Status200OK);
            var values = response?.Value as IEnumerable<PaymentResponse>;
            values!.First().Should().BeEquivalentTo(new PaymentResponse
            {
                Amount = 23.45m,
                Currency = "EUR",
                CreatedAt = DateTime.Parse("2021-02-08T12:41:28.595Z"),
                PaymentStatus = "Declined",
                CreditCardNumber = "4444"
            });
        }

        [Fact]
        public async Task GetById_Returns200HidingCreditCardNumber()
        {
            // Arrange
            var expectedPaymentId = new Guid("a2b6606e-258c-4a5a-b4df-c8f25a0afe48");
            _paymentServiceMock
                .Setup(x => x.GetUserPaymentAsync(
                    UserId,
                    expectedPaymentId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(GetDomainPayments().First());

            // Act
            var response = await _controller.GetByIdAsync(expectedPaymentId) as OkObjectResult;

            // Assert
            response?.StatusCode.Should().Be(StatusCodes.Status200OK);
            var payment = response?.Value as PaymentResponse;
            payment!.Should().BeEquivalentTo(new PaymentResponse
            {
                Amount = 23.45m,
                Currency = "EUR",
                CreatedAt = DateTime.Parse("2021-02-08T12:41:28.595Z"),
                PaymentStatus = "Declined",
                CreditCardNumber = "4444"
            });
        }

        [Fact]
        public async Task GetById_Returns404WhenNothingIsFound()
        {
            // Arrange
            _paymentServiceMock
                .Setup(x => x.GetUserPaymentAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Payment)null);

            // Act
            var response = await _controller.GetByIdAsync(It.IsAny<Guid>()) as NotFoundResult;

            // Assert
            response?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task Create_Returns201CreatedHidingCreditCardNumber()
        {
            // Arrange
            var payment = new Payment
            {
                Amount = 10,
                Currency = "EUR",
                ExpiryMonth = 10,
                ExpiryYear = 2028,
                CreditCardNumber = "4444-4444-4444-4444",
                PaymentStatus = "Approved",
                CreatedAt = DateTime.Parse("2021-02-08T12:41:28.595Z")
            };

            _paymentServiceMock
                .Setup(x => x.CreateUserPaymentAsync(UserId, It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payment);

            _bankingServiceMock
                .Setup(x => x.ProcessPaymentAsync(payment, It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payment);

            // Act
            var response = await _controller.CreateAsync(It.IsAny<PaymentRequest>()) as CreatedAtActionResult;

            // Assert
            _bankingServiceMock.Verify(
                x => x.ProcessPaymentAsync(It.IsAny<Payment>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once());
            response?.StatusCode.Should().Be(StatusCodes.Status201Created);
            var paymentResponse = response?.Value as PaymentResponse;
            paymentResponse!.Should().BeEquivalentTo(new PaymentResponse
            {
                Amount = 10,
                Currency = "EUR",
                CreatedAt = DateTime.Parse("2021-02-08T12:41:28.595Z"),
                PaymentStatus = "Approved",
                CreditCardNumber = "4444"
            });
        }

        private static IEnumerable<Payment> GetDomainPayments() =>
            new Payment[]
            {
                new()
                {
                    Id = new Guid("a2b6606e-258c-4a5a-b4df-c8f25a0afe48"),
                    UserId = UserId,
                    Amount = 23.45m,
                    Currency = "EUR",
                    CreatedAt = DateTime.Parse("2021-02-08T12:41:28.595Z"),
                    ExpiryMonth = 1,
                    ExpiryYear = 2028,
                    PaymentStatus = "Declined",
                    CreditCardNumber = "1111-2222-3333-4444",
                    Cvv = "446"
                },
                new()
                {
                    Id = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                    UserId = UserId,
                    Amount = 55.45m,
                    Currency = "EUR",
                    CreatedAt = DateTime.Parse("2021-02-08T12:41:28.595Z"),
                    ExpiryMonth = 1,
                    ExpiryYear = 2028,
                    PaymentStatus = "Approved",
                    CreditCardNumber = "1234-1234-1234-1234",
                    Cvv = "446"
                }
            };

        private static ClaimsPrincipal GetUserWithClaims(Guid userId)
        {
            var user = new ClaimsPrincipal();
            user.AddIdentity(
                new ClaimsIdentity(new List<Claim>
                {
                    new("userId", userId.ToString())
                }));
            return user;
        }
    }
}

namespace PaymentAPI.Tests.Domain.Services
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading;
    using System;

    using FluentAssertions;

    using Moq;

    using PaymentAPI.Domain.Repositories;
    using PaymentAPI.Domain.Services;
    using PaymentAPI.Domain;

    using Xunit;

    public class PaymentServiceTests
    {
        private readonly Mock<IRepository<Payment>> _paymentRepositoryMock;
        private readonly PaymentService _service;

        public PaymentServiceTests()
        {
            _paymentRepositoryMock = new Mock<IRepository<Payment>>();
            _service = new PaymentService(_paymentRepositoryMock.Object);
        }

        [Fact]
        public async Task ShouldGetUserPayments()
        {
            // Arrange
            _paymentRepositoryMock.Setup(x => x.FilterByAsync(It.IsAny<Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(GetDomainPayments());

            // Act
            var payments = await _service.GetUserPaymentsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>());

            // Assert
            payments.Should().BeEquivalentTo(GetDomainPayments());
        }

        [Fact]
        public async Task ShouldGetUserPayment()
        {
            // Arrange
            _paymentRepositoryMock.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(GetDomainPayments().First());

            // Act
            var payment = await _service.GetUserPaymentAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>());

            // Assert
            payment.Should().BeEquivalentTo(GetDomainPayments().First());
        }

        [Fact]
        public async Task ShouldCreatePayment()
        {
            // Arrange
            _paymentRepositoryMock.Setup(x => x.InsertOneAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(GetDomainPayments().First());

            // Act
            var payment = await _service.CreateUserPaymentAsync(It.IsAny<Guid>(), new Payment(), It.IsAny<CancellationToken>());

            // Assert
            payment.Should().BeEquivalentTo(GetDomainPayments().First());
        }


        private static IEnumerable<Payment> GetDomainPayments() =>
            new Payment[]
            {
                new()
                {
                    Id = new Guid("a2b6606e-258c-4a5a-b4df-c8f25a0afe48"),
                    UserId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
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
                    UserId = new Guid("a2b6606e-258c-4a5a-b4df-c8f25a0afe48"),
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
    }
}

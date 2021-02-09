namespace PaymentAPI.Tests.Domain.Services
{
    using System.Net.Http;
    using System.Net;
    using System.Threading.Tasks;
    using System.Threading;
    using System;

    using FluentAssertions;

    using Moq.Protected;
    using Moq;

    using PaymentAPI.Domain.Repositories;
    using PaymentAPI.Domain.Services;
    using PaymentAPI.Domain;

    using Xunit;

    public class BankingServiceTests
    {
        [Fact]
        public async Task ShouldProcessPaymentSuccessfully()
        {
            // Arrange
            var paymentRepositoryMock = new Mock<IRepository<Payment>>();

            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"{ ""paymentId"": ""aeb9fd74-fb35-4767-82bf-4dbdff7200f9"", ""paymentStatus"": ""Declined""}"),
                });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new UriBuilder("http://address").Uri
            };

            var service = new BankingService(httpClient, paymentRepositoryMock.Object);

            // Act
            var processedPayment = await service.ProcessPaymentAsync(new Payment(), "/endpoint", It.IsAny<CancellationToken>());

            // Assert
            processedPayment.PaymentStatus.Should().Be("Declined");
            paymentRepositoryMock.Verify(x => x.ReplaceOneAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Once());
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}

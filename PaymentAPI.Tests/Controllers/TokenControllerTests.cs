namespace PaymentAPI.Tests.Controllers
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading;
    using System;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using FluentAssertions;

    using Models;

    using Moq;

    using PaymentAPI.Controllers;
    using PaymentAPI.Domain.Repositories;
    using PaymentAPI.Domain.Services;
    using PaymentAPI.Domain;

    using Xunit;

    public class TokenControllerTests
    {
        private readonly Mock<ILogger<TokenController>> _loggerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IRepository<User>> _userRepositoryMock;
        private readonly TokenController _controller;

        public TokenControllerTests()
        {
            _loggerMock = new Mock<ILogger<TokenController>>();
            _configurationMock = new Mock<IConfiguration>();
            _userRepositoryMock = new Mock<IRepository<User>>();
            _tokenServiceMock = new Mock<ITokenService>();
            _controller = new TokenController(_loggerMock.Object, _configurationMock.Object, _userRepositoryMock.Object, _tokenServiceMock.Object);
        }

        [Fact]
        public async Task Create_Returns400IfCredentialsAreMalformed()
        {
            // Arrange
            var request = new TokenRequest
            {
                Credentials = "THIS IS NOT HOW THEY SHOULD BE DONE"
            };

            // Act
            var response = await _controller.CreateTokenAsync(request) as BadRequestObjectResult;

            // Assert
            response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            response.Value.Should().Be("Invalid credentials");
        }

        [Fact]
        public async Task Create_Returns200WithToken()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryMock
                .Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User
                {
                    Id = userId
                });

            _tokenServiceMock.Setup(x => x.CreateSecurityToken(userId, It.IsAny<string>(), It.IsAny<string>())).Returns(new JwtSecurityToken());
            _tokenServiceMock.Setup(x => x.WriteSecurityToken(It.IsAny<JwtSecurityToken>())).Returns("MY TOKEN");

            var request = new TokenRequest
            {
                Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("thiago:abc1234"))
            };

            // Act
            var response = await _controller.CreateTokenAsync(request) as OkObjectResult;

            // Assert
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            response.Value.Should().BeEquivalentTo(new TokenResponse
            {
                Token = "MY TOKEN"
            });
        }
    }
}

namespace PaymentAPI.Tests.Domain.Services
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System;

    using FluentAssertions;

    using PaymentAPI.Domain.Services;

    using Xunit;

    public class TokenServiceTests
    {
        [Fact]
        public void ShouldCreateSecurityTokenSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var issuer = "issuer";
            var secret = "secret";
            var service = new TokenService();

            // Act
            var securityToken = service.CreateSecurityToken(userId, secret, issuer);

            // Assert
            securityToken.ValidTo.Should().BeCloseTo(DateTime.Now.AddDays(1), 5000);
            var claim = securityToken.Claims.FirstOrDefault(x => x.Type.Equals("userid"));
            claim.Value.Should().Be(userId.ToString());
        }

        [Fact]
        public void ShouldWriteSecurityTokenSuccessfully()
        {
            var service = new TokenService();

            // Act
            var token = service.WriteSecurityToken(new JwtSecurityToken());

            // Assert
            token.Should().NotBeEmpty();
        }
    }
}

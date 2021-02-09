namespace PaymentAPI.Domain.Services
{
    using System;
    using System.IdentityModel.Tokens.Jwt;

    public interface ITokenService
    {
        JwtSecurityToken CreateSecurityToken(Guid userId, string jwtSecret, string jwtIssuer);
        string WriteSecurityToken(JwtSecurityToken jwtSecurityToken);
    }
}

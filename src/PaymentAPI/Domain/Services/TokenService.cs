namespace PaymentAPI.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    using Microsoft.IdentityModel.Tokens;

    public class TokenService : ITokenService
    {
        public JwtSecurityToken CreateSecurityToken(Guid userId, string jwtSecret, string jwtIssuer)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var issuer = jwtIssuer;
            return new JwtSecurityToken(
                issuer: issuer,
                audience: issuer,
                claims: new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), new("userid", userId.ToString())
                },
                notBefore: null,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);
        }

        public string WriteSecurityToken(JwtSecurityToken jwtSecurityToken)
        {
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
    }
}

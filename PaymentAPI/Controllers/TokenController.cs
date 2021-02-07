namespace PaymentAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Models;
    using PaymentAPI.Domain.Repositories;
    public static class Extensions
    {
        public static void Deconstruct<T>(this IList<T> list, out T first, out IList<T> rest)
        {
            first = list.Count > 0 ? list[0] : default; // or throw
            rest = list.Skip(1).ToList();
        }

        public static void Deconstruct<T>(this IList<T> list, out T first, out T second, out IList<T> rest)
        {
            first = list.Count > 0 ? list[0] : default; // or throw
            second = list.Count > 1 ? list[1] : default; // or throw
            rest = list.Skip(2).ToList();
        }
    }


    [ApiController]
    [Route("tokens")]
    public class TokenController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Domain.User> _userRepository;

        public TokenController(ILogger<PaymentController> logger, IConfiguration configuration, IRepository<Domain.User> userRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _userRepository = userRepository;
        }

        [HttpPost()]
        public async Task<string> CreateToken(TokenRequest tokenRequest)
        {
            var data = Convert.FromBase64String(tokenRequest.Credentials);
            var credentialsDecoded = Encoding.UTF8.GetString(data);

            var (username, password, _) = credentialsDecoded.Split(':');

            Domain.User user = await _userRepository.FindOneAsync(x => x.Username.Equals(username) && x.Password.Equals(password));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSecret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var permClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userid", user.Id.ToString())
            };

            var issuer = _configuration["JWTIssuer"];
            var token = new JwtSecurityToken(issuer,
                issuer,
                permClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

namespace PaymentAPI.Controllers
{
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading;
    using System;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using Domain.Repositories;
    using Domain.Services;

    using Models;

    [ApiController]
    [Route("tokens")]
    public class TokenController : ControllerBase
    {
        private readonly ILogger<TokenController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Domain.User> _userRepository;
        private readonly ITokenService _tokenService;

        public TokenController(ILogger<TokenController> logger, IConfiguration configuration,
            IRepository<Domain.User> userRepository, ITokenService tokenService)
        {
            _logger = logger;
            _configuration = configuration;
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        [HttpPost()]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTokenAsync(TokenRequest tokenRequest, CancellationToken ct = default)
        {
            try
            {
                var encodedCredentials = Convert.FromBase64String(tokenRequest.Credentials);
                var decodedCredentials = Encoding.UTF8.GetString(encodedCredentials);
                var credentials = decodedCredentials.Split(':');

                var user = await _userRepository.FindOneAsync(x =>
                    x.Username.Equals(credentials[0]) &&
                    x.Password.Equals(credentials[1]), ct);

                var securityToken = _tokenService.CreateSecurityToken(
                    user.Id, _configuration["JWTSecret"], _configuration["JWTIssuer"]);

                return Ok(new TokenResponse
                {
                    Token = _tokenService.WriteSecurityToken(securityToken)
                });
            }
            catch (Exception)
            {
                return BadRequest("Invalid credentials");
            }
        }
    }
}

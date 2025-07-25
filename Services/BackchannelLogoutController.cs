namespace PRASARNET.Services
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;

    [Route("backchannel-logout")]
    [ApiController]
    public class BackchannelLogoutController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BackchannelLogoutController> _logger;
        private readonly ISidStore _sidStore;

        public BackchannelLogoutController(SessionService sessionService, IConfiguration configuration, ILogger<BackchannelLogoutController> logger, ISidStore sidStore)
        {
            _sessionService = sessionService;
            _configuration = configuration;
            _logger = logger;
            _sidStore = sidStore;
        }

        [HttpPost]
        public async Task<IActionResult> Logout([FromForm] string logout_token)
        {
            if (string.IsNullOrWhiteSpace(logout_token))
            {
                _logger.LogWarning("Missing logout_token in backchannel request.");
                return BadRequest("Missing logout_token");
            }

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Keycloak:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = _configuration["Keycloak:ClientId"],

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(2),

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = await GetSigningKeysAsync()
                };

                var principal = handler.ValidateToken(logout_token, validationParameters, out _);

                var sid = principal.FindFirst("sid")?.Value;
                if (!string.IsNullOrEmpty(sid))
                {
                    await _sidStore.InvalidateSidAsync(sid);
                    _logger.LogInformation($"SID {sid} invalidated via backchannel logout.");
                    return Ok(); // SUCCESS!
                }

                _logger.LogWarning("Logout_token does not contain sid or sub.");
                return BadRequest("Invalid logout_token claims");
            }
            catch (SecurityTokenException stex)
            {
                _logger.LogWarning(stex, "Security token validation failed");
                return Unauthorized("Invalid logout_token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during logout");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<IEnumerable<SecurityKey>> GetSigningKeysAsync()
        {
            try
            {
                var discoveryEndpoint = _configuration["Keycloak:DiscoveryEndpoint"];

                if (string.IsNullOrWhiteSpace(discoveryEndpoint) || !discoveryEndpoint.StartsWith("http"))
                    throw new InvalidOperationException("DiscoveryEndpoint must be an absolute URI.");

                using var client = new HttpClient();
                var jwksUri = $"{discoveryEndpoint.TrimEnd('/')}/protocol/openid-connect/certs";

                var json = await client.GetStringAsync(jwksUri);
                var jwks = new JsonWebKeySet(json);

                return jwks.Keys;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch or parse signing keys");
                throw;
            }
        }
    }


}

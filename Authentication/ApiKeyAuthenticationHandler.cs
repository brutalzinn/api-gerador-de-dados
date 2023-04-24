using GeradorDeDados.Models.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace GeradorDeDados.Authentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private ApiConfig _apiConfig { get; set; }
        public ApiKeyAuthenticationHandler(IOptions<ApiConfig> apiConfig, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _apiConfig = apiConfig.Value;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "LOGADO por API KEY") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            if (_apiConfig.Authorization.Activate == false)
            {
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            if (Request.Headers.TryGetValue(_apiConfig.Authorization.ApiHeader, out
                   var extractedApiKey) == false)
            {
                return Task.FromResult(AuthenticateResult.Fail("Api key não informada"));
            }

            if (_apiConfig.Authorization.ApiKey.Equals(extractedApiKey) == false)
            {
                return Task.FromResult(AuthenticateResult.Fail("Não autorizado"));
            }

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}

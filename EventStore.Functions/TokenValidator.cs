using System.Security.Claims;
using System.Threading.Tasks;
using Numaka.Functions.Infrastructure;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using Numaka.Common;
using System.Threading;
using Numaka.Common.Exceptions;

namespace EventStore.Functions
{
    public class TokenValidator : ITokenValidator
    {
        private readonly TokenValidationParameters _tokenValidationParameters;

        public TokenValidator(string metadataAddress, string clientId)
        {
            if (string.IsNullOrWhiteSpace(metadataAddress)) throw new ArgumentNullException(nameof(metadataAddress));
            if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentNullException(nameof(clientId));

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(metadataAddress, new OpenIdConnectConfigurationRetriever());
            var configuration = AsyncHelper.RunSync(async () => await configurationManager.GetConfigurationAsync(CancellationToken.None));

            _tokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidAudience = clientId,
                    ValidIssuer = configuration.Issuer,
                    IssuerSigningKeys = configuration.SigningKeys,
                    ValidateIssuerSigningKey = true,
                };
        }

        public Task<ClaimsPrincipal> ValidateAsync(string bearerToken)
        {
            try
            {
                var claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(bearerToken, _tokenValidationParameters, out _);

                return Task.FromResult(claimsPrincipal);
            }
            catch (Exception e)
            {
                throw new TokenValidationException("Failed to validate bearer token", e);
            }
        }
    }
}
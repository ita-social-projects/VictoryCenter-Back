using System.Text;
using Microsoft.IdentityModel.Tokens;
using VictoryCenter.BLL.Options;

namespace VictoryCenter.BLL.Helpers;

public static class AuthHelper
{
    private static readonly TimeSpan ClockSkew = TimeSpan.FromSeconds(30);

    public static TokenValidationParameters GetTokenValidationParameters(JwtOptions options)
    {
        return new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireSignedTokens = true,
            RequireExpirationTime = true,
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            ClockSkew = ClockSkew,
            ValidAudience = options.Audience,
            ValidIssuer = options.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey))
        };
    }
}

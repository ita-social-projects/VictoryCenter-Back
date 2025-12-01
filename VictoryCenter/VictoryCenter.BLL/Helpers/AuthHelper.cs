using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace VictoryCenter.BLL.Helpers;

public static class AuthHelper
{
    public static TokenValidationParameters GetTokenValidationParameters(IConfiguration configuration)
    {
        return new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = configuration.GetValue<string>("JwtOptions:Audience") ?? throw new InvalidOperationException("Audience for jwt options is not specified"),
            ValidIssuer = configuration.GetValue<string>("JwtOptions:Issuer") ?? throw new InvalidOperationException("Issuer for jwt options is not specified"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JwtOptions:SecretKey") ??
                                                                               throw new InvalidOperationException("Secret Key for jwt options is not specified")))
        };
    }
}

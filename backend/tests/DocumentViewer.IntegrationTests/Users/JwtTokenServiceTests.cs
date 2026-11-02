using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DocumentViewer.Contract.Options;
using DocumentViewer.Contract.Users.Admin;
using DocumentViewer.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace DocumentViewer.IntegrationTests.Users;

public class JwtTokenServiceTests
{
    [Fact]
    public void Issued_token_has_valid_signature_audience_issuer_and_role()
    {
        var options = new JwtOptions
        {
            Issuer = "DocumentViewer.Tests",
            Audience = "DocumentViewer.Tests.Api",
            SigningKey = "test-only-signing-key-with-at-least-32-bytes",
            AccessTokenLifetimeMinutes = 15
        };
        var service = new JwtTokenService(Options.Create(options));

        var token = service.CreateAccessToken(42, "superadmin", [AdminRoles.SuperAdmin]);
        var principal = new JwtSecurityTokenHandler().ValidateToken(token.Value, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = options.Issuer,
            ValidateAudience = true,
            ValidAudience = options.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        }, out _);

        Assert.Equal("42", principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        Assert.Equal(AdminRoles.SuperAdmin, principal.FindFirst(ClaimTypes.Role)?.Value);
        Assert.True(token.ExpiresAtUtc > DateTimeOffset.UtcNow);
    }
}

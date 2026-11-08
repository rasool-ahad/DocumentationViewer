using System.IdentityModel.Tokens.Jwt;
using System.Text;
using DocumentViewer.Contract.Options;
using DocumentViewer.Contract.Users.Admin.Services;
using DocumentViewer.Domain.Users;
using Framework.DDD.Domain.Contract.Commons.ConfigurationModels;
using Framework.DDD.Presentation.Injections;
using Framework.DDD.Presentation.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DocumentViewer.WebApi;

public static class Bootstrapper
{
    public static WebApplicationBuilder AddDocumentViewerPresentation(this WebApplicationBuilder builder)
    {
        var section = builder.Configuration.GetSection("Presentation");
        builder.Services.AddOptions<PresentationOptions>().Bind(section);
        builder.AddPresentationFramework();
        var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var tokenId = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                        if (string.IsNullOrWhiteSpace(tokenId))
                        {
                            context.Fail("The access token has no token ID.");
                            return;
                        }

                        var revocations = context.HttpContext.RequestServices.GetRequiredService<IAccessTokenRevocationService>();
                        if (await revocations.IsRevokedAsync(tokenId))
                        {
                            context.Fail("The access token has been revoked.");
                            return;
                        }

                        var userId = context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                        var stamp = context.Principal?.FindFirst("security_stamp")?.Value;
                        var users = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
                        var user = userId is null ? null : await users.FindByIdAsync(userId);
                        if (user is null || !user.IsActive || user.Blocked || user.IsDeleted ||
                            string.IsNullOrEmpty(stamp) || stamp != await users.GetSecurityStampAsync(user))
                        {
                            context.Fail("The access token is no longer valid.");
                            return;
                        }

                        var currentRoles = await users.GetRolesAsync(user);
                        var tokenRoles = context.Principal!.FindAll(System.Security.Claims.ClaimTypes.Role)
                            .Select(claim => claim.Value).OrderBy(role => role).ToArray();
                        if (!currentRoles.OrderBy(role => role).SequenceEqual(tokenRoles))
                            context.Fail("The user's roles have changed.");
                    }
                };
            });
        builder.Services.AddAuthorization();
        return builder;
    }

    public static WebApplication UseDocumentViewerPresentation(this WebApplication app)
    {
        var presentation = app.Services.GetRequiredService<IOptions<PresentationOptions>>().Value;
        if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Debug"))
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var doc in presentation.SwaggerDocs)
                    options.SwaggerEndpoint($"/swagger/{doc.Name}/swagger.json", doc.Title);
            });
        }

        app.UseHttpsRedirection();
        app.UseApiResponseMiddleware();
        app.UseApiRequestMiddleware();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        return app;
    }
}


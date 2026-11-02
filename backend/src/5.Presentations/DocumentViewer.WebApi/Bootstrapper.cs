using System.Text;
using DocumentViewer.Contract.Options;
using Framework.DDD.Domain.Contract.Commons.ConfigurationModels;
using Framework.DDD.Presentation.Injections;
using Framework.DDD.Presentation.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
            });
        builder.Services.AddAuthorization();
        return builder;
    }

    public static WebApplication UseDocumentViewerPresentation(this WebApplication app)
    {
        var presentation = app.Services.GetRequiredService<IOptions<PresentationOptions>>().Value;
        if (app.Environment.IsDevelopment())
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


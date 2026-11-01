using Framework.DDD.Domain.Contract.Commons.ConfigurationModels;
using Framework.DDD.Presentation.Injections;
using Framework.DDD.Presentation.Middlewares;
using Microsoft.Extensions.Options;

namespace DocumentViewer.WebApi;

public static class Bootstrapper
{
    public static WebApplicationBuilder AddDocumentViewerPresentation(this WebApplicationBuilder builder)
    {
        var section = builder.Configuration.GetSection("Presentation");
        builder.Services.AddOptions<PresentationOptions>().Bind(section);
        builder.AddPresentationFramework();
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
        app.UseAuthorization();
        app.MapControllers();
        return app;
    }
}


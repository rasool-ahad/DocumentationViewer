using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Debug"))
{
    app.UseSwaggerUI(options =>
    {
        options.DocumentTitle = "DocumentViewer Admin Gateway API";
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Admin API");
    });
}
await app.UseOcelot();
await app.RunAsync();


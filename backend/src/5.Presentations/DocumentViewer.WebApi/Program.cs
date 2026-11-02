using DocumentViewer.Application;
using DocumentViewer.Contract.Options;
using DocumentViewer.Infrastructure;
using DocumentViewer.Infrastructure.Sql.Commons.Context;
using DocumentViewer.Infrastructure.Sql.Users;
using DocumentViewer.WebApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDocumentViewerApplication();
builder.Services.AddDocumentViewerInfrastructure(builder.Configuration);
builder.AddDocumentViewerPresentation();

var app = builder.Build();
_ = app.Services.GetRequiredService<IOptions<JwtOptions>>().Value;
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<DocumentViewerIdentityDbContext>();
    await db.Database.MigrateAsync();
}
await app.Services.SeedSuperAdminAsync();
app.UseDocumentViewerPresentation();
app.Run();

public partial class Program;


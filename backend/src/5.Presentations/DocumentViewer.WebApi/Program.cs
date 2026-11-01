using DocumentViewer.Application;
using DocumentViewer.Infrastructure;
using DocumentViewer.WebApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDocumentViewerApplication();
builder.Services.AddDocumentViewerInfrastructure(builder.Configuration);
builder.AddDocumentViewerPresentation();

var app = builder.Build();
app.UseDocumentViewerPresentation();
app.Run();

public partial class Program;


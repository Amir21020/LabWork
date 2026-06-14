using Serilog;
using TimeTrackingApp.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddPresentationServices(config);
builder.Services.AddBusinessLogic();
builder.Services.AddDataAccess(config);

var app = builder.Build();

app.UseSwaggerDocs();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCustomRequestLogging();

app.UseCors();

app.MapControllers();

app.Run();

public partial class Program { }
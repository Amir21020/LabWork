using Serilog;
using TimeTrackingApp.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddPresentationServices(builder.Configuration);

var app = builder.Build();

app.UseSwaggerDocs();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCustomRequestLogging();

app.MapControllers();

app.Run();

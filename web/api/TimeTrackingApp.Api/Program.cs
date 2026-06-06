using Serilog;
using TimeTrackingApp.Api.Extensions;
using TimeTrackingApp.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddPresentationServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty; 
    });
}

app.UseSerilogRequestLogging();
app.UseMiddleware<RequestContextLoggingMiddleware>();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

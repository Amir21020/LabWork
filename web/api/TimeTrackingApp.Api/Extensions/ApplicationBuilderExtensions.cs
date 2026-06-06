using Serilog;
using TimeTrackingApp.Api.Middlewares;

namespace TimeTrackingApp.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseSwaggerDocs(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty; 
            });
        }

        return app;
    }

    public static WebApplication UseCustomRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseMiddleware<RequestContextLoggingMiddleware>();
        return app;
    }
}

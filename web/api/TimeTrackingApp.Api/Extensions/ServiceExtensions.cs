using TimeTrackingApp.Api.Exceptions;

namespace TimeTrackingApp.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddPresentationServices(
        this IServiceCollection services, IConfiguration configuration)
    {

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
            {
                Title = "TimeTracker API",
                Version = "v1",
                Description = "API для веб-приложения учета рабочего времени сотрудников компании",
                Contact = new Microsoft.OpenApi.OpenApiContact
                {
                    Name = "Radmir Mergaliev",
                    Url = new Uri("https://t.me/Mergalievr")
                }
            });
        });

        return services;
    }
}

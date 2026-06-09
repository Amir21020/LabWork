using Microsoft.EntityFrameworkCore;
using TimeTrackingApp.Api.Exceptions;
using TimeTrackingApp.BL.Interfaces;
using TimeTrackingApp.BL.Services;
using TimeTrackingApp.DAL.Data;
using TimeTrackingApp.DAL.Entities;
using TimeTrackingApp.DAL.Interfaces;
using TimeTrackingApp.DAL.Repositories;

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

    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        return services;
    }

    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
        services.AddScoped<IBaseRepository<ProjectEntity>, BaseRepository<ProjectEntity>>(); 
        return services;
    }
}

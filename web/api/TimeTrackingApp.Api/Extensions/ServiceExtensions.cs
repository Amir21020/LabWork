using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTrackingApp.Api.Exceptions;
using TimeTrackingApp.Api.Filters;
using TimeTrackingApp.BL.Interfaces;
using TimeTrackingApp.BL.Services;
using TimeTrackingApp.BL.Validators;
using TimeTrackingApp.DAL.Data;
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
        services.AddControllers(options =>
        {
            options.Filters.Add<ValidationFilter>();
        });
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
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ITimeEntryService, TimeEntryService>();
        services.AddValidatorsFromAssembly(typeof(CreateProjectRequestValidator).Assembly);

        return services;
    }

    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITimeEntryRepository, TimeEntryRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        return services;
    }
}

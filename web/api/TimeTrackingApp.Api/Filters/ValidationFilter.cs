using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TimeTrackingApp.Api.Filters;

public sealed class ValidationFilter(ILogger<ValidationFilter> logger) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var serviceProvider = context.HttpContext.RequestServices;

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            var validator = serviceProvider.GetService(validatorType) as IValidator;

            if (validator is null) continue;

            ValidationResult result = await validator.ValidateAsync(new ValidationContext<object>(argument));
            
            if (!result.IsValid)
            {
                logger.LogWarning(
                    "Validation failed for request model '{ModelType}' at path '{Path}'. Found {ErrorCount} validation error(s).",
                    argument.GetType().Name,
                    context.HttpContext.Request.Path,
                    result.Errors.Count);

                var errors = result.Errors
                    .Select(e => new { field = e.PropertyName, message = e.ErrorMessage });

                context.Result = new BadRequestObjectResult(new
                {
                    Status = "ValidationFailed",
                    Errors = errors
                });

                return;
            }
        }
        await next();
    }
}

using FluentValidation;
using TimeTrackingApp.BL.DTOs;

namespace TimeTrackingApp.BL.Validators;

public sealed class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Название задачи обязательно для заполнения.")
            .MaximumLength(200)
            .WithMessage("Название задачи не должно превышать 200 символов.");
    }
}

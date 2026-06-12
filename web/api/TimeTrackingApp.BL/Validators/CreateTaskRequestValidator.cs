using FluentValidation;
using TimeTrackingApp.BL.DTOs;

namespace TimeTrackingApp.BL.Validators;

public sealed class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Название задачи обязательно для заполнения.")
            .MaximumLength(200)
            .WithMessage("Название задачи не должно превышать 200 символов.");
    }
}

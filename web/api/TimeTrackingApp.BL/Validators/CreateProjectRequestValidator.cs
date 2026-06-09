using FluentValidation;
using TimeTrackingApp.BL.DTOs;

namespace TimeTrackingApp.BL.Validators;

public sealed class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Название проекта обязательно для заполнения.")
            .MaximumLength(150)
            .WithMessage("Название проекта не должно превышать 150 символов.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Код проекта обязателен для заполнения.")
            .MaximumLength(50)
            .WithMessage("Код проекта не должен превышать 50 символов.")
            .Matches(@"^[A-Z0-9\-_]+$")
            .WithMessage("Код проекта может содержать только заглавные латинские буквы, цифры, дефис и нижнее подчеркивание (без пробелов).");
    }
}
using FluentValidation;
using TimeTrackingApp.BL.DTOs;

namespace TimeTrackingApp.BL.Validators;

public sealed class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequest>
{
    public UpdateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название проекта обязательно для заполнения.")
            .MaximumLength(100).WithMessage("Название проекта не должно превышать 100 символов.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Код проекта обязателен для заполнения.")
            .MaximumLength(10).WithMessage("Код проекта не должен превышать 10 символов.")
            .Matches(@"^[A-Z0-9\-_]+$").WithMessage("Код проекта может содержать только заглавные латинские буквы, цифры, дефис и нижнее подчеркивание (без пробелов).");
    }
}
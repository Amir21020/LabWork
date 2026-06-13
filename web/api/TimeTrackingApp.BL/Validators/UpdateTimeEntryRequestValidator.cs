using FluentValidation;
using TimeTrackingApp.BL.DTOs;

namespace TimeTrackingApp.BL.Validators;

public sealed class UpdateTimeEntryRequestValidator : AbstractValidator<UpdateTimeEntryRequest>
{
    public UpdateTimeEntryRequestValidator()
    {
        RuleFor(t => t.Hours)
            .GreaterThan(0)
            .LessThanOrEqualTo(24)
            .WithMessage("Количество часов должно быть положительным числом, не более 24.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Описание обязательно для заполнения.")
            .MaximumLength(500).WithMessage("Описание не должно превышать 500 символов.")
            .Must(desc => desc == null || (!desc.Contains('\r') && !desc.Contains('\n')))
            .WithMessage("Описание должно быть однострочным (без переносов строк).");
    }
}

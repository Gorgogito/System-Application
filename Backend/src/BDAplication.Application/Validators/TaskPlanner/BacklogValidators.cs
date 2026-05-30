using BDAplication.Application.DTOs.TaskPlanner;
using FluentValidation;

namespace BDAplication.Application.Validators.TaskPlanner;

public class BacklogRegisterValidator : AbstractValidator<BacklogRegisterRequest>
{
    public BacklogRegisterValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Priority).IsInEnum();
    }
}

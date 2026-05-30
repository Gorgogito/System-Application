using BDAplication.Application.DTOs.TaskPlanner;
using FluentValidation;

namespace BDAplication.Application.Validators.TaskPlanner;

public class PlannerRegisterValidator : AbstractValidator<PlannerRegisterRequest>
{
    public PlannerRegisterValidator()
    {
        RuleFor(x => x.BacklogId).GreaterThan(0);
        RuleFor(x => x.Day).InclusiveBetween(1, 31);
        RuleFor(x => x.Month).InclusiveBetween(1, 12);
        RuleFor(x => x.Year).InclusiveBetween(2000, 2100);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class MovePlannerValidator : AbstractValidator<MovePlannerRequest>
{
    public MovePlannerValidator()
    {
        RuleFor(x => x.PlannerId).GreaterThan(0);
        RuleFor(x => x.NewDay).InclusiveBetween(1, 31);
        RuleFor(x => x.NewMonth).InclusiveBetween(1, 12);
        RuleFor(x => x.NewYear).InclusiveBetween(2000, 2100);
    }
}

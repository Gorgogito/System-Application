using BDAplication.Application.DTOs.TaskBoard;
using FluentValidation;

namespace BDAplication.Application.Validators.TaskBoard;

public class CreateTaskBoardValidator : AbstractValidator<CreateTaskBoardRequest>
{
    public CreateTaskBoardValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.Priority).IsInEnum();
    }
}

public class UpdateTaskBoardValidator : AbstractValidator<UpdateTaskBoardRequest>
{
    public UpdateTaskBoardValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.Priority).IsInEnum();
    }
}

public class MoveTaskBoardValidator : AbstractValidator<MoveTaskBoardRequest>
{
    public MoveTaskBoardValidator()
    {
        RuleFor(x => x.TaskId).GreaterThan(0);
        RuleFor(x => x.Status).IsInEnum();
    }
}

using BDAplication.Application.DTOs.Finance;
using FluentValidation;

namespace BDAplication.Application.Validators.Finance;

public class CreateAccountValidator : AbstractValidator<CreateAccountRequest>
{
    public CreateAccountValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(300);
    }
}

public class UpdateAccountValidator : AbstractValidator<UpdateAccountRequest>
{
    public UpdateAccountValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(300);
    }
}

public class CreateTypeConceptValidator : AbstractValidator<CreateTypeConceptRequest>
{
    public CreateTypeConceptValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(100);
    }
}

public class UpdateTypeConceptValidator : AbstractValidator<UpdateTypeConceptRequest>
{
    public UpdateTypeConceptValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(100);
    }
}

public class CreateMovementValidator : AbstractValidator<CreateMovementRequest>
{
    public CreateMovementValidator()
    {
        RuleFor(x => x.AccountId).GreaterThan(0);
        RuleFor(x => x.Concept).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Type).Must(t => t == "I" || t == "O").WithMessage("Type must be 'I' or 'O'");
    }
}

public class UpdateMovementValidator : AbstractValidator<UpdateMovementRequest>
{
    public UpdateMovementValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Concept).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

public class CreateTransferValidator : AbstractValidator<CreateTransferRequest>
{
    public CreateTransferValidator()
    {
        RuleFor(x => x.SourceAccountId).GreaterThan(0);
        RuleFor(x => x.DestinyAccountId).GreaterThan(0)
            .NotEqual(x => x.SourceAccountId).WithMessage("Las cuentas origen y destino deben ser diferentes.");
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Concept).NotEmpty().MaximumLength(300);
    }
}

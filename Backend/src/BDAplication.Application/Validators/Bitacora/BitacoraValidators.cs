using BDAplication.Application.DTOs.Bitacora;
using FluentValidation;

namespace BDAplication.Application.Validators.Bitacora;

public class UpdateBitacoraValidator : AbstractValidator<UpdateBitacoraRequest>
{
    public UpdateBitacoraValidator()
    {
        RuleFor(x => x.Observacion).MaximumLength(1000);
    }
}

public class CreateActividadValidator : AbstractValidator<CreateActividadRequest>
{
    public CreateActividadValidator()
    {
        RuleFor(x => x.BitacoraId).GreaterThan(0);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(300);
        RuleFor(x => x.HoraFin).GreaterThan(x => x.HoraInicio)
            .WithMessage("La hora de fin debe ser posterior a la hora de inicio");
    }
}

public class UpdateActividadValidator : AbstractValidator<UpdateActividadRequest>
{
    public UpdateActividadValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(300);
        RuleFor(x => x.HoraFin).GreaterThan(x => x.HoraInicio)
            .WithMessage("La hora de fin debe ser posterior a la hora de inicio");
    }
}

public class GetResumenValidator : AbstractValidator<GetResumenRequest>
{
    public GetResumenValidator()
    {
        RuleFor(x => x.Hasta).GreaterThanOrEqualTo(x => x.Desde)
            .WithMessage("La fecha 'hasta' debe ser posterior o igual a 'desde'");
    }
}

public class SasUploadValidator : AbstractValidator<SasUploadRequest>
{
    public SasUploadValidator()
    {
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x.DeclaredSizeBytes).GreaterThan(0);
    }
}

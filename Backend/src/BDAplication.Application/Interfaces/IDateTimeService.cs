namespace BDAplication.Application.Interfaces;

public interface IDateTimeService
{
    /// <summary>Hora actual en UTC.</summary>
    DateTime UtcNow { get; }

    /// <summary>Convierte una fecha UTC a la zona horaria indicada.</summary>
    DateTime ConvertToTimeZone(DateTime utcDateTime, string timeZoneId);

    /// <summary>Atajo: convierte una fecha UTC a la hora de Lima, Perú (UTC-5).</summary>
    DateTime ToLimaTime(DateTime utcDateTime);
}

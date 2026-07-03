namespace BDAplication.Web.Services;

public interface IUserTimeZoneService
{
    /// <summary>ID de zona horaria Windows del usuario actual.</summary>
    string TimeZoneId { get; }

    /// <summary>Fecha y hora actual en la zona horaria del usuario.</summary>
    DateTime UserNow { get; }

    /// <summary>Fecha de "hoy" en la zona horaria del usuario (sin hora).</summary>
    DateTime UserToday { get; }

    /// <summary>Convierte una fecha UTC a la zona horaria del usuario.</summary>
    DateTime ToUserTime(DateTime utcDateTime);
}

public class UserTimeZoneService : IUserTimeZoneService
{
    // Lima, Perú (UTC-5, sin horario de verano).
    // En el futuro: leer este valor del perfil del usuario autenticado.
    private const string DefaultTimeZoneId = "SA Pacific Standard Time";

    private static readonly TimeZoneInfo Zone =
        TimeZoneInfo.FindSystemTimeZoneById(DefaultTimeZoneId);

    public string TimeZoneId => DefaultTimeZoneId;

    public DateTime UserNow
        => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zone);

    public DateTime UserToday => UserNow.Date;

    public DateTime ToUserTime(DateTime utcDateTime)
        => TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc), Zone);
}

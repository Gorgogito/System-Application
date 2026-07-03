using BDAplication.Application.Interfaces;

namespace BDAplication.Infrastructure.Services;

public class DateTimeService : IDateTimeService
{
    private const string LimaTimeZoneId = "SA Pacific Standard Time";

    private static readonly TimeZoneInfo LimaZone =
        TimeZoneInfo.FindSystemTimeZoneById(LimaTimeZoneId);

    public DateTime UtcNow => DateTime.UtcNow;

    public DateTime ConvertToTimeZone(DateTime utcDateTime, string timeZoneId)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc), tz);
    }

    public DateTime ToLimaTime(DateTime utcDateTime)
        => TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc), LimaZone);
}

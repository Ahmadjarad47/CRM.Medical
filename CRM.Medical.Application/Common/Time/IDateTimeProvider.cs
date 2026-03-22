namespace CRM.Medical.Application.Common.Time;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}

namespace CRM.Medical.Application.Common.Time;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}

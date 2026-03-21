namespace CRM.Medical.Application.Configuration.Database;

public interface IDatabaseConnectionStringBuilder
{
    string Build(DatabaseSettings settings);
}

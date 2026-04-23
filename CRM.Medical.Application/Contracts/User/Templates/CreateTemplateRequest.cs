using System.Text.Json;

namespace CRM.Medical.API.Contracts.User.Templates;

public sealed record CreateTemplateRequest(
    string Name,
    JsonElement? Data);

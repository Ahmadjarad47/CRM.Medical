using System.Text.Json;

namespace CRM.Medical.API.Controllers.User.Models;

public sealed record CreateTemplateRequest(
    string Name,
    JsonElement? Data);


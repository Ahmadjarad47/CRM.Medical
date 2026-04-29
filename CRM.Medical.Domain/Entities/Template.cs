using System.Text.Json;

namespace CRM.Medical.Domain.Entities;

public sealed class Template : BaseEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public JsonDocument? Data { get; set; }

    public string Role { get; set; } = string.Empty;
}


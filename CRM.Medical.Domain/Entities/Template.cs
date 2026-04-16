using System.Text.Json;

namespace CRM.Medical.Domain.Entities;

public sealed class Template
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public JsonDocument? Data { get; set; }

    public string Role { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}


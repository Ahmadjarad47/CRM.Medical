using System.Text.Json;

namespace CRM.Medical.Domain.Entities;

public class UserNotification : BaseEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public JsonDocument? Data { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public User? User { get; set; }
}

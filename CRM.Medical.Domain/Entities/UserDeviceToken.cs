namespace CRM.Medical.Domain.Entities;

public class UserDeviceToken : BaseEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FcmToken { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public User? User { get; set; }
}

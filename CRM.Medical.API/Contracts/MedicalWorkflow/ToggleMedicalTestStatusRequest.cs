using CRM.Medical.Domain.Enums;

namespace CRM.Medical.API.Contracts.MedicalWorkflow;

public sealed class ToggleMedicalTestStatusRequest
{
    public MedicalTestStatus Status { get; set; }
}

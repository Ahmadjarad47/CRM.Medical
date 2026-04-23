namespace CRM.Medical.API.Contracts.MedicalWorkflow.AppointmentMedicalTest;

public sealed record UpdateAppointmentMedicalTestRequest
{
    public int? MedicalTestId { get; init; }
    public string? MedicalTestCompletionStatus { get; init; }
}

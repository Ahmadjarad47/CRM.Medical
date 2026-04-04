namespace CRM.Medical.Application.Features.Appointments;

public enum AppointmentConfirmationActor
{
    Admin,
    Doctor,
    LabPartner
}

public enum AppointmentCancellationActor
{
    Admin,
    Patient,
    Doctor,
    LabPartner
}

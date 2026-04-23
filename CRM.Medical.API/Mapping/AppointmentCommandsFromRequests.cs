using CRM.Medical.API.Contracts.Appointments;
using CRM.Medical.Application.Features.Appointments.Commands.AdminCreateAppointment;
using CRM.Medical.Application.Features.Appointments.Commands.DoctorCreateAppointment;
using CRM.Medical.Application.Features.Appointments.Commands.LabCreateAppointment;
using CRM.Medical.Application.Features.Appointments.Commands.PatientBookAppointment;

namespace CRM.Medical.API.Mapping;

public static class AppointmentCommandsFromRequests
{
    public static PatientBookAppointmentCommand ToPatientBook(string patientUserId, PatientBookAppointmentRequest request) =>
        new(patientUserId, request.ToFormFields(), request.DoctorId, request.LabPartnerId);

    public static DoctorCreateAppointmentCommand ToDoctorCreate(string doctorUserId, DoctorCreateAppointmentRequest request) =>
        new(doctorUserId, request.PatientId, request.LabPartnerId, request.ToFormFields());

    public static LabCreateAppointmentCommand ToLabCreate(string labUserId, LabCreateAppointmentRequest request) =>
        new(labUserId, request.PatientId, request.DoctorId, request.ToFormFields());

    public static AdminCreateAppointmentCommand ToAdminCreate(string adminUserId, AdminCreateAppointmentRequest request) =>
        new(adminUserId, request.PatientId, request.DoctorId, request.LabPartnerId, request.ToFormFields());
}

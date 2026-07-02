using CRM.Medical.Application.Configuration.S3;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace CRM.Medical.Application.Features.Appointments.CQRS;

public sealed class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator(IOptions<S3StorageSettings> s3Options)
    {
        var maxBytes = s3Options.Value.MaxAttachmentBytes;

        RuleFor(x => x.AvailabilityId)
            .GreaterThan(0);

        RuleFor(x => x.PatientLocationType)
            .NotEmpty();

        RuleFor(x => x.StartTime)
            .NotEmpty();

        RuleFor(x => x.EndTime)
            .NotEmpty();

        RuleFor(x => x.Notes)
            .MaximumLength(4000);

        RuleFor(x => x.Attachment)
            .Must(f => f == null || f.Length <= maxBytes)
            .WithMessage($"Attachment must not exceed {maxBytes} bytes.");
    }
}

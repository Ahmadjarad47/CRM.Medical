using CRM.Medical.Application.Features.Complaints;
using FluentValidation;

namespace CRM.Medical.Application.Features.Complaints.Commands.UpdateComplaintStatus;

public sealed class UpdateComplaintStatusCommandValidator : AbstractValidator<UpdateComplaintStatusCommand>
{
    public UpdateComplaintStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(s => ComplaintStatuses.All.Contains(s))
            .WithMessage(
                _ => $"Status must be one of: {string.Join(", ", ComplaintStatuses.All)}.");
    }
}

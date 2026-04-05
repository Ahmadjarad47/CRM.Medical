using CRM.Medical.Domain.Constants;
using FluentValidation;

namespace CRM.Medical.Application.Features.TestRequests.Commands.UpdateTestRequest;

public sealed class UpdateTestRequestCommandValidator : AbstractValidator<UpdateTestRequestCommand>
{
    public UpdateTestRequestCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Status)
            .NotEmpty()
            .MaximumLength(64)
            .Must(s => TestRequestStatuses.All.Contains(s))
            .WithMessage("Status must be a known test request lifecycle value.");
        RuleFor(x => x.TotalAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Notes).MaximumLength(4000);
    }
}

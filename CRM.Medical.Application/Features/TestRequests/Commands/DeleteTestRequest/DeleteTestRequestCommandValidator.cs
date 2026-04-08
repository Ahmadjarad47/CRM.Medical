using FluentValidation;

namespace CRM.Medical.Application.Features.TestRequests.Commands.DeleteTestRequest;

public sealed class DeleteTestRequestCommandValidator : AbstractValidator<DeleteTestRequestCommand>
{
    public DeleteTestRequestCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

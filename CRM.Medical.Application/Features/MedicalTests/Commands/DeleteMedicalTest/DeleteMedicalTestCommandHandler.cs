using CRM.Medical.Application.Exceptions;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.Commands.DeleteMedicalTest;

public sealed class DeleteMedicalTestCommandHandler(IMedicalTestRepository repository)
    : IRequestHandler<DeleteMedicalTestCommand>
{
    public async Task Handle(DeleteMedicalTestCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Medical test '{request.Id}' was not found.");

        if (await repository.HasTestRequestAsync(request.Id, cancellationToken))
            throw new ApplicationConflictException(
                "Cannot delete a medical test that has a test request. Remove the test request first.");

        entity.ParameterSchema?.Dispose();
        await repository.DeleteAsync(entity, cancellationToken);
    }
}

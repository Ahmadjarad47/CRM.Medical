using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.MedicalTests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.Queries.GetMedicalTestById;

public sealed class GetMedicalTestByIdQueryHandler(IMedicalTestRepository repository)
    : IRequestHandler<GetMedicalTestByIdQuery, MedicalTestDto>
{
    public async Task<MedicalTestDto> Handle(
        GetMedicalTestByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Medical test '{request.Id}' was not found.");

        return entity.ToDto();
    }
}

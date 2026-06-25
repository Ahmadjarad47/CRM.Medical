using MediatR;

namespace CRM.Medical.Application.Features.CategoryMedical.CQRS;

public sealed record DeleteCategoryMedicalCommand(int Id) : IRequest<Unit>;

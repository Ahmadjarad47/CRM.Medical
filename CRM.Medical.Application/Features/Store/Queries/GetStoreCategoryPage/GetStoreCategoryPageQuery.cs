using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.GetStoreCategoryPage;

public sealed record GetStoreCategoryPageQuery(int Id) : IRequest<CategoryPageDto>;

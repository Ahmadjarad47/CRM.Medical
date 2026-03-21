using CRM.Medical.Application.Features.Users.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(string UserId) : IRequest<UserDetailDto>;

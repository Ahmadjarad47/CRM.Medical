using CRM.Medical.Application.Features.Users.Common;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Application.Features.Users.Queries.GetUsers;

public sealed class GetUsersQueryHandler(UserManager<User> userManager)
    : IRequestHandler<GetUsersQuery, IReadOnlyList<UserSummaryDto>>
{
    public async Task<IReadOnlyList<UserSummaryDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userManager.Users
            .OrderBy(x => x.Email)
            .ToListAsync(cancellationToken);

        return users.Select(x => x.ToSummaryDto()).ToArray();
    }
}

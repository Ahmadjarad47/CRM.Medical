using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.UpdateStoreSettings;

public sealed record UpdateStoreSettingsCommand(StoreSettingDto Request) : IRequest<StoreSettingDto>;

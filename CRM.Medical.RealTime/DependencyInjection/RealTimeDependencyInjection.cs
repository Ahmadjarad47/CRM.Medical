using CRM.Medical.Application.Abstractions.Chat;
using CRM.Medical.Application.Features.Chat.Services;
using CRM.Medical.RealTime.Hubs;
using CRM.Medical.RealTime.Infrastructure.Redis;
using CRM.Medical.RealTime.Notifications;
using CRM.Medical.RealTime.Presence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace CRM.Medical.RealTime;

/// <summary>Registers SignalR, Redis presence, chat broadcasting, and presence notifications.</summary>
public static class RealTimeDependencyInjection
{
    /// <summary>Registers SignalR, Redis-backed presence, chat broadcasting, and online-users presence notifications.</summary>
    public static IServiceCollection AddCrmRealTimeChat(this IServiceCollection services)
    {
        services.AddSignalR();

        services.AddSingleton<IChatRealtimeNotifier, ChatRealtimeNotifier>();
        services.AddSingleton<IPresenceEventNotifier, OnlineUsersPresenceNotifier>();
        services.AddSingleton<PresenceLifecycleCoordinator>();
        services.AddSingleton<IOnlineUserService, OnlineUserService>();

        services.AddSingleton<IConnectionManager>(sp =>
        {
            var mux = sp.GetService<IConnectionMultiplexer>();
            var lf = sp.GetRequiredService<ILoggerFactory>();
            return mux is null
                ? new NullConnectionManager(lf.CreateLogger<NullConnectionManager>())
                : new RedisConnectionManager(mux, lf.CreateLogger<RedisConnectionManager>());
        });

        return services;
    }

    /// <summary>Maps SignalR hubs (call after <c>UseAuthorization</c>).</summary>
    public static WebApplication MapCrmChatHubs(this WebApplication app)
    {
        app.MapHub<ChatHub>("/hubs/chat");
        app.MapHub<OnlineUsersHub>("/hubs/online-users");
        return app;
    }
}

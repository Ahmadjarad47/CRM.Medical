using CRM.Medical.Application.Abstractions.Chat;
using CRM.Medical.Application.Features.Chat.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace CRM.Medical.RealTime;

public static class RealTimeDependencyInjection
{
    /// <summary>Registers SignalR, Redis-backed presence, and chat broadcasting.</summary>
    public static IServiceCollection AddCrmRealTimeChat(this IServiceCollection services)
    {
        services.AddSignalR();

        services.AddSingleton<IChatRealtimeNotifier, ChatRealtimeNotifier>();

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
        return app;
    }
}

using MelbergFramework.ComponentTesting.Redis.Mocks;
using MelbergFramework.Core.DependencyInjection;
using MelbergFramework.Infrastructure.Redis;
using Microsoft.Extensions.DependencyInjection;

namespace MelbergFramework.ComponentTesting.Redis;

public static class RedisComponentTestDependencyModule
{
    public static IServiceCollection
        OverrideRedisContext <TContext> (this IServiceCollection services)
        where TContext : RedisContext
    {
        return services
            .Configure<RedisConnectionOptions<TContext>>(_ => 
            {
                _.Uri = "a";
            })
            .OverrideWithSingleton<IConnector,MockConnector>();
    }
}

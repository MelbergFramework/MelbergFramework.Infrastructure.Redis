using MelbergFramework.Core.HealthCheck;
using Microsoft.Extensions.DependencyInjection;

namespace MelbergFramework.Infrastructure.Redis;

public static class RedisDependencyModule
{
    public static void LoadRedisRepository<TFrom, TTo, TContext>(IServiceCollection services)
    where TTo : RedisRepository<TContext>, TFrom
    where TFrom : class
    where TContext : RedisContext
    {
        services.AddOptions<RedisConnectionOptions<TContext>>()
            .BindConfiguration(RedisConnectionOptions<TContext>.Section)
            .ValidateDataAnnotations();

        services.AddTransient<TFrom, TTo>();
        services.AddSingleton<TContext, TContext>();
        services.AddSingleton<IConnector,Connector>();
        
        services.AddSingleton<IHealthCheck
            ,RedisHealthCheck<TContext>>();
    }

}

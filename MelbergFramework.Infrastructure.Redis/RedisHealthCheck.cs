using MelbergFramework.Core.Health;
using Microsoft.Extensions.DependencyInjection;

namespace MelbergFramework.Infrastructure.Redis;
public class RedisHealthCheck<TRedisContext> : HealthCheck
    where TRedisContext : RedisContext
{
    private readonly TRedisContext _context;
    
    public RedisHealthCheck(IServiceProvider serviceProvider)
    {
        _context = serviceProvider.GetRequiredService<TRedisContext>();
    }

    public override string Name => "rabbitconsumer_"+typeof(TRedisContext).Name;
    public override async Task<bool> IsOk(CancellationToken token)
    {
        try
        {
            await _context.DB.PingAsync();       
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
}
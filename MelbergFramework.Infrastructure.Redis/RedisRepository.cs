using StackExchange.Redis;

namespace MelbergFramework.Infrastructure.Redis;

public class RedisRepository<TContext>
    where TContext : RedisContext
{
    private readonly TContext _context;

    public IDatabaseAsync DB  => _context.DB;
    public RedisRepository(TContext context) { _context = context; }
}
using System.ComponentModel.DataAnnotations;

namespace MelbergFramework.Infrastructure.Redis;

public interface IRedisConnectionOptions
{
    string Uri {get;}
}

public class RedisConnectionOptions<TContext> : IRedisConnectionOptions
    where TContext : RedisContext
{
    public static string Section => $"{typeof(TContext).Name}";

    [Required]
    [MinLength(1)]
    public string Uri {get; set;} = string.Empty;
}

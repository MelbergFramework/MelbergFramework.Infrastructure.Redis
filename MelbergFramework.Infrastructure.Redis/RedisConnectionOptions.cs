using System.ComponentModel.DataAnnotations;

namespace MelbergFramework.Infrastructure.Redis;

public class RedisConnectionOptions<TContext>
    where TContext : RedisContext
{
    public static string Section => $"{typeof(TContext).Name}";

    [Required]
    [MinLength(1)]
    public string Uri {get; set;} = string.Empty;
}

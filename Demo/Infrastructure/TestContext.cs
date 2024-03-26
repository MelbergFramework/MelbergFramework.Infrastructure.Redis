using MelbergFramework.Infrastructure.Redis;
using Microsoft.Extensions.Options;

namespace Demo.Infrastructure;

public class TestContext : RedisContext
{
    public TestContext(IOptions<RedisConnectionOptions<RedisContext>> options,
            IConnector connector) :
        base(options, connector)
    {
    }
}

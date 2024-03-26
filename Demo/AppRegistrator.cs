using Demo.Infrastructure;
using MelbergFramework.Application;
using MelbergFramework.Infrastructure.Redis;

namespace Demo;

public class AppRegistrator : Registrator
{
    public AppRegistrator()
    {
    }

    public override void RegisterServices(IServiceCollection services)
    {
        RedisDependencyModule.LoadRedisRepository<
            ITestRepository,
            TestRepository,
            TestContext>(services);
    }
}

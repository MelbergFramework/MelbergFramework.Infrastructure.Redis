using MelbergFramework.Infrastructure.Redis;

namespace Demo.Infrastructure;

public interface ITestRepository
{
    Task Demo();
}

public class TestRepository : RedisRepository<TestContext>, ITestRepository
{
    public TestRepository(TestContext context) : base(context)
    {
    }

    public Task Demo()
    {
        return DB.PingAsync();
    }
}
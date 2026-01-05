using Demo;
using MelbergFramework.Application;
using MelbergFramework.ComponentTesting.Redis;
using MelbergFramework.ComponentTesting.Redis.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestContext = Demo.Infrastructure.TestContext;

namespace Tests;

// [Collection("test")]
public partial class BaseTest : BaseTestFrame
{
    public BaseTest()
    {
        App = MelbergHost
            .CreateHost<AppRegistrator>()
            .AddServices( _ => 
            {
                _.OverrideRedisContext<TestContext>();
            }
            ) 
            .Build();
    }
    public async Task Init()
    {
        var fixture = GetClass<RedisFixture>();
        await fixture.InitializeAsync();
    }
    public async Task Something()
    {
        var context = GetClass<TestContext>();
        await context.DB.StringSetAsync("a","b");
        await context.DB.StringSetAsync("a","b");
        await context.DB.PingAsync();
        await context.DB.StringSetAsync("a","b");

    }

    public async Task Add_object()
    {
        var context = GetClass<TestContext>();
        await context.DB.StringSetAsync("a","b");
    }


    public async Task Object_is_there()
    {
        var context = GetClass<TestContext>();
        var result = (string) await context.DB.StringGetAsync("a");
        Assert.AreEqual(result,"b");
    }

}

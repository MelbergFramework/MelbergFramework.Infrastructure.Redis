using Demo;
using MelbergFramework.Application;
using MelbergFramework.ComponentTesting.Redis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TestContext = Demo.Infrastructure.TestContext;

namespace Tests;

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
    public async Task Add_object()
    {
        var context = GetClass<TestContext>();
        await context.DB.StringSetAsync("a","b");
    }

    public async Task Object_is_there()
    {
        var context = GetClass<TestContext>();
        var result = await context.DB.StringGetAsync("a");
        var resultValue = JsonConvert.DeserializeObject<string>(result);
        Assert.AreEqual(resultValue,"b");
    }
}

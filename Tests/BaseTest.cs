using Demo;
using MelbergFramework.Application;
using MelbergFramework.ComponentTesting.Redis;
using MelbergFramework.ComponentTesting.Redis.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

    public async Task Add_to_set()
    {
        var context = GetClass<TestContext>();
        await context.DB.SetAddAsync("set","a");
    }

    public async Task Object_is_there()
    {
        var context = GetClass<TestContext>();
        var result = (string) await context.DB.StringGetAsync("a");
        Assert.AreEqual(result,"b");
    }

    public async Task Get_from_set()
    {
        var context = GetClass<TestContext>();
        var resultset = await context.DB.SetMembersAsync("set");
        foreach(var result in resultset)
        {
            Assert.AreEqual((string)result,"a");
        }
    }

    public async Task Object_not_there_is_null()
    {
        var context = GetClass<TestContext>();
        var result = await context.DB.StringGetAsync("somethingthatdoesntexist");
        Assert.IsTrue(result.IsNull);
    }

    public async Task Lock_thing()
    {
        var context = GetClass<TestContext>();
        await context.DB.LockTakeAsync("lock", 1, TimeSpan.FromDays(1));
    }

    public async Task Verify_is_locked()
    {
        var context = GetClass<TestContext>();

        var mockDB = (MockDatabaseAsync)context.DB;
        var value = mockDB._dictionary["lock"];

        Assert.IsTrue(value.IsLocked);
    }

    public async Task Unlock_thing_sucessful()
    {
        var context = GetClass<TestContext>();
        await context.DB.LockReleaseAsync("lock",1);
    }

    public async Task Verify_range()
    {
        var context = GetClass<TestContext>();
        for(int i = 0; i < 10; i++)
        {
            await context.DB.ListRightPushAsync("h",$"{i}");
        }

        var result = await context.DB.ListRangeAsync("h");

        Assert.AreEqual(10,result.Count());

        result = await context.DB.ListRangeAsync("h",0,1);

        Assert.AreEqual(2,result.Count());

        result = await context.DB.ListRangeAsync("h",1,1);

        Assert.AreEqual(1,result.Count());
    }

    public async Task Verify_is_unlocked()
    {
        var context = GetClass<TestContext>();

        var mockDB = (MockDatabaseAsync)context.DB;
        var value = mockDB._dictionary["lock"];

        Assert.IsTrue(!value.IsLocked);
    }
}

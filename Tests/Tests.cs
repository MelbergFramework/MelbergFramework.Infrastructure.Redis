using LightBDD.Framework.Scenarios;
using LightBDD.MsTest3;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests;

[TestClass]
public class Tests : BaseTest
{
    [Scenario]
    [TestMethod]
    public async Task Test_not_found_is_ok()
    {
        await Runner.RunScenarioAsync(
            _ => Object_not_there_is_null()
            );
    }
    [Scenario]
    [TestMethod]
    public async Task Test_String_Set_And_Get()
    {
        await Runner.RunScenarioAsync(
            _ => Add_object(),
            _ => Object_is_there()
            );
    }
    [Scenario]
    [TestMethod]
    public async Task Test_Set_Set_And_Get()
    {
        await Runner.RunScenarioAsync(
            _ => Add_to_set(),
            _ => Get_from_set()
            );
    }

    [Scenario]
    [TestMethod]
    public async Task Test_locking()
    {
        await Runner.RunScenarioAsync(
            _ => Lock_thing(),
            _ => Verify_is_locked(),
            _ => Unlock_thing_sucessful(),
            _ => Verify_is_unlocked()
                );

    }
}

using LightBDD.Framework.Scenarios;
using LightBDD.MsTest3;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests;

[TestClass]
public class Tests : BaseTest
{
    [Scenario]
    [TestMethod]
    public async Task Test_Something()
    {
        await Runner.RunScenarioAsync(
                _ => Init(),
            _ => Something()
            );
    }
    [Scenario]
    [TestMethod]
    public async Task Test_String_Set_And_Get()
    {
        await Runner.RunScenarioAsync(
                _ => Init(),
            _ => Add_object(),
            _ => Object_is_there()
            );
    }
}

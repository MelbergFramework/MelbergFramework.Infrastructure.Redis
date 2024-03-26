using LightBDD.Framework.Scenarios;
using LightBDD.MsTest3;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests;

[TestClass]
public class Tests : BaseTest
{
    [Scenario]
    [TestMethod]
    public async Task Test_adding()
    {
        await Runner.RunScenarioAsync(
            _ => Add_object(),
            _ => Object_is_there()
            );
    }
}

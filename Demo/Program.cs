using Demo.Infrastructure;
using MelbergFramework.Application;

namespace Demo;
internal class Program
{
    private static async Task Main(string[] args)
    {
        var app = MelbergHost
                .CreateHost<AppRegistrator>()
                .Build();
        await app.Services.GetService<ITestRepository>().Demo();
    }
}

using MelbergFramework.Application;

namespace Demo;
internal class Program
{
    private static async Task Main(string[] args)
    {
        var app = MelbergHost
                .CreateHost<AppRegistrator>();
    }
}

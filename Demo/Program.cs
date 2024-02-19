using Demo.Infrastructure;
using MelbergFramework.Infrastructure.Redis;

var builder = WebApplication.CreateBuilder(args);

RedisModule.LoadRedisRepository<ITestRepository, TestRepository, TestContext>(builder.Services);


var app = builder.Build();

await app.Services.GetRequiredService<ITestRepository>().Demo();

app.MapGet("/", () => "Hello World!");




app.Run();

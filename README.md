# MelbergFramework.Infrastructure.Redis

Redis implementation for MelbergFramework.

# Anatomy of Appsettings.json

Each Repository will use a class which inherits from RedisContext for its Context.  See the demo for a clear example.

Use the [StackExchange Documentation](https://stackexchange.github.io/StackExchange.Redis/Configuration.html) to configure that connection string.

# Notes

## General Execution
This project requires dotnet 8 sdk to run (install link [here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)).
## How to run a Redis Server
Docker installation guide for redis [here](https://github.com/bitnami/containers/blob/main/bitnami/redis/README.md).

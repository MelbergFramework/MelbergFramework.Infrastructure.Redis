# MelbergFramework.Infrastructure.Redis

Redis implementation for MelbergFramework.

# Anatomy of Appsettings.json

Use the RedisContext type name to relate the connection string in the ConnectionStrings section to the repository.

Use the [StackExchange Documentation](https://stackexchange.github.io/StackExchange.Redis/Configuration.html) to configure that connection string.

# Notes

## General Execution
This project requires dotnet 6 sdk to run (install link [here](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)).
## How to run a Redis Server
Docker installation guide for redis [here](https://github.com/bitnami/containers/blob/main/bitnami/redis/README.md).
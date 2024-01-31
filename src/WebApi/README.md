# WebAPI

WebApi is a web API that provides CRUD operations for managing recipes. Technologies used:

- [ASP.NET Core minimal APIs](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis/overview)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [ASP.NET Core Identity](https://learn.microsoft.com/aspnet/core/security/authentication/identity) with authorization handlers
- Markdown ([Markdig](https://github.com/xoofx/markdig))
- Snowflake IDs ([IdGen](https://github.com/RobThree/IdGen))

## Configuration

Configuration can be provided in appsettings.json, user secrets, environment variables, or command line arguments. For more information, see: [Configuration Providers in .NET](https://learn.microsoft.com/dotnet/core/extensions/configuration-providers).

The following configuration options are available:

- GeneratorId - ID number for running instance (each concurrently running instance should have a unique value)

## Seed Data

Seed data was created using the [AIDataGenerator](../AIDataGenerator/) project with [Updated GPT-3.5 Turbo](https://platform.openai.com/docs/models/gpt-3-5) and [DALL-E 3](https://platform.openai.com/docs/models/dall-e).
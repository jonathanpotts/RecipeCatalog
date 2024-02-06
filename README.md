# Recipe Catalog

[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/jonathanpotts/RecipeCatalog/dotnet.yml)](https://github.com/jonathanpotts/RecipeCatalog/actions/workflows/dotnet.yml)

Recipe Catalog is a [.NET](https://dotnet.microsoft.com/) 8 project showcasing a Web catalog for recipes with generative AI capabilities.

## Projects

### AIDataGenerator

[AIDataGenerator](src/AIDataGenerator/) is a console app that uses generative AI to generate sample data for the catalog. Technologies used:

- [.NET](https://dotnet.microsoft.com/)
    - [Generic Host](https://learn.microsoft.com/dotnet/core/extensions/generic-host)
- [OpenAI](https://openai.com/) / [Azure OpenAI Service](https://azure.microsoft.com/products/ai-services/openai-service/) ([Azure SDK for .NET](https://github.com/Azure/azure-sdk-for-net))
    - [Chat Completions](https://platform.openai.com/docs/guides/text-generation/chat-completions-api) in [JSON Mode](https://platform.openai.com/docs/guides/text-generation/json-mode): [Updated GPT-3.5 Turbo](https://platform.openai.com/docs/models/gpt-3-5) / [GPT-4 Turbo](https://platform.openai.com/docs/models/gpt-4-and-gpt-4-turbo)
    - [Image Generation](https://platform.openai.com/docs/guides/images): [DALL-E 2 / DALL-E 3](https://platform.openai.com/docs/models/dall-e)
- [SkiaSharp](https://github.com/mono/SkiaSharp)
- [Spectre.Console](https://spectreconsole.net/)

### WebApi

[WebApi](src/WebApi/) is a REST Web API that provides CRUD operations for managing recipes. Technologies used:

- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
    - [Minimal APIs](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis/overview)
    - [ASP.NET Core Identity](https://learn.microsoft.com/aspnet/core/security/authentication/identity)
    - [Resource-based Authorization](https://learn.microsoft.com/aspnet/core/security/authorization/resourcebased)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
    - [Migrations](https://learn.microsoft.com/ef/core/managing-schemas/migrations/)
    - [SQLite](https://www.sqlite.org/)
- [FluentValidation](https://github.com/FluentValidation/FluentValidation)
- [Swagger / OpenAPI](https://swagger.io/) ([Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore))
- Markdown ([Markdig](https://github.com/xoofx/markdig))
- Snowflake IDs ([IdGen](https://github.com/RobThree/IdGen))

### Tests

#### WebApi.Tests

[WebApi.Tests](tests/WebApi.Tests/) is a project containing tests for the [WebApi](src/WebApi/) project. Technologies used:

- [xUnit](https://xunit.net/)

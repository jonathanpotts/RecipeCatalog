# Recipe Catalog

[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/jonathanpotts/RecipeCatalog/dotnet.yml)](https://github.com/jonathanpotts/RecipeCatalog/actions/workflows/dotnet.yml)

Recipe Catalog is a multilayer [.NET](https://dotnet.microsoft.com/) 8 project showcasing a Web catalog for recipes with generative AI capabilities.

## Projects

### AIDataGenerator

[AIDataGenerator](src/AIDataGenerator/) is a console app that uses generative AI to generate sample data for the catalog. Technologies used:

- [.NET](https://dotnet.microsoft.com/)
    - [Generic Host](https://learn.microsoft.com/dotnet/core/extensions/generic-host)
- [OpenAI](https://openai.com/) / [Azure OpenAI Service](https://azure.microsoft.com/products/ai-services/openai-service/) ([Azure SDK for .NET](https://github.com/Azure/azure-sdk-for-net))
    - [Chat Completions](https://platform.openai.com/docs/guides/text-generation/chat-completions-api) in [JSON Mode](https://platform.openai.com/docs/guides/text-generation/json-mode): [Updated GPT-3.5 Turbo](https://platform.openai.com/docs/models/gpt-3-5) / [GPT-4 Turbo](https://platform.openai.com/docs/models/gpt-4-and-gpt-4-turbo)
    - [Image Generation](https://platform.openai.com/docs/guides/images): [DALL-E 2 / DALL-E 3](https://platform.openai.com/docs/models/dall-e)
- [Polly](https://github.com/App-vNext/Polly)
- [SkiaSharp](https://github.com/mono/SkiaSharp)
- [Spectre.Console](https://spectreconsole.net/)

### Application

[Application](src/Application/) is the application layer for the project. Technologies used:

- [Resource-based Authorization](https://learn.microsoft.com/aspnet/core/security/authorization/resourcebased)
- [FluentValidation](https://github.com/FluentValidation/FluentValidation)
- Markdown ([Markdig](https://github.com/xoofx/markdig))
- Snowflake IDs ([IdGen](https://github.com/RobThree/IdGen))

### Application.Contracts

[Application.Contracts](src/Application.Contracts/) contains the service interfaces and DTO models for the application layer.

### BlazorApp

[BlazorApp](src/BlazorApp/) is a Blazor web app for viewing and managing recipes. Technologies used:

- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
    - [Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor)
    - [ASP.NET Core Identity](https://learn.microsoft.com/aspnet/core/security/authentication/identity)
- [Bootstrap](https://getbootstrap.com/)
    - [Bootstrap Icons](https://icons.getbootstrap.com/)
    - [Dark Mode](https://getbootstrap.com/docs/5.3/customize/color-modes/)

### BlazorApp.Client

[BlazorApp.Client](src/BlazorApp.Client/) contains the Blazor WebAssembly components for the BlazorApp project.

### Domain

[Domain](src/Domain/) is the domain layer for the project. Technologies used:

- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
    - [Migrations](https://learn.microsoft.com/ef/core/managing-schemas/migrations/)
    - [SQLite](https://www.sqlite.org/)

### Domain.Shared

[Domain.Shared](src/Domain.Shared/) contains the value objects for the domain layer.

### WebApi

[WebApi](src/WebApi/) is a REST Web API that provides CRUD operations for managing recipes. Technologies used:

- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
    - [Minimal APIs](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis/overview)
    - [ASP.NET Core Identity](https://learn.microsoft.com/aspnet/core/security/authentication/identity)
- [Swagger / OpenAPI](https://swagger.io/) ([Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore))

### Tests

#### WebApi.Tests

[WebApi.Tests](tests/WebApi.Tests/) is a project containing tests for the [WebApi](src/WebApi/) project. Technologies used:

- [xUnit](https://xunit.net/)


## Seed Data

Seed data was created using the [AIDataGenerator](../AIDataGenerator/) project with [Updated GPT-3.5 Turbo](https://platform.openai.com/docs/models/gpt-3-5) and [DALL-E 3](https://platform.openai.com/docs/models/dall-e).

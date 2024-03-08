# Recipe Catalog

[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/jonathanpotts/RecipeCatalog/dotnet.yml?logo=github)](https://github.com/jonathanpotts/RecipeCatalog/actions/workflows/dotnet.yml) [![Codecov Code Coverage](https://img.shields.io/codecov/c/gh/jonathanpotts/RecipeCatalog?logo=codecov)](https://codecov.io/gh/jonathanpotts/RecipeCatalog)

Recipe Catalog is a multilayered [.NET](https://dotnet.microsoft.com/) 8 project showcasing a Web catalog for recipes with AI capabilities.

This project contains continuous integration (CI) and continuous deployment (CD) workflows using [GitHub Actions](https://docs.github.com/actions). Code coverage reports are uploaded to [Codecov](https://codecov.io/). The web app is deployed to [Azure Web Apps (Azure App Service)](https://azure.microsoft.com/products/app-service/web/).

## Projects

### AI

[AI](src/AI/) is a class library that uses generative AI for text and image generation. Technologies used:

- [.NET](https://dotnet.microsoft.com/)
    - [Dependency Injection](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection)
    - [Options Pattern](https://learn.microsoft.com/dotnet/core/extensions/options)
- [OpenAI](https://openai.com/) / [Azure OpenAI Service](https://azure.microsoft.com/products/ai-services/openai-service/) ([Azure SDK for .NET](https://github.com/Azure/azure-sdk-for-net))
    - [Chat Completions](https://platform.openai.com/docs/guides/text-generation/chat-completions-api) in [JSON Mode](https://platform.openai.com/docs/guides/text-generation/json-mode): [GPT-3.5 Turbo](https://platform.openai.com/docs/models/gpt-3-5) / [GPT-4 Turbo](https://platform.openai.com/docs/models/gpt-4-and-gpt-4-turbo)
    - [Embeddings](https://platform.openai.com/docs/guides/embeddings): [Ada V2 / Embedding V3 Small](https://platform.openai.com/docs/models/embeddings)
    - [Image Generation](https://platform.openai.com/docs/guides/images): [DALL-E 3](https://platform.openai.com/docs/models/dall-e)

### AIDataGenerator

[AIDataGenerator](src/AIDataGenerator/) is a console app that uses the [AI class library](src/AI/) to generate example data for the catalog. Technologies used:

- [.NET](https://dotnet.microsoft.com/)
    - [Generic Host](https://learn.microsoft.com/dotnet/core/extensions/generic-host)
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
    - [Library Manager (LibMan)](https://learn.microsoft.com/aspnet/core/client-side/libman)
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

[WebApi](src/WebApi/) is a REST Web API that uses the [WebApi.Shared class library](src/WebApi.Shared/) to provide CRUD operations for managing recipes. Technologies used:

- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
    - [Minimal APIs](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis/overview)
    - [ASP.NET Core Identity](https://learn.microsoft.com/aspnet/core/security/authentication/identity)
- [Swagger / OpenAPI](https://swagger.io/) ([Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore))

### WebApi.Shared

[WebApi.Shared](src/WebApi.Shared/) contains the minimal APIs that provide CRUD operations for managing recipes. Technologies used:

- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
    - [Minimal APIs](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis/overview)

### Tests

#### Application.Tests

[Application.Tests](tests/Application.Tests/) is a project containing tests for the [Application](src/Application/) project. Technologies used:

- [xUnit](https://xunit.net/)
- [Moq](https://github.com/devlooped/moq)

#### Domain.Tests

[Domain.Tests](tests/Domain.Tests/) is a project containing tests for the [Domain](src/Domain/) project. Technologies used:

- [xUnit](https://xunit.net/)

#### WebApi.Shared.Tests

[WebApi.Shared.Tests](tests/WebApi.Shared.Tests/) is a project containing tests for the [WebApi.Shared](src/WebApi.Shared/) project. Technologies used:

- [xUnit](https://xunit.net/)
- [Moq](https://github.com/devlooped/moq)

#### WebApi.Tests

[WebApi.Tests](tests/WebApi.Tests/) is a project containing tests for the [WebApi](src/WebApi/) project. Technologies used:

- [xUnit](https://xunit.net/)
- [ASP.NET Core Integration Tests](https://learn.microsoft.com/aspnet/core/test/integration-tests) ([Microsoft.AspNetCore.Mvc.Testing](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Testing))

## Seed Data

Seed data was created using the [AIDataGenerator](../AIDataGenerator/) project with [GPT-3.5 Turbo](https://platform.openai.com/docs/models/gpt-3-5) and [DALL-E 3](https://platform.openai.com/docs/models/dall-e).

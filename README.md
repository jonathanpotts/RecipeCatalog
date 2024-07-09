# Recipe Catalog

[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/jonathanpotts/RecipeCatalog/dotnet.yml?logo=github)](https://github.com/jonathanpotts/RecipeCatalog/actions/workflows/dotnet.yml) [![Codecov Code Coverage](https://img.shields.io/codecov/c/gh/jonathanpotts/RecipeCatalog?logo=codecov)](https://codecov.io/gh/jonathanpotts/RecipeCatalog)

Recipe Catalog is a multilayered [.NET](https://dotnet.microsoft.com/) 8 project showcasing a Web catalog for recipes with AI capabilities.

This project contains continuous integration (CI) and continuous deployment (CD) workflows using [GitHub Actions](https://docs.github.com/actions). Code coverage reports are uploaded to [Codecov](https://codecov.io/). The web app is deployed to [Azure Web Apps (Azure App Service)](https://azure.microsoft.com/products/app-service/web/).

## Projects

### RecipeCatalog.AIDataGenerator

[RecipeCatalog.AIDataGenerator](src/RecipeCatalog.AIDataGenerator/) is a console app that generates example data for the catalog. Technologies used:

- [.NET](https://dotnet.microsoft.com/)
    - [Generic Host](https://learn.microsoft.com/dotnet/core/extensions/generic-host)
- [Semantic Kernel](https://learn.microsoft.com/semantic-kernel/overview/?tabs=Csharp)
- [SkiaSharp](https://github.com/mono/SkiaSharp)
- [Spectre.Console](https://spectreconsole.net/)

### RecipeCatalog.Application

[RecipeCatalog.Application](src/RecipeCatalog.Application/) is the application layer for the project. Technologies used:

- [Resource-based Authorization](https://learn.microsoft.com/aspnet/core/security/authorization/resourcebased)
- [FluentValidation](https://github.com/FluentValidation/FluentValidation)
- Markdown ([Markdig](https://github.com/xoofx/markdig))
- Snowflake IDs ([IdGen](https://github.com/RobThree/IdGen))

### RecipeCatalog.Application.Contracts

[RecipeCatalog.Application.Contracts](src/RecipeCatalog.Application.Contracts/) contains the service interfaces and DTO models for the application layer.

### RecipeCatalog.BlazorApp

[RecipeCatalog.BlazorApp](src/RecipeCatalog.BlazorApp/) is a Blazor web app for viewing and managing recipes. Technologies used:

- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
    - [Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor)
    - [ASP.NET Core Identity](https://learn.microsoft.com/aspnet/core/security/authentication/identity)
    - [Library Manager (LibMan)](https://learn.microsoft.com/aspnet/core/client-side/libman)
- [Bootstrap](https://getbootstrap.com/)
    - [Bootstrap Icons](https://icons.getbootstrap.com/)
    - [Dark Mode](https://getbootstrap.com/docs/5.3/customize/color-modes/)

### RecipeCatalog.BlazorApp.Client

[RecipeCatalog.BlazorApp.Client](src/RecipeCatalog.BlazorApp.Client/) contains the Blazor WebAssembly components for the RecipeCatalog.BlazorApp project.

### RecipeCatalog.Domain

[RecipeCatalog.Domain](src/RecipeCatalog.Domain/) is the domain layer for the project. Technologies used:

- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
    - [Migrations](https://learn.microsoft.com/ef/core/managing-schemas/migrations/)
    - [SQLite](https://www.sqlite.org/)

### RecipeCatalog.Domain.Shared

[RecipeCatalog.Domain.Shared](src/RecipeCatalog.Domain.Shared/) contains the value objects for the domain layer.

### RecipeCatalog.WebApi

[RecipeCatalog.WebApi](src/RecipeCatalog.WebApi/) is a REST Web API that uses the [RecipeCatalog.WebApi.Shared class library](src/RecipeCatalog.WebApi.Shared/) to provide CRUD operations for managing recipes. Technologies used:

- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
    - [Minimal APIs](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis/overview)
    - [ASP.NET Core Identity](https://learn.microsoft.com/aspnet/core/security/authentication/identity)
- [Swagger / OpenAPI](https://swagger.io/) ([Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore))

### RecipeCatalog.WebApi.Shared

[RecipeCatalog.WebApi.Shared](src/RecipeCatalog.WebApi.Shared/) contains the minimal APIs that provide CRUD operations for managing recipes. Technologies used:

- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
    - [Minimal APIs](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis/overview)

### Tests

#### RecipeCatalog.Application.Tests

[RecipeCatalog.Application.Tests](tests/RecipeCatalog.Application.Tests/) is a project containing tests for the [RecipeCatalog.Application](src/RecipeCatalog.Application/) project. Technologies used:

- [xUnit](https://xunit.net/)
- [Moq](https://github.com/devlooped/moq)

#### RecipeCatalog.BlazorApp.Tests

[RecipeCatalog.BlazorApp.Tests](tests/RecipeCatalog.BlazorApp.Tests/) is a project containing tests for the [RecipeCatalog.BlazorApp](src/RecipeCatalog.BlazorApp/) project. Technologies used:

- [xUnit](https://xunit.net/)
- [bUnit](https://bunit.dev/)

#### RecipeCatalog.BlazorApp.Client.Tests

[RecipeCatalog.BlazorApp.Client.Tests](tests/RecipeCatalog.BlazorApp.Client.Tests/) is a project containing tests for the [RecipeCatalog.BlazorApp.Client](src/RecipeCatalog.BlazorApp.Client/) project. Technologies used:

- [xUnit](https://xunit.net/)
- [bUnit](https://bunit.dev/)

#### RecipeCatalog.Domain.Tests

[RecipeCatalog.Domain.Tests](tests/RecipeCatalog.Domain.Tests/) is a project containing tests for the [RecipeCatalog.Domain](src/RecipeCatalog.Domain/) project. Technologies used:

- [xUnit](https://xunit.net/)

#### RecipeCatalog.WebApi.Shared.Tests

[RecipeCatalog.WebApi.Shared.Tests](tests/RecipeCatalog.WebApi.Shared.Tests/) is a project containing tests for the [RecipeCatalog.WebApi.Shared](src/RecipeCatalog.WebApi.Shared/) project. Technologies used:

- [xUnit](https://xunit.net/)
- [Moq](https://github.com/devlooped/moq)

#### RecipeCatalog.WebApi.Tests

[RecipeCatalog.WebApi.Tests](tests/RecipeCatalog.WebApi.Tests/) is a project containing tests for the [RecipeCatalog.WebApi](src/RecipeCatalog.WebApi/) project. Technologies used:

- [xUnit](https://xunit.net/)
- [ASP.NET Core Integration Tests](https://learn.microsoft.com/aspnet/core/test/integration-tests) ([Microsoft.AspNetCore.Mvc.Testing](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Testing))

## Seed Data

Seed data was created using the [RecipeCatalog.AIDataGenerator](src/RecipeCatalog.AIDataGenerator/) project with [GPT-3.5 Turbo](https://platform.openai.com/docs/models/gpt-3-5), [Embedding V3 small](https://platform.openai.com/docs/models/embeddings), and [DALL-E 3](https://platform.openai.com/docs/models/dall-e).

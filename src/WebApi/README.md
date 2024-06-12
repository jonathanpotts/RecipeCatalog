# WebAPI

WebApi is a REST Web API that uses the [WebApi.Shared class library](../src/WebApi.Shared/) to provide CRUD operations for managing recipes. Technologies used:

- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
    - [Minimal APIs](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis/overview)
    - [ASP.NET Core Identity](https://learn.microsoft.com/aspnet/core/security/authentication/identity)
- [Swagger / OpenAPI](https://swagger.io/) ([Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore))

## Configuration

Configuration can be provided in appsettings.json, user secrets, environment variables, or command line arguments. For more information, see: [Configuration Providers in .NET](https://learn.microsoft.com/dotnet/core/extensions/configuration-providers).

The following configuration options are available:

- GeneratorId - ID number for running instance (each concurrently running instance should have a unique value)
- OpenAI - **(Required for AI Search)**
    - TextEmbedding
        - Model - **(Required)** Text embedding model (e.g. text-embedding-ada-002, text-embedding-3-small); use deployment name for Azure OpenAI Service
        - Key - **(Required)** API key for OpenAI or Azure OpenAI Service
        - Endpoint - Azure OpenAI Service endpoint; only used when using Azure OpenAI Service

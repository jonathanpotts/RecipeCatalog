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
    - Endpoint - **(Required for Azure OpenAI Service)** Azure OpenAI Service Endpoint
    - ApiKey - **(Required)** API key for OpenAI or Azure OpenAI Service
    - EmbeddingsDeploymentName
        - OpenAI - Embeddings model (defaults to text-embedding-3-small); supports text-embedding-ada-002 or text-embedding-3-small
        - Azure OpenAI Service - **(Required)** Deployment name for Embeddings model; supports text-embedding-ada-002 or text-embedding-3-small

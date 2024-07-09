# RecipeCatalog.AIDataGenerator

![Screenshot](screenshot.png)

RecipeCatalog.AIDataGenerator is a console app that generates example data for the catalog. Technologies used:

- [.NET](https://dotnet.microsoft.com/)
    - [Generic Host](https://learn.microsoft.com/dotnet/core/extensions/generic-host)
- [Semantic Kernel](https://learn.microsoft.com/semantic-kernel/overview/?tabs=Csharp)
- [SkiaSharp](https://github.com/mono/SkiaSharp)
- [Spectre.Console](https://spectreconsole.net/)

## Configuration

Configuration can be provided in appsettings.json, user secrets, environment variables, or command line arguments. For more information, see: [Configuration Providers in .NET](https://learn.microsoft.com/dotnet/core/extensions/configuration-providers).

The following configuration options are available:

- OpenAI
    - ChatCompletion
        - Model - **(Required)** Chat completion model (e.g. gpt-3.5-turbo, gpt-4o); use deployment name for Azure OpenAI Service
        - Key - **(Required)** API key for OpenAI or Azure OpenAI Service
        - Endpoint - Azure OpenAI Service endpoint; only used when using Azure OpenAI Service
    - TextEmbedding
        - Model - **(Required)** Text embedding model (e.g. text-embedding-ada-002, text-embedding-3-small); use deployment name for Azure OpenAI Service
        - Key - **(Required)** API key for OpenAI or Azure OpenAI Service
        - Endpoint - Azure OpenAI Service endpoint; only used when using Azure OpenAI Service
    - TextToImage
        - Key - **(Required)** API key for OpenAI or Azure OpenAI Service
        - Endpoint - Azure OpenAI Service endpoint; only used when using Azure OpenAI Service
        - Deployment - Deployment for dall-e-3; only used when using Azure OpenAI Service
- Worker
    - Cuisines - **(Required)** List of cuisines to generate recipes for (default values in appsettings.json)
    - RecipesPerCuisine - Number of recipes to generate for each cuisine
    - RecipeGenerationMaxConcurrency - Number of recipes to generate at the same time (defaults to 5)
    - ImageGenerationMaxConcurrency - Number of images to generate at the same time (defaults to 1)
    - ImageQuality - Image quality of saved image (defaults to 60)

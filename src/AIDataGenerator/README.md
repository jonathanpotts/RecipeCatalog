# AIDataGenerator

![Screenshot](screenshot.png)

AIDataGenerator is a console app that uses the [AI class library](../src/AI/) to generate example data for the catalog. Technologies used:

- [.NET](https://dotnet.microsoft.com/)
    - [Generic Host](https://learn.microsoft.com/dotnet/core/extensions/generic-host)
- [SkiaSharp](https://github.com/mono/SkiaSharp)
- [Spectre.Console](https://spectreconsole.net/)

## Configuration

Configuration can be provided in appsettings.json, user secrets, environment variables, or command line arguments. For more information, see: [Configuration Providers in .NET](https://learn.microsoft.com/dotnet/core/extensions/configuration-providers).

The following configuration options are available:

- AITextGenerator
    - Endpoint - **(Required for Azure OpenAI Service)** Azure OpenAI Service Endpoint
    - ApiKey - **(Required)** API key for OpenAI or Azure OpenAI Service
    - ChatCompletionsDeploymentName
        - OpenAI - Chat Completions model (defaults to gpt-3.5-turbo); supports gpt-3.5-turbo or gpt-4-turbo-preview
        - Azure OpenAI Service - **(Required)** Deployment name for Chat Completions model; supports gpt-35-turbo (0125) or gpt-4 (0125-preview)
    - EmbeddingsEndpoint - **(Azure OpenAI Service)** Azure OpenAI Service Endpoint to use for embeddings; Defaults to Endpoint
    - EmbeddingsApiKey - **(Azure OpenAI Service)** API key for Azure OpenAI Service Endpoint to use for embeddings; Defaults to ApiKey
    - EmbeddingsDeploymentName
        - OpenAI - Embeddings model (defaults to text-embedding-3-small); supports text-embedding-ada-002 or text-embedding-3-small
        - Azure OpenAI Service - **(Required)** Deployment name for Embeddings model; supports text-embedding-ada-002 or text-embedding-3-small
- AIImageGenerator
    - Endpoint - **(Required for Azure OpenAI Service)** Azure OpenAI Service Endpoint
    - ApiKey - **(Required)** API key for OpenAI or Azure OpenAI Service
    - DeploymentName - **(Required for Azure OpenAI Service)** Deployment name for Image Generation model; supports dall-e-3
    - Size - Size of generated image; supports 1024x1024, 1024x1792, or 1792x1024 (defaults to 1024x1024)
    - Quality - Quality of image; supports standard or hd (defaults to standard)
    - Style - Style of image; supports vivid or natural (defaults to vivid)
- Worker
    - Cuisines - **(Required)** List of cuisines to generate recipes for (default values in appsettings.json)
    - RecipesPerCuisine - Number of recipes to generate for each cuisine
    - RecipeGenerationMaxConcurrency - Number of recipes to generate at the same time (defaults to 5)
    - ImageGenerationMaxConcurrency - Number of images to generate at the same time (defaults to 1)
    - ImageQuality - Image quality of saved image (defaults to 60)

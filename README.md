# RecipeBook

RecipeBook is a [.NET](https://dotnet.microsoft.com/) 8 project showcasing a web catalog for recipes.

## Projects

### RecipeBook.AIDataGenerator

[RecipeBook.AIDataGenerator](src/RecipeBook.AIDataGenerator/) is a console app that uses AI to generate sample data for the catalog. Technologies used:

- [.NET generic host](https://learn.microsoft.com/dotnet/core/extensions/generic-host)
- [OpenAI](https://openai.com/) / [Azure OpenAI Service](https://azure.microsoft.com/products/ai-services/openai-service/)
    - [Chat Completions](https://platform.openai.com/docs/guides/text-generation/chat-completions-api) in [JSON Mode](https://platform.openai.com/docs/guides/text-generation/json-mode): [GPT-3.5 Turbo (1106)](https://platform.openai.com/docs/models/gpt-3-5) / [GPT-4 Turbo (1106)](https://platform.openai.com/docs/models/gpt-4-and-gpt-4-turbo)
    - [Text Embeddings](https://platform.openai.com/docs/guides/embeddings): [Ada V2 / Embedding V3 small](https://platform.openai.com/docs/models/embeddings)
    - [Image Generation](https://platform.openai.com/docs/guides/images): [DALL-E 2 / DALL-E 3](https://platform.openai.com/docs/models/dall-e)

### RecipeBook.Api

[RecipeBook.Api](src/RecipeBook.Api/) is a web API that provides CRUD operations for managing recipes. Technologies used:

- [ASP.NET Core minimal APIs](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis/overview)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- Markdown ([Markdig](https://github.com/xoofx/markdig))
- Snowflake IDs ([IdGen](https://github.com/RobThree/IdGen))

### RecipeBook.Api.Tests

[RecipeBook.Api.Tests](tests/RecipeBook.Api.Tests/) is a suite of unit tests for the RecipeBook.Api project. Technologies used:

- [xUnit](https://xunit.net/)

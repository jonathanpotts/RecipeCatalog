# AI

AI is a class library that uses generative AI for text and image generation. Technologies used:

- [.NET](https://dotnet.microsoft.com/)
    - [Dependency Injection](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection)
    - [Options Pattern](https://learn.microsoft.com/dotnet/core/extensions/options)
- [OpenAI](https://openai.com/) / [Azure OpenAI Service](https://azure.microsoft.com/products/ai-services/openai-service/) ([Azure SDK for .NET](https://github.com/Azure/azure-sdk-for-net))
    - [Chat Completions](https://platform.openai.com/docs/guides/text-generation/chat-completions-api) in [JSON Mode](https://platform.openai.com/docs/guides/text-generation/json-mode): [GPT-3.5 Turbo](https://platform.openai.com/docs/models/gpt-3-5) / [GPT-4 Turbo](https://platform.openai.com/docs/models/gpt-4-and-gpt-4-turbo)
    - [Embeddings](https://platform.openai.com/docs/guides/embeddings): [Ada V2 / Embedding V3 Small](https://platform.openai.com/docs/models/embeddings)
    - [Image Generation](https://platform.openai.com/docs/guides/images): [DALL-E 3](https://platform.openai.com/docs/models/dall-e)

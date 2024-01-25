using Azure.AI.OpenAI;

namespace RecipeBook.AIDataGenerator.Services;

public abstract class BaseOpenAIImageGenerator : IAIImageGenerator
{
    protected abstract OpenAIClient Client { get; init; }

    public abstract Task<string> GenerateImageAsync(string prompt, CancellationToken cancellationToken = default);

    protected async Task<string> GenerateImageAsync(string deploymentName, string prompt,
        CancellationToken cancellationToken = default)
    {
        var generations = await Client.GetImageGenerationsAsync(new ImageGenerationOptions
        {
            DeploymentName = deploymentName,
            Prompt = prompt,
            ImageCount = 1,
            Size = ImageSize.Size1024x1024
        }, cancellationToken);

        return generations.Value.Data.FirstOrDefault()?.Url.ToString() ??
               throw new Exception("The returned content is invalid");
    }
}

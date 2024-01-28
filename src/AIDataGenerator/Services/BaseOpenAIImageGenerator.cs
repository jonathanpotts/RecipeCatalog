using Azure.AI.OpenAI;

namespace JonathanPotts.RecipeCatalog.AIDataGenerator.Services;

public abstract class BaseOpenAIImageGenerator : IAIImageGenerator
{
    protected abstract OpenAIClient Client { get; init; }

    public abstract Task<string> GenerateImageAsync(string prompt, CancellationToken cancellationToken = default);

    protected async Task<string> GenerateImageAsync(string deploymentName, string prompt, string? size = null,
        string? quality = null, string? style = null, CancellationToken cancellationToken = default)
    {
        var imageSize = size?.ToLower() switch
        {
            "256x256" => ImageSize.Size256x256,
            "512x512" => ImageSize.Size512x512,
            "1024x1024" => ImageSize.Size1024x1024,
            "1024x1792" => ImageSize.Size1024x1792,
            "1792x1024" => ImageSize.Size1792x1024,
            _ => ImageSize.Size1024x1024
        };

        var imageQuality = quality?.ToLower() switch
        {
            "standard" => ImageGenerationQuality.Standard,
            "hd" => ImageGenerationQuality.Hd,
            _ => null
        };

        var imageStyle = style?.ToLower() switch
        {
            "vivid" => ImageGenerationStyle.Vivid,
            "natural" => ImageGenerationStyle.Natural,
            _ => null
        };

        var generations = await Client.GetImageGenerationsAsync(new ImageGenerationOptions
        {
            DeploymentName = deploymentName,
            Prompt = prompt,
            ImageCount = 1,
            Size = imageSize,
            Quality = imageQuality,
            Style = imageStyle
        }, cancellationToken);

        return generations.Value.Data.FirstOrDefault()?.Url.ToString() ??
               throw new Exception("The returned content is invalid");
    }
}

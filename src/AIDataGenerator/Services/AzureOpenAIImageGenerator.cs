using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;

namespace JonathanPotts.RecipeCatalog.AIDataGenerator.Services;

public class AzureOpenAIImageGenerator(IOptions<AzureOpenAIImageGeneratorOptions> options) : BaseOpenAIImageGenerator
{
    private readonly string _deploymentName = options.Value.DeploymentName!;
    private readonly string? _quality = options.Value.Quality;
    private readonly string? _size = options.Value.Size;
    private readonly string? _style = options.Value.Style;

    protected override OpenAIClient Client { get; init; } = new(new Uri(options.Value.Endpoint!),
        new AzureKeyCredential(options.Value.ApiKey!));

    public override Task<string> GenerateImageAsync(string prompt, CancellationToken cancellationToken = default)
    {
        return GenerateImageAsync(_deploymentName, prompt, _size, _quality, _style, cancellationToken);
    }
}

using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;

namespace RecipeBook.DataGenerator.Services;

public class OpenAIImageGenerator(IOptions<OpenAIImageGeneratorOptions> options) : BaseOpenAIImageGenerator
{
    private readonly string _deploymentName = options.Value.DeploymentName ?? "dall-e-2";

    protected override OpenAIClient Client { get; init; } = new(options.Value.ApiKey);

    public override Task<string> GenerateImageAsync(string prompt, CancellationToken cancellationToken = default)
    {
        return GenerateImageAsync(_deploymentName, prompt, cancellationToken);
    }
}

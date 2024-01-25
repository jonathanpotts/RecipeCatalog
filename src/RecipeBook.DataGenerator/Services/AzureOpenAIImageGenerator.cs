using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;

namespace RecipeBook.DataGenerator.Services;

public class AzureOpenAIImageGenerator(IOptions<AzureOpenAIImageGeneratorOptions> options) : BaseOpenAIImageGenerator
{
    private readonly string _deploymentName = options.Value.DeploymentName!;

    protected override OpenAIClient Client { get; init; } = new(new Uri(options.Value.Endpoint!),
        new AzureKeyCredential(options.Value.ApiKey!));

    public override Task<string> GenerateImageAsync(string prompt, CancellationToken cancellationToken = default)
    {
        return GenerateImageAsync(_deploymentName, prompt, cancellationToken);
    }
}

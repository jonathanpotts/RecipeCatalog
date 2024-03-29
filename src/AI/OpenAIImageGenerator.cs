﻿using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;

namespace JonathanPotts.RecipeCatalog.AI;

public class OpenAIImageGenerator(IOptions<OpenAIImageGeneratorOptions> options) : BaseOpenAIImageGenerator
{
    private const string _deploymentName = "dall-e-3";
    private readonly string? _quality = options.Value.Quality;
    private readonly string? _size = options.Value.Size;
    private readonly string? _style = options.Value.Style;

    protected override OpenAIClient Client { get; init; } = new(options.Value.ApiKey);

    public override Task<string> GenerateImageAsync(string prompt, CancellationToken cancellationToken = default)
    {
        return GenerateImageAsync(_deploymentName, prompt, _size, _quality, _style, cancellationToken);
    }
}

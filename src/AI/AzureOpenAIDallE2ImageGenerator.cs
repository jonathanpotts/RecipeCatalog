using System.Net.Http.Json;
using JonathanPotts.RecipeCatalog.AI.Models;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace JonathanPotts.RecipeCatalog.AI;

public class AzureOpenAIDallE2ImageGenerator : IAIImageGenerator
{
    private const string _startGenerationUrl = "/openai/images/generations:submit";
    private const string _imageOperationUrl = "/openai/operations/images";
    private const string _apiVersion = "2023-08-01-preview";

    private readonly HttpClient _client;
    private readonly int _maxRetries;
    private readonly int _retryDelay;

    public AzureOpenAIDallE2ImageGenerator(IOptions<AzureOpenAIDallE2ImageGeneratorOptions> options)
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri(options.Value.Endpoint!)
        };

        _client.DefaultRequestHeaders.Add("api-key", options.Value!.ApiKey);

        _maxRetries = options.Value.MaxRetries ?? 5;
        _retryDelay = options.Value.RetryDelay ?? 1000;
    }

    public async Task<string> GenerateImageAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var operationId = await StartGenerationAsync(prompt, cancellationToken);
        var imageUrl = await GetGeneratedImageAsync(operationId, cancellationToken);

        return imageUrl;
    }

    private async Task<string> StartGenerationAsync(string prompt, CancellationToken cancellationToken)
    {
        HttpRequestMessage request = new(
            HttpMethod.Post,
            $"{_startGenerationUrl}?api-version={_apiVersion}")
        {
            Content = JsonContent.Create(new AzureOpenAIImageGenerationRequest { Prompt = prompt })
        };

        var response = await _client.SendAsync(request, cancellationToken);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<AzureOpenAIImageGenerationResponse>(cancellationToken);

        return content?.Id ?? throw new Exception("Start generation request failed");
    }

    private async Task<string> GetGeneratedImageAsync(string operationId, CancellationToken cancellationToken)
    {
        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromMilliseconds(_retryDelay),
                MaxRetryAttempts = _maxRetries
            })
            .Build();

        var state = new
        {
            OperationId = operationId,
            Client = _client
        };

        return await pipeline.ExecuteAsync(static async (state, token) =>
        {
            HttpRequestMessage request = new(
                HttpMethod.Get,
                $"{_imageOperationUrl}/{state.OperationId}?api-version={_apiVersion}");

            var response = await state.Client.SendAsync(request, token);

            response.EnsureSuccessStatusCode();

            var content =
                await response.Content.ReadFromJsonAsync<AzureOpenAIImageOperationResponse>(token);

            if (string.Equals(content?.Status, AzureOpenAIImageOperationStatus.Cancelled,
                    StringComparison.OrdinalIgnoreCase)
                || string.Equals(content?.Status, AzureOpenAIImageOperationStatus.Failed,
                    StringComparison.OrdinalIgnoreCase)
                || string.Equals(content?.Status, AzureOpenAIImageOperationStatus.Deleted,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new OperationCanceledException($"The operation did not complete: {content?.Status}");
            }

            if (!string.Equals(content?.Status, AzureOpenAIImageOperationStatus.Succeeded,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("The operating is still pending");
            }

            return content?.Result?.Data?.FirstOrDefault()?.Url
                   ?? throw new OperationCanceledException("The returned content is invalid");
        }, state, cancellationToken);
    }
}

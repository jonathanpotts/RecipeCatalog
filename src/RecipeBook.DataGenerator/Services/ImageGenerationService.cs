using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using RecipeBook.DataGenerator.Services.Models;

namespace RecipeBook.DataGenerator.Services;

public sealed class ImageGenerationService : IDisposable
{
    private const string _startGenerationUrl = "/openai/images/generations:submit";
    private const string _imageOperationUrl = "/openai/operations/images";
    private const string _apiVersion = "2023-08-01-preview";
    private const int _defaultMaxRetries = 5;
    private readonly HttpClient _client;
    private readonly int _maxRetries;

    public ImageGenerationService(IOptions<ImageGenerationServiceOptions> options)
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri(options.Value.Endpoint!)
        };

        _client.DefaultRequestHeaders.Add("api-key", options.Value!.ApiKey);

        _maxRetries = options.Value.MaxRetries ?? _defaultMaxRetries;
    }

    public void Dispose()
    {
        _client.Dispose();
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
        var retries = 0;

        while (true)
        {
            if (retries == _maxRetries)
            {
                throw new Exception("Max retry attempts reached");
            }

            cancellationToken.ThrowIfCancellationRequested();

            HttpRequestMessage request = new(
                HttpMethod.Get,
                $"{_imageOperationUrl}/{operationId}?api-version={_apiVersion}");

            var response = await _client.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var content =
                    await response.Content.ReadFromJsonAsync<AzureOpenAIImageOperationResponse>(cancellationToken);

                if (string.Equals(content?.Status, AzureOpenAIImageOperationStatus.Succeeded,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return content?.Result?.Data?.FirstOrDefault()?.Url
                           ?? throw new Exception("The returned content is invalid");
                }

                if (string.Equals(content?.Status, AzureOpenAIImageOperationStatus.Cancelled,
                        StringComparison.OrdinalIgnoreCase)
                    || string.Equals(content?.Status, AzureOpenAIImageOperationStatus.Failed,
                        StringComparison.OrdinalIgnoreCase)
                    || string.Equals(content?.Status, AzureOpenAIImageOperationStatus.Deleted,
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception($"The operation did not complete: {content?.Status}");
                }
            }

            if (response.Headers.TryGetValues("retry-after", out var values) &&
                int.TryParse(values.FirstOrDefault(), out var retryAfter))
            {
                await Task.Delay(retryAfter * 1000, cancellationToken);
            }

            retries++;
        }
    }
}

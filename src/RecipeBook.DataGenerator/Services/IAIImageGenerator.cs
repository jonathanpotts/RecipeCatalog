namespace RecipeBook.DataGenerator.Services;

public interface IAIImageGenerator
{
    public Task<string> GenerateImageAsync(string prompt, CancellationToken cancellationToken = default);
}

namespace RecipeBook.DataGenerator.Services;

public interface IImageGenerator
{
    public Task<string> GenerateImageAsync(string prompt, CancellationToken cancellationToken = default);
}

namespace JonathanPotts.RecipeBook.AIDataGenerator.Services;

public interface IAIImageGenerator
{
    public Task<string> GenerateImageAsync(string prompt, CancellationToken cancellationToken = default);
}

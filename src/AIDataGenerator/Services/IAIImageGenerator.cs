namespace JonathanPotts.RecipeCatalog.AIDataGenerator.Services;

public interface IAIImageGenerator
{
    public Task<string> GenerateImageAsync(string prompt, CancellationToken cancellationToken = default);
}

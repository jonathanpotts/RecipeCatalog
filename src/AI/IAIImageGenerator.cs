namespace JonathanPotts.RecipeCatalog.AI;

public interface IAIImageGenerator
{
    public Task<string> GenerateImageAsync(string prompt, CancellationToken cancellationToken = default);
}

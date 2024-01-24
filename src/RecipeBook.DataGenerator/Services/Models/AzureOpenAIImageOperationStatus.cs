namespace RecipeBook.DataGenerator.Services.Models;

public static class AzureOpenAIImageOperationStatus
{
    public const string NotRunning = "notRunning";

    public const string Running = "running";

    public const string Succeeded = "succeeded";

    public const string Cancelled = "cancelled";

    public const string Failed = "failed";

    public const string Deleted = "deleted";
}

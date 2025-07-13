namespace Nanobin.Components.Models;

public class Paste
{
    public required string Id { get; init; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string Content { get; init; }
    public required DateTime ExpiresAt { get; init; }

    public string GetFormattedTimestamp()
    {
        return ExpiresAt.ToString("yyyy-MM-dd HH:mm:ss UTC");
    }
}
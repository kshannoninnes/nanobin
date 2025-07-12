namespace Nanobin.Components.Models;

public class Paste
{
    public required string Id { get; init; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string Content { get; init; }
    public required DateTime CreatedAt { get; init; }

    public string GetFormattedTimestamp()
    {
        return CreatedAt.ToString("yyyy-MM-dd HH:mm:ss UTC");
    }
}
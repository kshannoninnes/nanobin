using System.ComponentModel.DataAnnotations;

namespace Nanobin.Components.Models;

public class Paste(string content)
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..18];
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string Content { get; init; } = content[..100000];
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public string GetFormattedTimestamp()
    {
        return CreatedAt.ToString("yyyy-MM-dd HH:mm:ss UTC");
    }
}

using System.Buffers.Text;

namespace Nanobin.Components.Models;

public class Paste(string content)
{
    public string Id { get; init; } = GenerateId();
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string Content { get; init; } = new(content.Take(100000).ToArray());
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public string GetFormattedTimestamp()
    {
        return CreatedAt.ToString("yyyy-MM-dd HH:mm:ss UTC");
    }

    private static string GenerateId()
    {
        var guid = Guid.NewGuid();
        return Base64Url.EncodeToString(guid.ToByteArray());
    }
}
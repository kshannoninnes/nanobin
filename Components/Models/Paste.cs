namespace Nanobin.Components.Models;

public class Paste
{
    public string Id { get; } = Guid.NewGuid().ToString("N")[..8];
    public string Content { get; set; } = string.Empty;
}

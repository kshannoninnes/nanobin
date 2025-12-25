namespace Nanobin.API.Model;

public record Paste
{
    public string Id { get; init; } = "";

    public byte[] Ciphertext { get; init; } = [];

    public byte[] Iv { get; init; } = [];

    public DateTimeOffset CreatedAtUtc { get; init; }

    public DateTimeOffset ExpiresAtUtc { get; init; }
}
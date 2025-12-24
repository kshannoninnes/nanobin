namespace Nanobin.API.Data;

public record Paste(
    string Id,
    byte[] Ciphertext,
    byte[] Iv,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset ExpiresAtUtc);
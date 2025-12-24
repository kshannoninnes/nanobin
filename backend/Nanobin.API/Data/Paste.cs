namespace Nanobin.API.Data;

public record Paste(
    string Id,
    byte[] Ciphertext,
    byte[] Iv,
    byte[] Salt,
    string ContentType,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset ExpiresAtUtc
);
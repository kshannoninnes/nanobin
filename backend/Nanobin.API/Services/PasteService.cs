using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Nanobin.API.Data;
using Nanobin.API.Model;

namespace Nanobin.API.Services;

public sealed class PasteService(NanobinDbContext dbContext, IConfiguration configuration)
{
    private readonly int _defaultTtlDays = configuration.GetValue("Nanobin:DefaultTtlDays", 7);

    public async Task<(string Id, DateTimeOffset ExpiresAtUtc)> CreateAsync(
        string ciphertextBase64,
        string ivBase64,
        CancellationToken cancellationToken = default)
    {
        var ciphertext = DecodeBase64(ciphertextBase64);
        var iv = DecodeBase64(ivBase64);
        var now = DateTimeOffset.UtcNow;
        var expires = now.AddDays(_defaultTtlDays);

        for (var attemptIndex = 0; attemptIndex < 3; attemptIndex++)
        {
            var id = GeneratePasteId();

            var paste = new Paste
            {
                Id = id,
                Ciphertext = ciphertext,
                Iv = iv,
                CreatedAtUtc = now,
                ExpiresAtUtc = expires
            };

            dbContext.Pastes.Add(paste);

            try
            {
                await dbContext.SaveChangesAsync(cancellationToken);
                return (id, expires);
            }
            catch (DbUpdateException)
            {
                // In the rare case of an id collision
                dbContext.ChangeTracker.Clear();
            }
        }

        throw new Exception("Failed to allocate paste id");
    }

    public async Task<(string Ciphertext, string Iv)> GetAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var paste = await dbContext.Pastes
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (paste is null)
            throw new PasteNotFoundException();
        
        var base64Ciphertext = Convert.ToBase64String(paste.Ciphertext);
        var base64Iv = Convert.ToBase64String(paste.Iv);

        if (paste.ExpiresAtUtc > DateTimeOffset.UtcNow) 
            return (base64Ciphertext, base64Iv);
        
        // Paste expiry time was invalid, delete it and throw exception
        await dbContext.Pastes
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        throw new PasteExpiredException();
    }

    private static byte[] DecodeBase64(string value)
    {
        try
        {
            return Convert.FromBase64String(value);
        }
        catch (FormatException)
        {
            throw new InvalidPasteException($"Could not decode value {value}");
        }
    }

    private static string GeneratePasteId()
    {
        // 9 bytes converts to 12 base64 characters for the url
        // 1 in 9 billion collision risk at 1 million pastes
        var bytes = new byte[9];
        RandomNumberGenerator.Fill(bytes);
        return WebEncoders.Base64UrlEncode(bytes);
    }
}
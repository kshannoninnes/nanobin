using System.Buffers.Text;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Nanobin.Components.Models;
using Nanobin.Config;
using Nanobin.Exceptions;

namespace Nanobin.Components.Services;

public class PasteService(NanobinDbContext dbContext, ILogger<PasteService> logger)
{
    private static readonly TimeSpan MaxAge = TimeSpan.FromDays(7);
    
    public async Task<string> CreatePasteAsync(string content)
    {
        var newPaste = new Paste
        {
            Id = await GenerateIdAsync(),
            Content = new string(content.Take(100000).ToArray()),
            ExpiresAt = DateTime.UtcNow + MaxAge
        };
        
        logger.LogDebug("Paste created, expiry set for {PasteExpiry}", newPaste.ExpiresAt.ToString("yyyy-MM-dd HH:mm:ss UTC"));
        
        try
        {
            logger.LogDebug("Adding new paste with id {PasteId} to database", newPaste.Id);
            await dbContext.Pastes.AddAsync(newPaste);
            await dbContext.SaveChangesAsync();

            return newPaste.Id;
        }
        catch (DbException e)
        {
            logger.LogDebug(e, "Database error occurred while creating paste with ID: {PasteId}", newPaste.Id);
            throw new IOException("Error creating paste. See debug logs for details.");
        }
    }

    public async Task<Paste> GetPasteAsync(string id)
    {
        try
        {
            logger.LogDebug("Searching for paste with id {PasteId}", id);
            var paste = await dbContext.Pastes
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && p.ExpiresAt > DateTime.UtcNow);

            if (paste is null)
            {
                throw new PasteNotFoundException($"No paste matching ID {id} found");
            }
            
            logger.LogDebug("Paste with id {PasteId} found", paste.Id);

            return paste;
        }
        catch (DbException e)
        {
            throw new IOException("Error retrieving paste. See debug logs for details.");
        }
    }

    private async Task<string> GenerateIdAsync()
    {
        string id;
        
        do
        {
            var guid = Guid.NewGuid();
            id = Base64Url.EncodeToString(guid.ToByteArray())[..8];
            logger.LogDebug("Generated new paste ID: {PasteId}", guid);
        } while (await dbContext.Pastes.AnyAsync(p => p.Id == id));

        return id;
    }

    public async Task<int> DeleteExpiredPastesAsync()
    {
        try
        {
            var expiredPastes = await dbContext.Pastes
                .Where(p => p.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();
            
            logger.LogDebug("Found {Count} expired pastes to delete", expiredPastes.Count);

            if (expiredPastes.Count == 0)
            {
                return 0;
            }

            dbContext.Pastes.RemoveRange(expiredPastes);
            await dbContext.SaveChangesAsync();

            return expiredPastes.Count;
        }
        catch (DbException e)
        {
            logger.LogDebug(e, "Error deleting expired pastes");
            throw new IOException("Error deleting expired pastes. See debug logs for details.");
        }
    }
}
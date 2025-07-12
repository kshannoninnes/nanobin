using System.Buffers.Text;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Nanobin.Components.Models;
using Nanobin.Config;
using Nanobin.Exceptions;

namespace Nanobin.Components.Services;

public class PasteService(NanobinDbContext dbContext, ILogger<PasteService> logger)
{
    public async Task<string> CreatePasteAsync(string content)
    {
        var newPaste = new Paste
        {
            Id = await GenerateIdAsync(),
            Content = CleanContent(content),
            CreatedAt = DateTime.UtcNow
        };
        
        try
        {
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
            var paste = await dbContext.Pastes
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paste is null)
            {
                throw new PasteNotFoundException($"No paste matching ID {id} found");
            }

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
            var base64Urlid = Base64Url.EncodeToString(guid.ToByteArray());
            id = base64Urlid[..8];
        } while (await dbContext.Pastes.AnyAsync(p => p.Id == id));

        return id;
    }

    private static string CleanContent(string content)
    {
        var fixedIndentation = FixIndentation(content);
        var lengthTruncated = fixedIndentation.Take(100000).ToArray();

        return new string(lengthTruncated);
    }
    
    private static string FixIndentation(string text)
    {
        var lines = text.Replace("\r\n", "\n").Split('\n');
        var nonEmptyLines = lines.Where(line => line.Trim().Length > 0).ToArray();
        var minIndent = nonEmptyLines.Length != 0
            ? nonEmptyLines.Min(line => line.TakeWhile(char.IsWhiteSpace).Count())
            : 0;

        return string.Join("\n", lines.Select(line =>
            line.Length >= minIndent ? line[minIndent..] : line
        ));
    }
}

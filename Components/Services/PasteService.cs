using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Nanobin.Components.Models;
using Nanobin.Data;

namespace Nanobin.Components.Services;

public class PasteService(NanobinDbContext dbContext)
{
    public async Task<string> CreatePasteAsync(string content)
    {
        try
        {
            var newPaste = new Paste(FixIndentation(content));

            await dbContext.Pastes.AddAsync(newPaste);
            await dbContext.SaveChangesAsync();

            return newPaste.Id;
        }
        catch (DbException e)
        {
            throw new IOException("Error creating paste. See debug logs for details.");
        }
    }

    public async Task<Paste?> GetPasteAsync(string id)
    {
        try
        {
            return await dbContext.Pastes
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        catch (DbException e)
        {
            throw new IOException("Error retrieving paste. See debug logs for details.");
        }
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

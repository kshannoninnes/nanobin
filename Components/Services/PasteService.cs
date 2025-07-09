using Nanobin.Components.Models;

namespace Nanobin.Components.Services;

public class PasteService
{
    private readonly Dictionary<string, Paste> _pastes = new();

    public void CreatePaste(Paste paste)
    {
        paste.Content = Unindent(paste.Content);
        _pastes[paste.Id] = paste;
    }

    public Paste? GetPaste(string id)
    {
        return _pastes.GetValueOrDefault(id);
    }
    
    private static string Unindent(string text)
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

using System.Collections.Generic;
using System.Linq;

namespace DebScraper;

public static class Ligen
{
    public static List<Guid> ReadFileLines(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Die Datei {filePath} wurde nicht gefunden.");
        }

        var guids = File.ReadAllLines(filePath)
            .Where(line => !line.StartsWith("---") && !string.IsNullOrWhiteSpace(line))
            .Select(line => line.Replace("\"", "").Trim())
            .Select(id =>
            {
                if (Guid.TryParse(id, out var guid))
                {
                    return guid;
                }

                return Guid.Empty;
            })
            .Where(guid => guid != Guid.Empty)
            .ToList();

        return guids;
    }
}
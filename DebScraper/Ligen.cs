using System.Collections.Generic;
using System.Linq;

namespace DebScraper;

public static class Ligen
{
    public static List<string> ReadFileLines(string filePath)
    {
        // Überprüfen Sie, ob die Datei existiert, bevor Sie versuchen, sie zu lesen
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Die Datei {filePath} wurde nicht gefunden.");
        }

        // Lesen Sie alle Zeilen der Datei und geben Sie sie als String-Array zurück
        string[] lines = File.ReadAllLines(filePath);
        List<string> allLines = new List<string>();
        foreach (var line in lines)
        {
            if (!line.StartsWith("---"))
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    allLines.Add(line.Replace("\"", ""));
                }
            }
        }

        return allLines;
    }
}

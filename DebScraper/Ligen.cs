using System.Collections.Generic;
using System.Linq;

namespace DebScraper;

public static class Ligen
{
    public static List<Guid> ReadFileLines(string filePath)
    {
        // Überprüfen Sie, ob die Datei existiert, bevor Sie versuchen, sie zu lesen
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Die Datei {filePath} wurde nicht gefunden.");
        }

        // Lesen Sie alle Zeilen der Datei und geben Sie sie als String-Array zurück
        string[] lines = File.ReadAllLines(filePath);
        List<Guid> allLines = new List<Guid>();
        foreach (var line in lines)
        {
            if (!line.StartsWith("---"))
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    try
                    {
                        var id = line.Replace("\"", "");
                        var guid = Guid.Parse(id);
                        allLines.Add(guid);
                    } 
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }
        return allLines;
    }
}

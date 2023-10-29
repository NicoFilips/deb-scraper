namespace DebScraper;

public record Game
{
    DateTime date { get; set; }
    string liga { get; set; }
    string homeTeam { get; set; }
    Guid Guid { get; set; }
}

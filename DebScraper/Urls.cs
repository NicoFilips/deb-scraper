namespace DebScraper;

public record liga
{
    public string name { get; set; }
    public string sourceUrl { get; set; }
    public List<Guid> Ids { get; set; } = new List<Guid>();
}

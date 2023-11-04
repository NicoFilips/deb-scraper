namespace DebScraper;

public record Game
{
    public DateTime date { get; set; }
    public string liga { get; set; }
    public string ort { get; set; }
    public Guid Guid { get; set; }
    public string divId { get; set; }
    public string url { get; set; }
}

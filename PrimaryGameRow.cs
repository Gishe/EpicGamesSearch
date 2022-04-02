namespace EpicGamesSearch;

public class PrimaryGameRow
{
    public int AppId { get; set; }
    public int Ranking { get; set; }
    public string? GameName { get; set; }
    public int CurrentPlayers { get; set; }
    public int PeakInLast24Hours { get; set; }
    public int PeakPlayers { get; set; }
    public double? InitialPrice { get; set; }
    public double? Price { get; set; }
    public string? Engine { get; set; }
    public bool? IndyGame { get; set; }
    public string LowOwners { get; set; }
    public string HighOwners { get; set; }
    public int? RevenueReports { get; set; }
    
}

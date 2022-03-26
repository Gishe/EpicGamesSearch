namespace EpicGamesSearch;

public class PrimaryGameRow
{
    public int Ranking { get; set; }
    public string? GameName { get; set; }
    public int CurrentPlayers { get; set; }
    public int PeakInLast24Hours { get; set; }
    public int PeakPlayers { get; set; }
    public int Price { get; set; }
    public string? Engine { get; set; }
    public int Sales { get; set; }
    public int RevenueReports { get; set; }
}

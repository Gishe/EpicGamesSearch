using System.Text.Json;

namespace EpicGamesSearch;

public class SteamApi
{
    private const string apiDataFile = "SteamAPIData.txt";
    private readonly Dictionary<string, int> SteamLookup = new Dictionary<string, int>();
    
    private async Task Init()
    {
        await using var stream = File.OpenRead(apiDataFile);
        var bytes = await File.ReadAllBytesAsync(apiDataFile);
        var jsonReader = new Utf8JsonReader(bytes);

        while (jsonReader.Read())
        {
            
        }
        

    }
    
    
    
}
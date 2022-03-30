using System.Text.Json;
using Newtonsoft.Json;

namespace EpicGamesSearch;

public class SteamApi
{
    private const string apiDataFile = "SteamAPIData.txt";
    private readonly Dictionary<string, int> SteamLookup = new Dictionary<string, int>();

    public static async Task<SteamApi> Create()
    {
        var api = new SteamApi();
        await api.Init();
        return api;
    }

    private SteamApi()
    {
    }
    
    private async Task Init()
    {
        await using var stream = File.OpenRead(apiDataFile);
        var data = await File.ReadAllTextAsync(apiDataFile);
        dynamic parsedJson = JsonConvert.DeserializeObject(data) ?? throw new InvalidOperationException();

        foreach (var app in parsedJson.applist.apps)
        {
            if (Int32.TryParse(app.appid.ToString(), out int appId))
            {
                SteamLookup[app.name.ToString().ToLower()] = appId;
            }
            
        }

    }


    public int FindAppId(string? valueGameName)
    {
        var lower = valueGameName.ToLower();
        return SteamLookup.ContainsKey(lower) ? SteamLookup[lower] : 0;
    }
}
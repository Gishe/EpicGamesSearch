using System.Text.Json;
using Newtonsoft.Json;

namespace EpicGamesSearch;

public class SteamApi : FileAndApiBase
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
    
    protected override string ApiFileName => apiDataFile;
    protected override void ProcessJsonData(dynamic parsedJson)
    {
        foreach (var app in parsedJson.applist.apps)
        {
            if (Int32.TryParse(app.appid.ToString(), out int appId))
            {
                SteamLookup[app.name.ToString().ToLower()] = appId;
            }
        }
    }

    protected override int CallsPerSecond => 300;

    public override Task LoadData(PrimaryGameRow row)
    {
        row.AppId = FindAppId(row.GameName);
        return Task.CompletedTask;
    }

    private int FindAppId(string? valueGameName)
    {
        var lower = valueGameName.ToLower();
        return SteamLookup.ContainsKey(lower) ? SteamLookup[lower] : 0;
    }
}
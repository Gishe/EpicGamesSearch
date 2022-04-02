using System.Globalization;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;

namespace EpicGamesSearch;

public class SteamSpy : FileAndApiBase, IDisposable
{
    private const string baseUri = "https://steamspy.com";
    private const string steamSpyApi = "/api.php?request=all&page=0";
    private const string requestDetails = "/api.php?request=appdetails&appid=";
    private const string apiDataFile = "SteamSpyAll0.json";
    
    private readonly Dictionary<int, dynamic> SpyLookup = new Dictionary<int, dynamic>(); 
    
    private readonly HttpClient _client;

    public static async Task<SteamSpy> Create()
    {
        var api = new SteamSpy();
        await api.Init();
        return api;
    }
    
    private SteamSpy()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri(baseUri)
        };
    }


    public async Task<IList<SteamSpyResult>> GetData()
    {
        var response = _client.GetAsync(steamSpyApi).Result;
        response.EnsureSuccessStatusCode();

        var json = response.Content.ReadAsStringAsync().Result;

        dynamic parsedJson = JsonConvert.DeserializeObject(json);

        return new List<SteamSpyResult>();
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    protected override string ApiFileName => apiDataFile;

    protected override void ProcessJsonData(dynamic parsedJson)
    {
        foreach (var app in parsedJson)
        {
            var data = app.First;
            if (int.TryParse(data.appid.ToString(),out int appId))
            {
                SpyLookup[appId] = data;
            }
        }
    }

    protected override int CallsPerSecond => 1;

    public override async Task LoadData(PrimaryGameRow row)
    {
        // if (SpyLookup.ContainsKey(row.AppId))
        // {
        //     LoadDataFromDictionary(row);
        //     return;
        // }

        await PauseForRequestLimit();
        await LoadDataFromWeb(row);
    }

    private async Task LoadDataFromWeb(PrimaryGameRow row)
    {
        var response = await _client.GetAsync(requestDetails + row.AppId);
        response.EnsureSuccessStatusCode();

        dynamic jsonData = JsonConvert.DeserializeObject( await response.Content.ReadAsStringAsync()) ?? throw new InvalidOperationException();

        LoadSharedData(row, jsonData);

        row.IndyGame = jsonData.ToString().IndexOf("Indie") > 0;
    }

    private void LoadDataFromDictionary(PrimaryGameRow row)
    {
        var data = SpyLookup[row.AppId];
        LoadSharedData(row, data);
    }

    private static void LoadSharedData(PrimaryGameRow row, dynamic data)
    {
        row.Price = data.price / 100d;
        row.InitialPrice = data.initialprice / 100d;

        string[] ownerData = data.owners.ToString().Split("..");
        if (ownerData.Length > 1)
        {
            row.LowOwners = ownerData[0];
            row.HighOwners = ownerData[1];
        }
    }
}
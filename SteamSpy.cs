using Newtonsoft.Json;

namespace EpicGamesSearch;

public class SteamSpy
{
    private const string baseUri = "https://steamspy.com";
    const string steamSpyApi = "/api.php?request=all&page=1";
    
    private readonly HttpClient _client;

    public SteamSpy()
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
}
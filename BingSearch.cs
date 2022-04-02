using EpicGamesSearch.Qualifiers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace EpicGamesSearch;

public class BingSearch : ApiBase, IDisposable
{
    private readonly HttpClient _client;

    private readonly ListQualifier _engineQualifier;

    public readonly Dictionary<string, IList<SearchResult>> SearchResults = new();

    public BingSearch()
    {
        var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        var subscriptionKey = config["Bing:SubscriptionKey"];
        var endpoint = "https://api.bing.microsoft.com/v7.0/search";

        _client = new HttpClient
        {
            BaseAddress = new Uri(endpoint)
        };
        _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
        _engineQualifier = new ListQualifier(GameEngines.Values);
    }

    protected override int CallsPerSecond => 3;

    public void Dispose()
    {
        _client.Dispose();
    }

    public async Task<IEnumerable<SearchResult>> Search(string searchGroupName, string searchIdentifier, string search,
        Func<string, IList<string>> qualifier)
    {
        var response = await _client.GetAsync($"?q={Uri.EscapeDataString(search)}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        return ParseJsonData(searchGroupName, searchIdentifier, json, qualifier);
    }

    private IEnumerable<SearchResult> ParseJsonData(string searchGroupName, string searchIdentifier, string s,
        Func<string, IList<string>> qualifier)
    {
        dynamic parsedJson = JsonConvert.DeserializeObject(s) ?? throw new InvalidOperationException();
        foreach (var webPage in parsedJson.webPages.value)
        {
            IList<string> result = qualifier(webPage.snippet.ToString());
            var jsonString = JsonConvert.SerializeObject(webPage, Formatting.Indented);

            Console.WriteLine(jsonString);
            if (result.Count > 0)
            {
                SearchResult searchResult = JsonConvert.DeserializeObject<SearchResult>(jsonString);
                searchResult.GameName = searchIdentifier;
                searchResult.result = result.GroupBy(s => s).Select(g => new {Value = g.Key, Count = g.Count()})
                    .OrderByDescending(x => x.Count).FirstOrDefault()?.Value;
                AddToSearchResults(searchGroupName, searchResult);
                yield return searchResult;
            }
        }
    }

    private void AddToSearchResults(string searchIdentifier, SearchResult searchResult)
    {
        if (!SearchResults.ContainsKey(searchIdentifier))
        {
            SearchResults[searchIdentifier] = new List<SearchResult>();
        }

        SearchResults[searchIdentifier].Add(searchResult);
    }

    public override async Task LoadData(PrimaryGameRow row)
    {
        await PauseForRequestLimit();
        var searchResult = await Search( "GameEngineSearch", row.GameName, $"{row.GameName} Game Engine", _engineQualifier.Qualifier);

        row.Engine = searchResult.GroupBy(s => s.result).Select(g => new {Value = g.Key, Count = g.Count()})
            .OrderByDescending(x => x.Count).FirstOrDefault()?.Value;
    }
}
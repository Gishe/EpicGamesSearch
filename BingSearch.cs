using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace EpicGamesSearch;

public class BingSearch
{
    private readonly HttpClient _client;

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
    }
   
    
    
    public IEnumerable<SearchResult> Search(string searchIdentifier, string search, Func<string, IList<string>> qualifier)
    {
        var response = _client.GetAsync($"?q={Uri.EscapeDataString(search)}").Result;
        response.EnsureSuccessStatusCode();

        var json = response.Content.ReadAsStringAsync().Result;
        
        foreach (var o in ParseJsonData(searchIdentifier, json, qualifier)) yield return o;
    }

    private IEnumerable<SearchResult> ParseJsonData(string searchIdentifier, string s, Func<string, IList<string>> qualifier)
    {
        dynamic parsedJson = JsonConvert.DeserializeObject(s) ?? throw new InvalidOperationException();
        foreach (var webPage in parsedJson.webPages.value)
        {
            var result = qualifier(webPage.snippet.ToString());
            var jsonString = JsonConvert.SerializeObject(webPage, Formatting.Indented);
            
            Console.WriteLine(jsonString);
            Console.WriteLine(
                $" ------------ Found {result.Count} results {string.Join(",", result)} ------------------------");
            if (result.Count > 0)
            {
                var searchResult = JsonConvert.DeserializeObject<SearchResult>(jsonString);
                searchResult.GameName = searchIdentifier;
                searchResult.result = string.Join(",", result);
                yield return searchResult;
            }
        }
    }
}
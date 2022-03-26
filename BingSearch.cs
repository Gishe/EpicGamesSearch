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
        string subscriptionKey = config["Bing:SubscriptionKey"];
        string endpoint = "https://api.bing.microsoft.com/v7.0/search";

        _client = new HttpClient()
        {
            BaseAddress = new Uri(endpoint)
        };
        _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
    }

    public IEnumerable<SearchResult> Search( string search, Regex regularExpression)
    {
        var response = _client.GetAsync($"?q={Uri.EscapeDataString(search)}").Result;
        response.EnsureSuccessStatusCode();

        string json = response.Content.ReadAsStringAsync().Result;
    
    
    
        foreach (var o in ParseJsonData(json, regularExpression)) yield return o;
    }

    private IEnumerable<SearchResult> ParseJsonData(string s, Regex regex)
    {
        dynamic parsedJson = JsonConvert.DeserializeObject(s) ?? throw new InvalidOperationException();
        foreach (var webPage in parsedJson.webPages.value)
        {
            MatchCollection foundResult = regex.Matches(webPage.snippet.ToString());
            var jsonString = JsonConvert.SerializeObject(webPage, Formatting.Indented); 
            Console.WriteLine(jsonString);
            Console.WriteLine(
                $" ------------ Found {foundResult.Count} results {string.Join(",", foundResult.Select(m => m.ToString()))} ------------------------");
            if (foundResult.Count > 0)
            {
                yield return JsonConvert.DeserializeObject<SearchResult>(jsonString);
            }
        }
    }
}
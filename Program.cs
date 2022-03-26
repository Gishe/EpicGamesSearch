// See https://aka.ms/new-console-template for more information


using System.Text.RegularExpressions;
using EpicGamesSearch;
using Ganss.Excel;
using Google.Apis.CustomSearchAPI.v1.Data;
using static System.Console;


const string query = "Elden Ring sales";
const string excelFile = "TopSellingGames.xlsx";

var searchClient = new BingSearch();

//var successful = searchClient.Search(query, new Regex(@"[,\d]{3,}"));
//WriteLine($"Found {successful.Count()} results");

var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

var filePath = Path.Combine(folder, excelFile);

var excel = new ExcelMapper();
excel.NormalizeUsing(n => Regex.Replace(n, "\\s", ""));
var gameRows = (await excel.FetchAsync<PrimaryGameRow>(filePath, 2)).ToList();
var allResults = new List<SearchResult>();
for(int i = 0; i < 10; i++)
{
    var game = gameRows[i];
    WriteLine($"Starting {game.GameName}");

    var results = searchClient.Search($"{game.GameName} Sales", new Regex(@"[,\d]{3,}"));
    //allResults.AddRange(results);
    allResults.Add(new SearchResult(){name = game.GameName});
    await Task.Delay(400); // Using the slow version for testing
}

await excel.SaveAsync(filePath, allResults, 3);

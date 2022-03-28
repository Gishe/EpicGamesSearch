// See https://aka.ms/new-console-template for more information


using System.Text.RegularExpressions;
using EpicGamesSearch;
using EpicGamesSearch.Qualifiers;
using Google.Apis.CustomSearchAPI.v1.Data;
using Npoi.Mapper;
using static System.Console;


const string excelFile = "TopSellingGames.xlsx";
const string excelOut = "Output.xlsx";

var searchClient = new BingSearch();

//var successful = searchClient.Search(query, new Regex(@"[,\d]{3,}"));
//WriteLine($"Found {successful.Count()} results");

var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

var filePath = Path.Combine(folder, excelFile);
var outPath = Path.Combine(folder, excelOut);

var excel = new Mapper(filePath);

var gameRows = excel.Take<PrimaryGameRow>(2).ToList();
var allResults = new List<SearchResult>();

var regexQualifier = new RegexQualifier(@"[,\d]{3,}");

var engineQualifier = new ListQualifier(GameEngines.Values);
    

for(int i = 0; i < 20; i++)
{
    var game = gameRows[i];
    WriteLine($"Starting {game.Value.GameName}");

    var results = searchClient.Search(game.Value.GameName, $"{game.Value.GameName} Game Engine", engineQualifier.Qualifier);
    allResults.AddRange(results);
    //allResults.Add(new SearchResult(){name = game.Value.GameName});
    await Task.Delay(400); // Using the slow version for testing
}

excel.Put(allResults, "EngineSearch", true);
excel.Save(outPath);

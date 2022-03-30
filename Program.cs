// See https://aka.ms/new-console-template for more information


using System.Text.RegularExpressions;
using EpicGamesSearch;
using EpicGamesSearch.Qualifiers;
using Google.Apis.CustomSearchAPI.v1.Data;
using Npoi.Mapper;
using static System.Console;


const string excelFile = "InputV1.xlsx";
const string excelOut = "Output.xlsx";
const string mainSheetName = "Todays Top Played Games";


// var steamSpy = new SteamSpy();
//
// var steamSpyResults = await steamSpy.GetData();
//
// foreach (var result in steamSpyResults)
// {
//     Console.WriteLine(result);
// }

var steamApi = await SteamApi.Create();
var searchClient = new BingSearch();

//var successful = searchClient.Search(query, new Regex(@"[,\d]{3,}"));
//WriteLine($"Found {successful.Count()} results");

var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

var filePath = Path.Combine(folder, excelFile);
var outPath = Path.Combine(folder, excelOut);

var excel = new Mapper(filePath);
excel.TrackObjects = true;

var gameRows = excel.Take<PrimaryGameRow>(mainSheetName).ToList();

foreach (var rowInfo in gameRows)
{
    rowInfo.Value.AppId = steamApi.FindAppId(rowInfo.Value.GameName);
}

excel.Save(outPath);

async Task HandleSearchResults(List<RowInfo<PrimaryGameRow>> rows)
{
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
}



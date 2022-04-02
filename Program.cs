// See https://aka.ms/new-console-template for more information


using EpicGamesSearch;
using Npoi.Mapper;

const string excelFile = "InputV1.xlsx";
const string excelOut = "Output.xlsx";
const string mainSheetName = "Todays Top Played Games";


using var steamSpy = await SteamSpy.Create();

var steamApi = await SteamApi.Create();
using var searchClient = new BingSearch();

var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

var filePath = Path.Combine(folder, excelFile);
var outPath = Path.Combine(folder, excelOut);

var excel = new Mapper(filePath);
excel.TrackObjects = true;

var gameRows = excel.Take<PrimaryGameRow>(mainSheetName).ToList();

for (var i = 0; i < gameRows.Count; i++)
{
    var row = gameRows[i].Value;

    // Already loaded into the input dataset
    //steamApi.LoadData(row);
    if (row.AppId != 0) await steamSpy.LoadData(row);

    await searchClient.LoadData(row);
}

excel.Put(gameRows.Select(g => g.Value), mainSheetName);

foreach (var clientSearchResult in searchClient.SearchResults)
    excel.Put(clientSearchResult.Value, clientSearchResult.Key);

excel.Save(outPath);
using Newtonsoft.Json;

namespace EpicGamesSearch;

public abstract class ApiBase
{
    private DateTime lastRequest = DateTime.MinValue;
    protected abstract int CallsPerSecond { get; }

    protected async Task PauseForRequestLimit()
    {
        var nextCallTime = lastRequest + TimeSpan.FromMilliseconds(1000d / CallsPerSecond);
        if (nextCallTime > DateTime.UtcNow)
        {
            await Task.Delay(nextCallTime - DateTime.UtcNow);
        }
        lastRequest = DateTime.UtcNow;
    }
    
    public abstract Task LoadData(PrimaryGameRow row);
}


public abstract class FileAndApiBase : ApiBase
{
    
    protected async Task Init()
    {
        var data = await File.ReadAllTextAsync(ApiFileName);
        dynamic parsedJson = JsonConvert.DeserializeObject(data) ?? throw new InvalidOperationException();

        ProcessJsonData(parsedJson);
        
    }
    
    protected abstract string ApiFileName { get; }

    protected abstract void ProcessJsonData(dynamic parsedJson);
}
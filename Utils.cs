using System.Dynamic;

namespace EpicGamesSearch;

public class Utils
{
    public static bool HasProperty(dynamic obj, string property)
    {
        if (obj is IDictionary<string, object> dictionary)
        {
            return dictionary.ContainsKey(property);
        }

        return obj.GetType().GetProperty(property) != null;
    }
}
namespace EpicGamesSearch.Qualifiers;

public class ListQualifier
{
    private readonly IList<string> _values;
    public ListQualifier(IList<string> values)
    {
        _values = values;
    }

    public IList<string> Qualifier(string search)
    {
        return _values.Where(v => search.IndexOf(v, StringComparison.CurrentCultureIgnoreCase) != -1).ToList();
    }
    
}
using System.Text.RegularExpressions;

namespace EpicGamesSearch.Qualifiers;

public class RegexQualifier
{
    private Regex _regex;
    public RegexQualifier(string regex)
    {
        _regex = new Regex(regex);
    }
    
    public IList<string> Qualifier(string s)
    {
        MatchCollection foundResult = _regex.Matches(s);

        return foundResult.Select(m => m.ToString()).ToList();
    }
}
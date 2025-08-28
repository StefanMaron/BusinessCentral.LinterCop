using System.Text.RegularExpressions;

namespace BusinessCentral.LinterCop.Helpers;
public static class Pattern
{
    public static Regex? CompilePattern(string? patternString, RegexOptions options = RegexOptions.Compiled | RegexOptions.CultureInvariant)
    {
        if (string.IsNullOrWhiteSpace(patternString))
            return null;

        try
        {
            return new Regex(patternString.Trim(), options, TimeSpan.FromSeconds(2));
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
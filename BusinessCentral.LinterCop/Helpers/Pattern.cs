using System.Text.RegularExpressions;

namespace BusinessCentral.LinterCop.Helpers;
public class Pattern
{
    public static Regex? CompilePattern(string patternString)
    {
        if (patternString != null && patternString != "")
        {
            return new Regex(patternString);
        }

        return null;
    }
}
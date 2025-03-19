using BusinessCentral.LinterCop.Test.RoslynTestKit.Utils;
using Microsoft.Dynamics.Nav.CodeAnalysis;

namespace BusinessCentral.LinterCop.Test.RoslynTestKit;

public class CodeMarkup
{
    public CodeMarkup(string markup)
    {
        Code = markup.Replace("[|", "").Replace("|]", "");
        Locator = GetLocator(markup);
        AllLocators = GetAllLocators(markup);
    }

    private static IDiagnosticLocator GetLocator(string markupCode)
    {
        if (TryFindMarkedTimeSpan(markupCode, out var textSpan))
        {
            return new TextSpanLocator(textSpan);
        }

        throw new Exception("Cannot find any position marked with [||]");
    }

    private static List<IDiagnosticLocator> GetAllLocators(string markupCode)
    {
        List<IDiagnosticLocator> locators = new List<IDiagnosticLocator>();
        int startIndex = 0, markers = 0;
        while (TryFindMarkedTimeSpan(markupCode, ref startIndex, ref markers, out var textSpan))
        {
            locators.Add(new TextSpanLocator(textSpan));
        }

        if (markers == 0)
            throw new Exception("Cannot find any position marked with [||]");

        return locators;
    }

    private static bool TryFindMarkedTimeSpan(string markupCode, out TextSpan textSpan)
    {
        textSpan = default;
        var start = markupCode.IndexOf("[|", StringComparison.InvariantCulture);
        if (start < 0)
        {
            return false;
        }

        var end = markupCode.IndexOf("|]", StringComparison.InvariantCulture);
        if (end < 0)
        {
            return false;
        }

        textSpan = TextSpan.FromBounds(start, end - 2);
        return true;
    }

    private static bool TryFindMarkedTimeSpan(string markupCode, ref int startIndex, ref int markers, out TextSpan textSpan)
    {
        textSpan = default;
        var start = markupCode.IndexOf("[|", startIndex, StringComparison.InvariantCulture);
        if (start < 0)
        {
            return false;
        }

        var end = markupCode.IndexOf("|]", startIndex, StringComparison.InvariantCulture);
        if (end < 0)
        {
            return false;
        }

        // textspans for code without the [| and |] markers
        textSpan = TextSpan.FromBounds(start - (4 * markers), end - (4 * markers) - 2);
        markers++;
        startIndex = end + 2;
        return true;
    }

    public IDiagnosticLocator Locator { get; }

    public List<IDiagnosticLocator> AllLocators { get; }

    public string Code { get; }
}
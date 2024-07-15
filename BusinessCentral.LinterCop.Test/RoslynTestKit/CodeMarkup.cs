using BusinessCentral.LinterCop.Test.RoslynTestKit.Utils;
using Microsoft.Dynamics.Nav.CodeAnalysis;

namespace BusinessCentral.LinterCop.Test.RoslynTestKit;

public class CodeMarkup
{
    public CodeMarkup(string markup)
    {
        Code = markup.Replace("[|", "").Replace("|]", "");
        Locator = GetLocator(markup);
    }

    private static IDiagnosticLocator GetLocator(string markupCode)
    {
        if (TryFindMarkedTimeSpan(markupCode, out var textSpan))
        {
            return new TextSpanLocator(textSpan);
        }

        throw new Exception("Cannot find any position marked with [||]");
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


    public IDiagnosticLocator Locator { get; }

    public string Code { get; }
}
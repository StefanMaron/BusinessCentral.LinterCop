using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces;

namespace BusinessCentral.LinterCop.Test.RoslynTestKit.Utils;

internal class LineLocator : IDiagnosticLocator
{
    private readonly int line;
    private readonly int _startPosition;
    private readonly int _endPosition;

    public LineLocator(int line, int startPosition, int endPosition)
    {
        this.line = line;
        _startPosition = startPosition;
        _endPosition = endPosition;
    }

    public bool Match(Location location)
    {
        var zeroBaseLine = line - 1;
        var lineSpan = location.GetLineSpan();
        return
            location.IsInSource &&
            lineSpan.StartLinePosition.Line <= zeroBaseLine &&
            lineSpan.EndLinePosition.Line >= zeroBaseLine;
    }

    public TextSpan GetSpan()
    {
        return TextSpan.FromBounds(_startPosition, _endPosition);
    }

    public string Description() => $"line {line}";

    public static LineLocator FromCode(string code, int lineNumber)
    {
        var lineStart = NthIndexOf(code, '\n', lineNumber - 1);
        if (lineStart == -1)
        {
            lineStart = 0;
        }

        var lineEnd = NthIndexOf(code, '\n', lineNumber);
        if (lineEnd == -1)
        {
            lineEnd = code.Length;
        }

        return new LineLocator(lineNumber, lineStart + 1, lineEnd - 1);
    }

    private static int NthIndexOf(string text, char value, int expectedOccurence)
    {
        int count = 0;
        for (int position = 0; position < text.Length; position++)
        {
            if (text[position] == value)
            {
                count++;
                if (count == expectedOccurence)
                {
                    return position;
                }
            }
        }

        return -1;
    }

    public static LineLocator FromDocument(Document document, int lineNumber)
    {
        var sourceCode = document.GetSyntaxRootAsync().GetAwaiter().GetResult().ToFullString();
        return FromCode(sourceCode, lineNumber);
    }
}
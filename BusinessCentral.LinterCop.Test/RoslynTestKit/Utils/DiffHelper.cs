using System.Diagnostics;
using System.Text;
using ApprovalTests.Reporters;
using DiffPlex;
using DiffPlex.Chunkers;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace BusinessCentral.LinterCop.Test.RoslynTestKit.Utils;

public static class DiffHelper
{
    public static string GenerateInlineDiff(string expected, string actual)
    {
        var differ = new Differ();
        var diffBuilder = new InlineDiffBuilder(differ);
        var diff = diffBuilder.BuildDiffModel(expected, actual, false, false, new LineEndingsPreservingChunker());

        var sb = new StringBuilder();
        var lastChanged = false;
        int lastLine = 1;
        foreach (var line in diff.Lines)
        {
            if (line.Type != ChangeType.Unchanged)
            {
                if (lastChanged == false)
                {
                    sb.AppendLine("===========================");
                    var linePosition = line.Position ?? lastLine;
                    sb.AppendLine($"From line {linePosition}:");
                }

                lastChanged = true;
                sb.Append(GetLinePrefix(line));
                sb.Append(PresentWhitespaces(line.Text));
            }
            else
            {
                lastChanged = false;
            }

            if (line.Type != ChangeType.Inserted)
            {
                lastLine++;
            }
        }

        return sb.ToString();
    }

    private static readonly string CrLfVisualization = $"\u240D\u240A{Environment.NewLine}";
    private static readonly string LfVisualization = $"\u240A{Environment.NewLine}";
    private static readonly string CrVisualization = $"\u240D{Environment.NewLine}";
    private static readonly char SpaceVisualization = '\u00B7';
    private static readonly char TabVisualization = '\u2192';

    private static string PresentWhitespaces(string lineText)
    {
        var middleText = lineText.Replace(' ', SpaceVisualization)
            .Replace('\t', TabVisualization);

        if (middleText.EndsWith("\r\n"))
        {
            return middleText.Replace("\r\n", CrLfVisualization);
        }

        if (middleText.EndsWith("\n"))
        {
            return middleText.Replace("\n", LfVisualization);
        }

        if (middleText.EndsWith("\r"))
        {
            return middleText.Replace("\r", CrVisualization);
        }

        return middleText;
    }

    private static string GetLinePrefix(DiffPiece line)
    {
        switch (line.Type)
        {
            case ChangeType.Inserted:
                return "+ ";
            case ChangeType.Deleted:
                return "- ";
            case ChangeType.Modified:
                return "M ";
            case ChangeType.Imaginary:
                return "I ";
            default:
                return "  ";
        }
    }

    public static void TryToReportDiffWithExternalTool(string expectedCode, string text)
    {
        if (Debugger.IsAttached)
        {
            var tmpDir = Path.GetTempPath();
            var tempFileName = Guid.NewGuid().ToString("N").Substring(0, 4);
            var transformedPath = Path.Combine(tmpDir, $"{tempFileName}_transformed.cs");
            File.WriteAllText(transformedPath, text);
            var expectedPath = Path.Combine(tmpDir, $"{tempFileName}_expected.cs");
            File.WriteAllText(expectedPath, expectedCode);
            new DiffReporter().Report(transformedPath, expectedPath);
        }
    }
}
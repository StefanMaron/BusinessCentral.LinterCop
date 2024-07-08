using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;

namespace BusinessCentral.LinterCop.Test.RoslynTestKit;

public sealed class AnalyzerAdditionalFile : AdditionalText
{
    private readonly string path;

    public AnalyzerAdditionalFile(string path)
    {
        this.path = path;
    }

    public override string Path => path;

    public override SourceText GetText(CancellationToken cancellationToken)
    {
        return SourceText.From(File.ReadAllText(path));
    }
}
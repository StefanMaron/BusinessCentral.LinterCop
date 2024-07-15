using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions;

namespace BusinessCentral.LinterCop.Test.RoslynTestKit.CodeActionLocators;

public interface ICodeActionSelector
{
    public CodeAction Find(IReadOnlyList<CodeAction> actions);
}
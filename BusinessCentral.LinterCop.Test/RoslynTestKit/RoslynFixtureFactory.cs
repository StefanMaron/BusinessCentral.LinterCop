using Microsoft.Dynamics.Nav.CodeAnalysis.CodeFixes;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeRefactoring;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces.Completion;

namespace BusinessCentral.LinterCop.Test.RoslynTestKit;

public static class RoslynFixtureFactory
{
    public static AnalyzerTestFixture Create<T>(AnalyzerTestFixtureConfig? config = null)
        where T : DiagnosticAnalyzer, new()
    {
        var diagnosticAnalyzer = new T();
        return Create(diagnosticAnalyzer, config);
    }

    public static AnalyzerTestFixture Create(DiagnosticAnalyzer diagnosticAnalyzer,
        AnalyzerTestFixtureConfig? config = null)
    {
        return new ConfigurableAnalyzerTestFixture(diagnosticAnalyzer, config ?? new AnalyzerTestFixtureConfig());
    }

    public static CodeFixTestFixture Create<T>(CodeFixTestFixtureConfig? config = null) where T : CodeFixProvider, new()
    {
        var codeFixProvider = new T();
        return Create(codeFixProvider, config);
    }

    public static CodeFixTestFixture Create(CodeFixProvider codeFixProvider, CodeFixTestFixtureConfig? config = null)
    {
        return new ConfigurableCodeFixTestFixture(codeFixProvider, config ?? new CodeFixTestFixtureConfig());
    }

    public static CodeRefactoringTestFixture Create<T>(CodeRefactoringTestFixtureConfig? config = null)
        where T : CodeRefactoringProvider, new()
    {
        var codeRefactoringProvider = new T();
        return Create(codeRefactoringProvider, config);
    }

    public static CodeRefactoringTestFixture Create(CodeRefactoringProvider codeRefactoringProvider,
        CodeRefactoringTestFixtureConfig? config = null)
    {
        return new ConfigurableCodeRefactoringTestFixture(codeRefactoringProvider,
            config ?? new CodeRefactoringTestFixtureConfig());
    }

    public static CompletionProviderFixture Create<T>(CompletionProviderTestFixtureConfig? config = null)
        where T : CompletionProvider, new()
    {
        var codeRefactoringProvider = new T();
        return Create(codeRefactoringProvider, config);
    }

    private static ConfigurableCompletionProviderTestFixture Create(CompletionProvider completionProvider,
        CompletionProviderTestFixtureConfig? config = null)
    {
        return new ConfigurableCompletionProviderTestFixture(completionProvider,
            config ?? new CompletionProviderTestFixtureConfig());
    }
}
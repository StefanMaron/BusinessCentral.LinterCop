﻿using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;
using BusinessCentral.LinterCop.AnalysisContextExtension;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0077MissingParenthesis : DiagnosticAnalyzer
{
    private static readonly ImmutableHashSet<string> MethodsRequiringParenthesis = ImmutableHashSet.Create(
        "Count",
        "IsEmpty",
        "Today",
        "WorkDate",
        "GuiAllowed"
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0077MissingParenthesis);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterOperationAction(AnalyzeParenthesis, OperationKind.InvocationExpression);

    private void AnalyzeParenthesis(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved())
            return;

        if (ctx.Operation is not IInvocationExpression { Arguments.Length: 0 } operation ||
            operation.TargetMethod is not IMethodSymbol { MethodKind: MethodKind.BuiltInMethod } method)
            return;

        if (!operation.Syntax.GetLastToken().IsKind(SyntaxKind.CloseParenToken) &&
            MethodsRequiringParenthesis.Contains(method.Name))
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0077MissingParenthesis,
                ctx.Operation.Syntax.GetIdentifierNameSyntax().GetLocation(),
                method.Name));
        }
    }

    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor Rule0077MissingParenthesis = new(
            id: LinterCopAnalyzers.AnalyzerPrefix + "0077",
            title: LinterCopAnalyzers.GetLocalizableString("Rule0077MissingParenthesisTitle"),
            messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0077MissingParenthesisFormat"),
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: LinterCopAnalyzers.GetLocalizableString("Rule0077MissingParenthesisDescription"),
            helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0077");
    }
}

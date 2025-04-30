using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0003DoNotUseObjectIDsInVariablesOrProperties : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(
            DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties,
            DiagnosticDescriptors.Rule0005CasingMismatch);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.CheckForObjectIDsInVariablesOrProperties), new SyntaxKind[] {
                SyntaxKind.ObjectReference,
                SyntaxKind.PermissionValue
        });

    private void CheckForObjectIDsInVariablesOrProperties(SyntaxNodeAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved())
            return;

        string correctName = string.Empty;
        if (ctx.ContainingSymbol.Kind == SymbolKind.LocalVariable || ctx.ContainingSymbol.Kind == SymbolKind.GlobalVariable)
        {
            IVariableSymbol variable = (IVariableSymbol)ctx.ContainingSymbol;
            if (variable.Type.IsErrorType() || variable.Type.GetNavTypeKindSafe() == NavTypeKind.DotNet)
                return;

            switch (variable.Type.GetNavTypeKindSafe())
            {
                case NavTypeKind.Array:
                    correctName = ((IArrayTypeSymbol)variable.Type).ElementType.Name.ToString();
                    break;

                case NavTypeKind.List:
                    //TODO: Find a way to access variable.Type.TypeArguments[0].OriginalDefinition
                    return;

                default:
                    correctName = variable.Type.Name;
                    break;
            }

            if (ctx.Node.GetLastToken().ToString().Trim('"').ToUpper() != correctName.ToUpper())
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties, ctx.Node.GetLocation(), new object[] { ctx.Node.ToString().Trim('"'), correctName }));

            if (ctx.Node.GetLastToken().ToString().Trim('"') != correctName)
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005CasingMismatch, ctx.Node.GetLocation(), new object[] { correctName, "" }));
        }
        if (ctx.ContainingSymbol.Kind == SymbolKind.Property)
        {
            IPropertySymbol property = (IPropertySymbol)ctx.ContainingSymbol;
            if (ctx.Node.Kind == SyntaxKind.PermissionValue)
            {
                var nodes = ctx.Node.ChildNodesAndTokens().GetEnumerator();

                while (nodes.MoveNext())
                {
                    if (nodes.Current.IsNode)
                    {
                        var subnodes = nodes.Current.ChildNodesAndTokens().GetEnumerator();

                        while (subnodes.MoveNext())
                        {
                            if (subnodes.Current.Kind == SyntaxKind.ObjectId)
                            {
                                Location? location = nodes.Current.GetLocation();
                                if (location is not null)
                                    ctx.ReportDiagnostic(Diagnostic.Create(
                                        DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties,
                                        location,
                                        new object[] { "", "the object name" }));

                            }
                        }
                        ;
                    }
                }
                ;
            }

            if (property.PropertyKind != PropertyKind.Permissions && property.PropertyKind != PropertyKind.AccessByPermission)
            {
                if (ctx.Node.GetLastToken().ToString().Trim('"').ToUpper() != property.ValueText.ToUpper())
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties, ctx.Node.GetLocation(), new object[] { ctx.Node.ToString().Trim('"'), property.ValueText }));

                if (ctx.Node.GetLastToken().ToString().Trim('"') != property.ValueText)
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005CasingMismatch, ctx.Node.GetLocation(), new object[] { property.ValueText, "" }));
            }
        }

        if (ctx.ContainingSymbol.Kind == SymbolKind.Method)
        {
            IMethodSymbol method = (IMethodSymbol)ctx.ContainingSymbol;

            foreach (IParameterSymbol parameter in method.Parameters)
            {
                if (parameter.ParameterType.GetNavTypeKindSafe() == NavTypeKind.DotNet) continue;

                if (ctx.Node.GetLocation().SourceSpan.End == parameter.DeclaringSyntaxReference?.GetSyntax(ctx.CancellationToken).Span.End)
                {
                    if (parameter.ParameterType.GetNavTypeKindSafe() == NavTypeKind.Array)
                        correctName = ((IArrayTypeSymbol)parameter.ParameterType).ElementType.Name.ToString();
                    else
                        correctName = parameter.ParameterType.Name;

                    if (string.IsNullOrEmpty(correctName))
                        continue;

                    if (ctx.Node.GetLastToken().ToString().Trim('"').ToUpper() != correctName.ToUpper())
                        ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties, ctx.Node.GetLocation(), new object[] { ctx.Node.ToString().Trim('"'), correctName }));

                    if (ctx.Node.GetLastToken().ToString().Trim('"') != correctName)
                        ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005CasingMismatch, ctx.Node.GetLocation(), new object[] { correctName, "" }));
                }
            }
            IReturnValueSymbol returnValue = method.ReturnValueSymbol;
            if (returnValue?.DeclaringSyntaxReference is null || returnValue.ReturnType.GetNavTypeKindSafe() == NavTypeKind.DotNet) return;

            if (ctx.Node.GetLocation().SourceSpan.End == returnValue.DeclaringSyntaxReference.GetSyntax(ctx.CancellationToken).Span.End)
            {
                correctName = returnValue.ReturnType.Name;

                if (ctx.Node.GetLastToken().ToString().Trim('"').ToUpper() != correctName.ToUpper())
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties, ctx.Node.GetLocation(), new object[] { ctx.Node.ToString().Trim('"'), correctName }));

                if (ctx.Node.GetLastToken().ToString().Trim('"') != correctName)
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005CasingMismatch, ctx.Node.GetLocation(), new object[] { correctName, "" }));
            }
        }
    }
}
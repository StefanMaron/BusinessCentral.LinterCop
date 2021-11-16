using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0005VariableCasingShouldNotDIfferFromDeclaration : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDIfferFromDeclaration);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckForBuiltInMethodsWithCasingMismatch), new OperationKind[] {
                OperationKind.InvocationExpression,
                OperationKind.FieldAccess,
                OperationKind.GlobalReferenceExpression,
                OperationKind.LocalReferenceExpression,
                OperationKind.ParameterReferenceExpression,
                OperationKind.ReturnValueReferenceExpression
            });

            context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForBuiltInTypeCasingMismatch), new SymbolKind[] {
                SymbolKind.Codeunit,
                SymbolKind.Entitlement,
                SymbolKind.Enum,
                SymbolKind.EnumExtension,
                SymbolKind.Interface,
                SymbolKind.Page,
                SymbolKind.PageCustomization,
                SymbolKind.PageExtension,
                SymbolKind.Permission,
                SymbolKind.PermissionSet,
                SymbolKind.PermissionSetExtension,
                SymbolKind.Profile,
                SymbolKind.ProfileExtension,
                SymbolKind.Query,
                SymbolKind.Report,
                SymbolKind.ReportExtension,
                SymbolKind.Table,
                SymbolKind.TableExtension,
                SymbolKind.XmlPort
            });
        }
        private void CheckForBuiltInTypeCasingMismatch(SymbolAnalysisContext ctx)
        {
            foreach (var node in ctx.Symbol.DeclaringSyntaxReference.GetSyntax().DescendantNodesAndTokens().Where(n => IsValidToken(n)))
            {
                if (node.IsToken)
                    if (SyntaxFactory.Token(node.Kind).ToString() != node.ToString())
                        ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDIfferFromDeclaration, node.GetLocation(), new object[] { SyntaxFactory.Token(node.Kind), "" }));
                if (node.IsNode)
                {
                    if ((node.AsNode().IsKind(SyntaxKind.SimpleTypeReference) || node.Kind.ToString().Contains("DataType")) && !node.Kind.ToString().StartsWith("Codeunit") && !node.Kind.ToString().StartsWith("Enum") && !node.Kind.ToString().StartsWith("Label"))
                    {
                        var targetName = Enum.GetValues(typeof(NavTypeKind)).Cast<NavTypeKind>().FirstOrDefault(Kind => Kind.ToString().ToUpper() == node.AsNode().ToString().ToUpper() && Kind.ToString() != node.AsNode().ToString());
                        if (targetName != NavTypeKind.None)
                            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDIfferFromDeclaration, node.GetLocation(), new object[] { targetName, "" }));
                    }
                    if (node.AsNode().IsKind(SyntaxKind.SubtypedDataType) || node.AsNode().IsKind(SyntaxKind.GenericDataType) || node.AsNode().IsKind(SyntaxKind.OptionAccessExpression) ||
                       (node.AsNode().IsKind(SyntaxKind.SimpleTypeReference) && (node.Kind.ToString().StartsWith("Codeunit") || !node.Kind.ToString().StartsWith("Enum") || !node.Kind.ToString().StartsWith("Label"))))
                    {
                        var targetName = Enum.GetValues(typeof(NavTypeKind)).Cast<NavTypeKind>().FirstOrDefault(Kind => node.AsNode().ToString().ToUpper().StartsWith(Kind.ToString().ToUpper()) && !node.AsNode().ToString().StartsWith(Kind.ToString()));
                        if (targetName != NavTypeKind.None)
                            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDIfferFromDeclaration, node.GetLocation(), new object[] { targetName, "" }));
                    }
                }
            }
        }

        private static bool IsValidToken(Microsoft.Dynamics.Nav.CodeAnalysis.Syntax.SyntaxNodeOrToken n)
        {
            if (n.Kind.ToString().Contains("Keyword") &&
                !n.Kind.ToString().StartsWith("Codeunit") &&
                !n.Kind.ToString().StartsWith("Enum") &&
                !n.Kind.ToString().StartsWith("Label") &&
                !n.Kind.ToString().StartsWith("Action") &&
                !n.Kind.ToString().StartsWith("Page") &&
                !n.Kind.ToString().StartsWith("Interface") &&
                !n.Kind.ToString().StartsWith("Report") &&
                !n.Kind.ToString().StartsWith("Query") &&
                !n.Kind.ToString().StartsWith("XmlPort") &&
                !n.Kind.ToString().StartsWith("DotNet")
            )
                return true;
            if (n.Kind.ToString().Contains("DataType"))
                return true;
            if (n.Kind == SyntaxKind.SimpleTypeReference)
                return true;
            if (n.Kind == SyntaxKind.SubtypedDataType)
                return true;
            if (n.Kind == SyntaxKind.GenericDataType)
                return true;
            if (n.Kind == SyntaxKind.OptionAccessExpression)
                return true;

            return false;
        }

        private void CheckForBuiltInMethodsWithCasingMismatch(OperationAnalysisContext ctx)
        {
            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
            if (ctx.ContainingSymbol.IsObsoletePending || ctx.ContainingSymbol.IsObsoleteRemoved) return;

            var targetName = "";
            if (ctx.Operation.Kind == OperationKind.InvocationExpression)
            {
                IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
                targetName = operation.TargetMethod.Name;
            }
            if (ctx.Operation.Kind == OperationKind.FieldAccess)
            {
                try
                {
                    IFieldAccess operation = (IFieldAccess)ctx.Operation;
                    targetName = operation.FieldSymbol.Name;
                }
                catch (System.InvalidCastException)
                {
                }
            }
            if (new object[] {
                OperationKind.GlobalReferenceExpression,
                OperationKind.LocalReferenceExpression,
                OperationKind.ParameterReferenceExpression,
                OperationKind.ReturnValueReferenceExpression }.Contains(ctx.Operation.Kind))
            {
                switch (ctx.Operation.Kind)
                {
                    case OperationKind.GlobalReferenceExpression:
                        targetName = ((IGlobalReferenceExpression)ctx.Operation).GlobalVariable.Name;
                        break;
                    case OperationKind.LocalReferenceExpression:
                        targetName = ((ILocalReferenceExpression)ctx.Operation).LocalVariable.Name;
                        break;
                    case OperationKind.ParameterReferenceExpression:
                        targetName = ((IParameterReferenceExpression)ctx.Operation).Parameter.Name;
                        break;
                    case OperationKind.ReturnValueReferenceExpression:
                        targetName = ((IReturnValueReferenceExpression)ctx.Operation).ReturnValue.Name;
                        break;
                }
            }

            if (OnlyDiffersInCasing(ctx.Operation.Syntax.ToString(), targetName))
            {
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDIfferFromDeclaration, ctx.Operation.Syntax.GetLocation(), new object[] { targetName, "" }));
                return;
            }

            var nodes = Array.Find(ctx.Operation.Syntax.DescendantNodes((SyntaxNode e) => true).ToArray(), element => OnlyDiffersInCasing(element.ToString(), targetName));
            if (nodes != null)
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDIfferFromDeclaration, ctx.Operation.Syntax.GetLocation(), new object[] { targetName, "" }));
        }
        private bool OnlyDiffersInCasing(string left, string right)
        {
            return left.Trim('"').ToUpper() == right.Trim('"').ToUpper() && left.Trim('"') != right.Trim('"');
        }
    }
}

using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0005VariableCasingShouldNotDifferFromDeclaration : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
            = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration);

        private static readonly HashSet<SyntaxKind> _dataTypeSyntaxKinds = Enum.GetValues(typeof(SyntaxKind)).Cast<SyntaxKind>().Where(x => x.ToString().AsSpan().EndsWith("DataType")).ToHashSet();
        private static readonly string[] _symbolKinds = Enum.GetValues(typeof(SymbolKind)).Cast<SymbolKind>().Select(x => x.ToString()).ToArray();
        private static string[] _navTypeKindStrings;
        private static readonly string[] _propertyKindStrings = Enum.GetValues(typeof(PropertyKind)).Cast<PropertyKind>().Select(x => x.ToString()).ToArray();
        private static readonly string[] _labelPropertyString = LabelPropertyHelper.GetAllLabelProperties();

        public override void Initialize(AnalysisContext context)
        {
            GenerateNavTypeKindArray();

            context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckForBuiltInMethodsWithCasingMismatch), new OperationKind[] {
                OperationKind.InvocationExpression,
                OperationKind.FieldAccess,
                OperationKind.GlobalReferenceExpression,
                OperationKind.LocalReferenceExpression,
                OperationKind.ParameterReferenceExpression,
                OperationKind.ReturnValueReferenceExpression,
                OperationKind.XmlPortDataItemAccess
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

        private static void GenerateNavTypeKindArray()
        {
            var navTypeKinds = Enum.GetValues(typeof(NavTypeKind)).Cast<NavTypeKind>().Select(s => s.ToString()).ToList();
            navTypeKinds.Add("Database"); // for Database::"G/L Entry" (there is no NavTypeKind for this)
            _navTypeKindStrings = navTypeKinds.ToArray();
        }

        private void CheckForBuiltInTypeCasingMismatch(SymbolAnalysisContext ctx)
        {
            AnalyseTokens(ctx);
            AnalyseNodes(ctx);
            AnalyzeMemberAccessExpressions(ctx);
            AnalyzePropertyNames(ctx);
            AnalyzeLabelProperties(ctx);
        }

        private void AnalyseTokens(SymbolAnalysisContext ctx)
        {
            IEnumerable<SyntaxToken> descendantTokens = ctx.Symbol.DeclaringSyntaxReference.GetSyntax().DescendantTokens()
                                            .Where(t => t.Kind.IsKeyword())
                                            .Where(t => !_dataTypeSyntaxKinds.Contains(t.Parent.Kind));

            foreach (SyntaxToken descendantToken in descendantTokens)
            {
                ctx.CancellationToken.ThrowIfCancellationRequested();

                SyntaxToken syntaxToken = SyntaxFactory.Token(descendantToken.Kind);
                if (syntaxToken.Kind == SyntaxKind.None)
                    continue;

                if (!syntaxToken.ToString().AsSpan().Equals(descendantToken.ToString().AsSpan(), StringComparison.Ordinal))
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration, descendantToken.GetLocation(), new object[] { syntaxToken, "" }));
            }
        }

        private void AnalyseNodes(SymbolAnalysisContext ctx)
        {
            IEnumerable<SyntaxNode> descendantNodes = ctx.Symbol.DeclaringSyntaxReference.GetSyntax().DescendantNodes()
                                            .Where(n => !n.ToString().AsSpan().StartsWith("array"));

            foreach (SyntaxNode descendantNode in descendantNodes)
            {
                ctx.CancellationToken.ThrowIfCancellationRequested();

                var syntaxNodeKindSpan = descendantNode.Kind.ToString().AsSpan();
                var syntaxNodeSpan = descendantNode.ToString();

                if ((descendantNode.IsKind(SyntaxKind.SimpleTypeReference) ||
                     syntaxNodeKindSpan.Contains("DataType", StringComparison.Ordinal)) &&
                    !syntaxNodeKindSpan.StartsWith("Codeunit") &&
                    !syntaxNodeKindSpan.StartsWith("Enum") &&
                    !syntaxNodeKindSpan.StartsWith("Label"))
                {
                    var targetName = _navTypeKindStrings.FirstOrDefault(Kind =>
                    {
                        var kindSpan = Kind.AsSpan();
                        return kindSpan.Equals(syntaxNodeSpan.AsSpan(), StringComparison.OrdinalIgnoreCase) &&
                               !kindSpan.Equals(syntaxNodeSpan.AsSpan(), StringComparison.Ordinal);
                    });

                    if (targetName != null)
                    {
                        ctx.ReportDiagnostic(Diagnostic.Create(
                            DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration,
                            descendantNode.GetLocation(), new object[] { targetName, "" }));
                        continue;
                    }
                }

                if (IsValidKind(descendantNode.Kind))
                {
                    if (syntaxNodeKindSpan.StartsWith("Codeunit") ||
                        !syntaxNodeKindSpan.StartsWith("Enum") ||
                        !syntaxNodeKindSpan.StartsWith("Label"))
                    {
                        var targetName = _navTypeKindStrings.FirstOrDefault(Kind =>
                        {
                            var kindSpan = Kind.AsSpan();
                            var readOnlySpan = syntaxNodeSpan.AsSpan();
                            return readOnlySpan.StartsWith(kindSpan, StringComparison.OrdinalIgnoreCase) &&
                                   !readOnlySpan.StartsWith(kindSpan, StringComparison.Ordinal);
                        });
                        if (targetName != null)
                        {
                            var firstToken = descendantNode.GetFirstToken();
                            ctx.ReportDiagnostic(Diagnostic.Create(
                                DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration,
                                firstToken.GetLocation(), new object[] { targetName, "" }));
                        }
                    }
                }
            }
        }

        private void AnalyzeMemberAccessExpressions(SymbolAnalysisContext ctx)
        {
            IEnumerable<SyntaxToken> descendantTokens = ctx.Symbol.DeclaringSyntaxReference.GetSyntax().DescendantTokens()
                                            .Where(t => t.Parent.Parent.Kind == SyntaxKind.MemberAccessExpression);

            foreach (SyntaxToken token in descendantTokens)
            {
                ctx.CancellationToken.ThrowIfCancellationRequested();

                int result = Array.FindIndex(_symbolKinds, t => t.Equals(token.ValueText, StringComparison.OrdinalIgnoreCase));
                if (result == -1)
                    continue;

                if (!token.ValueText.AsSpan().Equals(_symbolKinds[result].ToString().AsSpan(), StringComparison.Ordinal))
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration, token.GetLocation(), new object[] { _symbolKinds[result].ToString(), "" }));
            }
        }

        private void AnalyzePropertyNames(SymbolAnalysisContext ctx)
        {
            IEnumerable<SyntaxToken> descendantTokens = ctx.Symbol.DeclaringSyntaxReference.GetSyntax().DescendantTokens()
                                            .Where(t => t.Parent.Kind == SyntaxKind.PropertyName);

            foreach (SyntaxToken token in descendantTokens)
            {
                ctx.CancellationToken.ThrowIfCancellationRequested();

                int result = Array.FindIndex(_propertyKindStrings, t => t.Equals(token.ValueText, StringComparison.OrdinalIgnoreCase));
                if (result == -1)
                    continue;

                if (!token.ValueText.AsSpan().Equals(_propertyKindStrings[result].ToString().AsSpan(), StringComparison.Ordinal))
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration, token.GetLocation(), new object[] { _propertyKindStrings[result].ToString(), "" }));
            }
        }

        private static void AnalyzeLabelProperties(SymbolAnalysisContext ctx)
        {
            IEnumerable<SyntaxToken> descendantTokens = ctx.Symbol.DeclaringSyntaxReference.GetSyntax().DescendantTokens()
                                            .Where(n => n.Kind == SyntaxKind.IdentifierToken)
                                            .Where(n => n.Parent.Parent.Parent.Kind == SyntaxKind.Label);

            foreach (SyntaxToken token in descendantTokens)
            {
                ctx.CancellationToken.ThrowIfCancellationRequested();

                int result = Array.FindIndex(_labelPropertyString, t => t.Equals(token.ToString(), StringComparison.OrdinalIgnoreCase));
                if (result == -1)
                    continue;

                if (!token.ToString().AsSpan().Equals(_labelPropertyString[result].ToString().AsSpan(), StringComparison.Ordinal))
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration, token.GetLocation(), new object[] { _labelPropertyString[result].ToString(), "" }));
            }
        }

        private static bool IsValidKind(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.SubtypedDataType:
                case SyntaxKind.GenericDataType:
                case SyntaxKind.OptionAccessExpression:
                case SyntaxKind.SimpleTypeReference:
                    return true;
            }

            return false;
        }

        private void CheckForBuiltInMethodsWithCasingMismatch(OperationAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            var targetName = "";

            switch (ctx.Operation.Kind)
            {
                case OperationKind.InvocationExpression:
                    if (ctx.Operation is IInvocationExpression invocationExpression)
                        targetName = invocationExpression.TargetMethod.Name;
                    break;
                case OperationKind.FieldAccess:
                    if (ctx.Operation is IFieldAccess fieldAccess)
                        targetName = fieldAccess.FieldSymbol.Name;
                    break;
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
                case OperationKind.XmlPortDataItemAccess:
                    targetName = ((IXmlPortNodeAccess)ctx.Operation).XmlPortNodeSymbol.Name;
                    break;
                default:
                    return;
            }

            if (OnlyDiffersInCasing(ctx.Operation.Syntax.ToString().AsSpan(), targetName.AsSpan()))
            {
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration, ctx.Operation.Syntax.GetLocation(), new object[] { targetName, "" }));
                return;
            }

            var nodes = Array.Find(ctx.Operation.Syntax.DescendantNodes((SyntaxNode e) => true).ToArray(), element => OnlyDiffersInCasing(element.ToString().AsSpan(), targetName.AsSpan()));
            if (nodes != null)
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration, ctx.Operation.Syntax.GetLocation(), new object[] { targetName, "" }));
        }

        private bool OnlyDiffersInCasing(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
        {
            var leftSpan = left.Trim('"');
            var rightSpan = right.Trim('"');
            return leftSpan.Equals(rightSpan, StringComparison.OrdinalIgnoreCase) &&
                   !leftSpan.Equals(rightSpan, StringComparison.Ordinal);
        }
    }
}
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0005VariableCasingShouldNotDifferFromDeclaration : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } 
            = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration);
        
        private static readonly HashSet<SyntaxKind> _validTokens = new();
        private static string[] _navTypeKindStrings; 

        public override void Initialize(AnalysisContext context)
        {
            GenerateNavTypeKindArray();
            GenerateValidTokenArray();
           
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

        private static void GenerateNavTypeKindArray()
        {
            var navTypeKinds = Enum.GetValues(typeof(NavTypeKind)).Cast<NavTypeKind>().Select(s => s.ToString()).ToList();
            navTypeKinds.Add("Database"); // for Database::"G/L Entry" (there is no NavTypeKind for this)
            _navTypeKindStrings = navTypeKinds.ToArray();
        }

        private static void GenerateValidTokenArray()
        {
            _validTokens.Clear();
            var allKinds = Enum.GetValues(typeof(SyntaxKind)).Cast<SyntaxKind>();
            foreach (var kind in allKinds)
            {
                var kindSpan = kind.ToString().AsSpan();
              
                if ((kindSpan.Contains("Keyword", StringComparison.Ordinal) &&
                    !kindSpan.StartsWith("Codeunit") &&
                    !kindSpan.StartsWith("Enum") &&
                    !kindSpan.StartsWith("Label") &&
                    !kindSpan.StartsWith("Action") &&
                    !kindSpan.StartsWith("Page") &&
                    !kindSpan.StartsWith("Interface") &&
                    !kindSpan.StartsWith("Report") &&
                    !kindSpan.StartsWith("Query") &&
                    !kindSpan.StartsWith("XmlPort") &&
                    !kindSpan.StartsWith("DotNet")) ||
                    kindSpan.Contains("DataType", StringComparison.Ordinal)
                   )
                {
                    _validTokens.Add(kind);
                    continue;
                }
                
                switch (kind)
                {
                    case SyntaxKind.SimpleTypeReference:
                    case SyntaxKind.OptionAccessExpression:
                        _validTokens.Add(kind);
                        continue;
                }
                
            }
        }

        private void CheckForBuiltInTypeCasingMismatch(SymbolAnalysisContext ctx)
        {
            foreach (var node in ctx.Symbol.DeclaringSyntaxReference.GetSyntax().DescendantNodesAndTokens().Where(n => _validTokens.Contains(n.Kind)))
            {
                ctx.CancellationToken.ThrowIfCancellationRequested();

                var syntaxNodeKindSpan = node.Kind.ToString().AsSpan();

                if (node.IsToken)
                {
                    var syntaxToken = SyntaxFactory.Token(node.Kind);
                    if (!syntaxToken.ToString().AsSpan().Equals(node.ToString().AsSpan(), StringComparison.Ordinal))
                    {
                        ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration, node.GetLocation(), new object[] { syntaxToken, "" }));
                        continue;
                    }
                }

                var syntaxNode = node.AsNode();
                if (syntaxNode == null)
                    continue;

                if (!node.IsNode)
                    continue;
                
                var syntaxNodeAsString = syntaxNode.ToString();
                if (!syntaxNodeAsString.StartsWith("array"))
                {
                    if ((syntaxNode.IsKind(SyntaxKind.SimpleTypeReference) ||
                         syntaxNodeKindSpan.Contains("DataType", StringComparison.Ordinal)) &&
                        !syntaxNodeKindSpan.StartsWith("Codeunit") &&
                        !syntaxNodeKindSpan.StartsWith("Enum") &&
                        !syntaxNodeKindSpan.StartsWith("Label"))
                    {
                        var targetName = _navTypeKindStrings.FirstOrDefault(Kind =>
                        {
                            var kindSpan = Kind.AsSpan();
                            return kindSpan.Equals(syntaxNodeAsString.AsSpan(), StringComparison.OrdinalIgnoreCase) &&
                                   !kindSpan.Equals(syntaxNodeAsString.AsSpan(), StringComparison.Ordinal);
                        });

                        if (targetName != null)
                        {
                            ctx.ReportDiagnostic(Diagnostic.Create(
                                DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration,
                                node.GetLocation(), new object[] { targetName, "" }));
                            continue;
                        }
                    }

                    if (IsValidKind(syntaxNode.Kind))
                    {
                        if (syntaxNodeKindSpan.StartsWith("Codeunit") ||
                            !syntaxNodeKindSpan.StartsWith("Enum") ||
                            !syntaxNodeKindSpan.StartsWith("Label"))
                        {
                            var targetName = _navTypeKindStrings.FirstOrDefault(Kind =>
                            {
                                var kindSpan = Kind.AsSpan();
                                var readOnlySpan = syntaxNodeAsString.AsSpan();
                                return readOnlySpan.StartsWith(kindSpan, StringComparison.OrdinalIgnoreCase) &&
                                       !readOnlySpan.StartsWith(kindSpan, StringComparison.Ordinal);
                            });
                            if (targetName != null)
                            {
                                var firstToken = syntaxNode.GetFirstToken();
                                ctx.ReportDiagnostic(Diagnostic.Create(
                                    DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration,
                                    firstToken.GetLocation(), new object[] { targetName, "" }));
                                
                            }
                        }
                    }
                }
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
            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || 
                ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) 
                return;
            
            if (ctx.ContainingSymbol.IsObsoletePending || 
                ctx.ContainingSymbol.IsObsoleteRemoved) 
                return;

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

        private bool OnlyDiffersInCasing(ReadOnlySpan<char> left,ReadOnlySpan<char> right)
        {
            var leftSpan = left.Trim('"');
            var rightSpan = right.Trim('"');
            return leftSpan.Equals(rightSpan, StringComparison.OrdinalIgnoreCase) &&
                   !leftSpan.Equals(rightSpan, StringComparison.Ordinal);
       }
    }
}
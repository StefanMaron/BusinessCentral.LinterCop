using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0005VariableCasingShouldNotDifferFromDeclaration : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
            = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration);

        private static readonly HashSet<SyntaxKind> _dataTypeSyntaxKinds = Enum.GetValues(typeof(SyntaxKind)).Cast<SyntaxKind>().Where(x => x.ToString().AsSpan().EndsWith("DataType")).ToHashSet();
        private static readonly string[] _areaKinds = Enum.GetValues(typeof(AreaKind)).Cast<AreaKind>().Select(x => x.ToString()).ToArray();
        private static readonly string[] _actionAreaKinds = Enum.GetValues(typeof(ActionAreaKind)).Cast<ActionAreaKind>().Select(x => x.ToString()).ToArray();
        private static readonly string[] _labelPropertyString = LabelPropertyHelper.GetAllLabelProperties();
        private static readonly string[] _navTypeKindStrings = GenerateNavTypeKindArray();
        private static readonly string[] _propertyKindStrings = Enum.GetValues(typeof(PropertyKind)).Cast<PropertyKind>().Select(x => x.ToString()).ToArray();
        private static readonly string[] _symbolKinds = Enum.GetValues(typeof(SymbolKind)).Cast<SymbolKind>().Select(x => x.ToString()).ToArray();
        private static readonly Dictionary<TriggerTypeKind, string> _triggerTypeKinds = GenerateNTriggerTypeKindMappings();


        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeLabel), SyntaxKind.Label);
            context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzePropertyName), SyntaxKind.PropertyName);
            context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeMemberAccessExpression), SyntaxKind.MemberAccessExpression);
            context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeAreaSectionName), SyntaxKind.PageArea);
            context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeActionAreaSectionName), SyntaxKind.PageActionArea);
            context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeTriggerDeclaration), SyntaxKind.TriggerDeclaration);
            context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeIdentifierName), SyntaxKind.IdentifierName);
            context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeQualifiedName), SyntaxKind.QualifiedName);
            context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeQualifiedNameWithoutNamespace), SyntaxKind.QualifiedName);
            context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeLengthDataType), SyntaxKind.LengthDataType);

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

        private static string[] GenerateNavTypeKindArray()
        {
            var navTypeKinds = Enum.GetValues(typeof(NavTypeKind)).Cast<NavTypeKind>().Select(s => s.ToString()).ToList();
            navTypeKinds.Add("Database"); // for Database::"G/L Entry" (there is no NavTypeKind for this)
            return navTypeKinds.ToArray();
        }

        private static Dictionary<TriggerTypeKind, string> GenerateNTriggerTypeKindMappings()
        {
            var mappings = new Dictionary<TriggerTypeKind, string>();

            foreach (TriggerTypeKind type in Enum.GetValues(typeof(TriggerTypeKind)))
            {
                string typeName = type.ToString();
                int index = typeName.IndexOf("On");
                if (index > 0)
                {
                    mappings[type] = typeName.Substring(index); ;
                }
            }

            return mappings;
        }

        private void AnalyzeLabel(SyntaxNodeAnalysisContext ctx)
        {
            IEnumerable<SyntaxNode> nodes = ctx.Node.DescendantNodes()
                        .Where(n => n.Kind == SyntaxKind.IdentifierEqualsLiteral);

            foreach (SyntaxNode node in nodes)
            {
                ctx.CancellationToken.ThrowIfCancellationRequested();

                SyntaxToken syntaxToken = ((IdentifierEqualsLiteralSyntax)node).Identifier;
                int result = Array.FindIndex(_labelPropertyString, t => t.Equals(syntaxToken.ValueText, StringComparison.OrdinalIgnoreCase));
                if (result == -1)
                    continue;

                if (!syntaxToken.ValueText.AsSpan().Equals(_labelPropertyString[result].ToString().AsSpan(), StringComparison.Ordinal))
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration, syntaxToken.GetLocation(), new object[] { _labelPropertyString[result].ToString(), "" }));
            }
        }

        private void AnalyzePropertyName(SyntaxNodeAnalysisContext ctx)
        {
            SyntaxToken syntaxToken = ((PropertyNameSyntax)ctx.Node).Identifier;
            int result = Array.FindIndex(_propertyKindStrings, t => t.Equals(syntaxToken.ValueText, StringComparison.OrdinalIgnoreCase));
            if (result == -1)
                return;

            if (!syntaxToken.ValueText.AsSpan().Equals(_propertyKindStrings[result].ToString().AsSpan(), StringComparison.Ordinal))
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration, syntaxToken.GetLocation(), new object[] { _propertyKindStrings[result].ToString(), "" }));
        }

        private void AnalyzeMemberAccessExpression(SyntaxNodeAnalysisContext ctx)
        {
            SyntaxNode childNode = ctx.Node.ChildNodes().Where(n => n.Kind == SyntaxKind.IdentifierName).FirstOrDefault();
            if (childNode == null) return;

            SyntaxToken syntaxToken = ((IdentifierNameSyntax)childNode).Identifier;
            int result = Array.FindIndex(_symbolKinds, t => t.Equals(syntaxToken.ValueText, StringComparison.OrdinalIgnoreCase));
            if (result == -1)
                return;

            if (!syntaxToken.ValueText.AsSpan().Equals(_symbolKinds[result].ToString().AsSpan(), StringComparison.Ordinal))
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration, syntaxToken.GetLocation(), new object[] { _symbolKinds[result].ToString(), "" }));
        }

        private void AnalyzeAreaSectionName(SyntaxNodeAnalysisContext ctx)
        {
            IEnumerable<SyntaxNode> childNodes = ctx.Node.ChildNodes().Where(n => n.Kind == SyntaxKind.IdentifierName);

            foreach (SyntaxNode childNode in childNodes)
            {
                ctx.CancellationToken.ThrowIfCancellationRequested();

                SyntaxToken syntaxToken = ((IdentifierNameSyntax)childNode).Identifier;
                int result = Array.FindIndex(_areaKinds, t => t.Equals(syntaxToken.ValueText, StringComparison.OrdinalIgnoreCase));
                if (result == -1)
                    continue;

                if (!syntaxToken.ValueText.AsSpan().Equals(_areaKinds[result].ToString().AsSpan(), StringComparison.Ordinal))
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration, childNode.GetLocation(), new object[] { _areaKinds[result].ToString(), "" }));
            }
        }

        private void AnalyzeActionAreaSectionName(SyntaxNodeAnalysisContext ctx)
        {
            IEnumerable<SyntaxNode> childNodes = ctx.Node.ChildNodes().Where(n => n.Kind == SyntaxKind.IdentifierName);

            foreach (SyntaxNode childNode in childNodes)
            {
                ctx.CancellationToken.ThrowIfCancellationRequested();

                SyntaxToken syntaxToken = ((IdentifierNameSyntax)childNode).Identifier;
                int result = Array.FindIndex(_actionAreaKinds, t => t.Equals(syntaxToken.ValueText, StringComparison.OrdinalIgnoreCase));
                if (result == -1)
                    continue;

                if (!syntaxToken.ValueText.AsSpan().Equals(_actionAreaKinds[result].ToString().AsSpan(), StringComparison.Ordinal))
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration, childNode.GetLocation(), new object[] { _actionAreaKinds[result].ToString(), "" }));
            }
        }

        private void AnalyzeTriggerDeclaration(SyntaxNodeAnalysisContext ctx)
        {
            TriggerDeclarationSyntax syntax = ctx.Node as TriggerDeclarationSyntax;
            if (syntax == null)
                return;

            ISymbolWithTriggers symbolWithTriggers = ctx.ContainingSymbol.ContainingSymbol as ISymbolWithTriggers;

            TriggerTypeInfo triggerTypeInfo = symbolWithTriggers.GetTriggerTypeInfo(syntax.Name.Identifier.ValueText);
            if (triggerTypeInfo == null)
                return;

            if (!_triggerTypeKinds.TryGetValue(triggerTypeInfo.Kind, out string targetName))
                return;

            if (!syntax.Name.Identifier.ValueText.AsSpan().Equals(targetName.AsSpan(), StringComparison.Ordinal))
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration, syntax.Name.GetLocation(), new object[] { targetName, "" }));
        }

        private void AnalyzeIdentifierName(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.Node is not IdentifierNameSyntax node)
                return;

            if (node.Parent.Kind == SyntaxKind.PragmaWarningDirectiveTrivia)
                return;

            ISymbol fieldSymbol = ctx.SemanticModel.GetSymbolInfo(ctx.Node, ctx.CancellationToken).Symbol;
            if (fieldSymbol == null)
                return;

            // TODO: Support more SymbolKinds
            if (fieldSymbol.Kind != SymbolKind.Field)
                return;

            string identifierName = StringExtensions.UnquoteIdentifier(node.Identifier.ValueText);

            if (!identifierName.AsSpan().Equals(fieldSymbol.Name.AsSpan(), StringComparison.Ordinal))
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration, node.GetLocation(), new object[] { StringExtensions.QuoteIdentifierIfNeeded(fieldSymbol.Name), "" }));
        }

        private void AnalyzeQualifiedName(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.Node is not QualifiedNameSyntax node)
                return;

            ISymbol fieldSymbol = ctx.SemanticModel.GetSymbolInfo(ctx.Node, ctx.CancellationToken).Symbol;
            if (fieldSymbol == null)
                return;

            string identifierName = StringExtensions.UnquoteIdentifier(node.Right.Identifier.ValueText);

            if (!identifierName.AsSpan().Equals(fieldSymbol.Name.AsSpan(), StringComparison.Ordinal))
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration, node.Right.GetLocation(), new object[] { StringExtensions.QuoteIdentifierIfNeeded(fieldSymbol.Name), "" }));
        }

        private void AnalyzeQualifiedNameWithoutNamespace(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.Node is not QualifiedNameSyntax node)
                return;

            if (node.Left.Kind != SyntaxKind.IdentifierName)
                return;

            ISymbol fieldSymbol = ctx.SemanticModel.GetSymbolInfo(ctx.Node, ctx.CancellationToken).Symbol;
            if (fieldSymbol == null)
                return;

            if (fieldSymbol.ContainingSymbol is not IObjectTypeSymbol objectTypeSymbol)
                return;

            if (fieldSymbol.ContainingSymbol.Kind == SymbolKind.TableExtension)
            {
                ITableExtensionTypeSymbol tableExtension = (ITableExtensionTypeSymbol)fieldSymbol.ContainingSymbol;
                objectTypeSymbol = tableExtension.Target as IObjectTypeSymbol;
            }

            IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)node.Left;
            SyntaxToken identifier = identifierNameSyntax.Identifier;
            if (identifier == null)
                return;

            string identifierName = StringExtensions.UnquoteIdentifier(identifier.ValueText);

            if (!identifierName.AsSpan().Equals(objectTypeSymbol.Name.AsSpan(), StringComparison.Ordinal))
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration, identifierNameSyntax.GetLocation(), new object[] { StringExtensions.QuoteIdentifierIfNeeded(objectTypeSymbol.Name), "" }));
        }

        private void AnalyzeLengthDataType(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.Node is not LengthDataTypeSyntax node)
                return;

            SyntaxToken identifierToken = node.GetFirstToken();
            if (!IsNavTypeKindWithDifferentCasing(identifierToken.ValueText, out string targetName))
                return;

            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration, identifierToken.GetLocation(), new object[] { targetName, "" }));
        }

        private void CheckForBuiltInTypeCasingMismatch(SymbolAnalysisContext ctx)
        {
            AnalyseTokens(ctx);
            AnalyseNodes(ctx);
        }

        private void AnalyseTokens(SymbolAnalysisContext ctx)
        {
            IEnumerable<SyntaxToken> descendantTokens = ctx.Symbol.DeclaringSyntaxReference.GetSyntax().DescendantTokens()
                                            .Where(t => t.Kind.IsKeyword())
                                            .Where(t => !_dataTypeSyntaxKinds.Contains(t.Parent.Kind))
                                            .Where(t => !string.IsNullOrEmpty(t.ToString()));

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
                                            .Where(t => t.Kind != SyntaxKind.LengthDataType) // handeld on AnalyzeLengthDataType method
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
                    if (descendantNode is SimpleTypeReferenceSyntax simpleTypeRefSubstituteToken && simpleTypeRefSubstituteToken.DataType.Kind == SyntaxKind.LengthDataType)
                        continue; // handeld on AnalyzeLengthDataType method

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

        private static bool IsNavTypeKindWithDifferentCasing(string inputNavTypeKind, out string matchedNavTypeKind)
        {
            matchedNavTypeKind = _navTypeKindStrings.SingleOrDefault(Kind =>
                {
                    var kindSpan = Kind.AsSpan();
                    return kindSpan.Equals(inputNavTypeKind.AsSpan(), StringComparison.OrdinalIgnoreCase) &&
                            !kindSpan.Equals(inputNavTypeKind.AsSpan(), StringComparison.Ordinal);
                });

            return matchedNavTypeKind is not null;
        }
    }
}
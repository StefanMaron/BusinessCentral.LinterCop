using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0005VariableCasingShouldNotDifferFromDeclaration : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
        = ImmutableArray.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration);

    #region Comparators Dictionaries

    private static readonly HashSet<SyntaxKind> _dataTypeSyntaxKinds =
        Enum.GetValues(typeof(SyntaxKind))
            .Cast<SyntaxKind>()
            .Where(x => x.ToString().AsSpan().EndsWith("DataType"))
            .ToHashSet();

    private static readonly Lazy<ImmutableDictionary<string, string>> _accessibilityDictionary =
        new Lazy<ImmutableDictionary<string, string>>(() =>
            Enum.GetValues(typeof(Accessibility))
                .Cast<Accessibility>()
                .Select(x => x.ToString())
                .ToImmutableDictionary(s => s, s => s, StringComparer.OrdinalIgnoreCase));

    private static readonly Lazy<ImmutableDictionary<string, string>> _actionAreaKindDictionary =
        new Lazy<ImmutableDictionary<string, string>>(() =>
            Enum.GetValues(typeof(ActionAreaKind))
                .Cast<ActionAreaKind>()
                .Select(x => x.ToString())
                .ToImmutableDictionary(s => s, s => s, StringComparer.OrdinalIgnoreCase));

    private static readonly Lazy<ImmutableDictionary<string, string>> _allowInCustomizationsKindDictionary =
        new Lazy<ImmutableDictionary<string, string>>(() =>
            Enum.GetValues(typeof(AllowInCustomizationsKind))
                .Cast<AllowInCustomizationsKind>()
                .Select(x => x.ToString())
                .ToImmutableDictionary(s => s, s => s, StringComparer.OrdinalIgnoreCase));

    private static readonly Lazy<ImmutableDictionary<string, string>> _areaKindDictionary =
        new Lazy<ImmutableDictionary<string, string>>(() =>
            Enum.GetValues(typeof(AreaKind))
                .Cast<AreaKind>()
                .Select(x => x.ToString())
                .ToImmutableDictionary(s => s, s => s, StringComparer.OrdinalIgnoreCase));

    private static readonly Lazy<ImmutableDictionary<string, string>> _blankNumbersKindDictionary =
        new Lazy<ImmutableDictionary<string, string>>(() =>
            Enum.GetValues(typeof(BlankNumbersKind))
                .Cast<BlankNumbersKind>()
                .Select(x => x.ToString())
                .ToImmutableDictionary(s => s, s => s, StringComparer.OrdinalIgnoreCase));

    private static readonly Lazy<ImmutableDictionary<string, string>> _compressionTypeKindDictionary =
        new Lazy<ImmutableDictionary<string, string>>(() =>
            Enum.GetValues(typeof(CompressionTypeKind))
                .Cast<CompressionTypeKind>()
                .Select(x => x.ToString())
                .ToImmutableDictionary(s => s, s => s, StringComparer.OrdinalIgnoreCase));

    private static readonly Lazy<ImmutableDictionary<string, string>> _dataClassificationKindDictionary =
        new Lazy<ImmutableDictionary<string, string>>(() =>
            Enum.GetValues(typeof(DataClassificationKind))
                .Cast<DataClassificationKind>()
                .Select(x => x.ToString())
                .ToImmutableDictionary(s => s, s => s, StringComparer.OrdinalIgnoreCase));

    private static readonly Lazy<ImmutableDictionary<string, string>> _fieldClassKindDictionary =
        new Lazy<ImmutableDictionary<string, string>>(() =>
            Enum.GetValues(typeof(FieldClassKind))
                .Cast<FieldClassKind>()
                .Select(x => x.ToString())
                .ToImmutableDictionary(s => s, s => s, StringComparer.OrdinalIgnoreCase));

    private static readonly Lazy<ImmutableDictionary<string, string>> _labelPropertyDictionary =
        new Lazy<ImmutableDictionary<string, string>>(() =>
            LabelPropertyHelper.GetAllLabelProperties()
                            .ToImmutableDictionary(s => s, s => s, StringComparer.OrdinalIgnoreCase));

    private static readonly Lazy<ImmutableDictionary<string, string>> _navTypeKindDictionary =
        new Lazy<ImmutableDictionary<string, string>>(() =>
            GenerateNavTypeKindDictionary());

    private static readonly Lazy<ImmutableDictionary<string, string>> _obsoleteStateKindDictionary =
        new Lazy<ImmutableDictionary<string, string>>(() =>
            Enum.GetValues(typeof(TableObsoleteStateKind))
                .Cast<TableObsoleteStateKind>()
                .Select(x => x.ToString())
            .Concat(
                Enum.GetValues(typeof(FieldObsoleteStateKind))
                    .Cast<FieldObsoleteStateKind>()
                    .Select(x => x.ToString()))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToImmutableDictionary(s => s, s => s, StringComparer.OrdinalIgnoreCase));

    private static readonly Lazy<ImmutableDictionary<string, string>> _propertyKindDictionary =
        new Lazy<ImmutableDictionary<string, string>>(() =>
            Enum.GetValues(typeof(PropertyKind))
                .Cast<PropertyKind>()
                .Select(x => x.ToString())
                .ToImmutableDictionary(s => s, s => s, StringComparer.OrdinalIgnoreCase));

    private static readonly Lazy<ImmutableDictionary<string, string>> _scopeKindDictionary =
        new Lazy<ImmutableDictionary<string, string>>(() =>
            Enum.GetValues(typeof(TableScopeKind))
                .Cast<TableScopeKind>()
                .Select(x => x.ToString())
                .ToImmutableDictionary(s => s, s => s, StringComparer.OrdinalIgnoreCase));

    private static readonly Lazy<ImmutableDictionary<string, string>> _subtypeKindDictionary =
        new Lazy<ImmutableDictionary<string, string>>(() =>
            Enum.GetValues(typeof(FieldSubtypeKind))
                .Cast<FieldSubtypeKind>()
                .Select(x => x.ToString())
                .ToImmutableDictionary(s => s, s => s, StringComparer.OrdinalIgnoreCase));

    private static readonly Lazy<ImmutableDictionary<string, string>> _symbolKindDictionary =
        new Lazy<ImmutableDictionary<string, string>>(() =>
            GenerateSymbolKindDictionary());

    private static readonly Lazy<ImmutableDictionary<string, string>> _tableTypeKindDictionary =
        new Lazy<ImmutableDictionary<string, string>>(() =>
            Enum.GetValues(typeof(TableTypeKind))
                .Cast<TableTypeKind>()
                .Select(x => x.ToString())
                .ToImmutableDictionary(s => s, s => s, StringComparer.OrdinalIgnoreCase));

    private static readonly Lazy<ImmutableDictionary<TriggerTypeKind, string>> _triggerTypeKinds =
        new Lazy<ImmutableDictionary<TriggerTypeKind, string>>(() =>
            GenerateTriggerTypeKindDictionary());

    private static readonly Dictionary<string, ImmutableDictionary<string, string>> ordinalDictionary = new Dictionary<string, ImmutableDictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
    {
        { "Access",                 _accessibilityDictionary.Value },
        { "AllowInCustomizations",  _allowInCustomizationsKindDictionary.Value },
        { "BlankNumbers",           _blankNumbersKindDictionary.Value },
        { "Caption",                _labelPropertyDictionary.Value },
        { "CompressionType",        _compressionTypeKindDictionary.Value },
        { "DataClassification",     _dataClassificationKindDictionary.Value },
        { "FieldClass",             _fieldClassKindDictionary.Value },
        { "ObsoleteState",          _obsoleteStateKindDictionary.Value },
        { "Scope",                  _scopeKindDictionary.Value },
        { "Subtype",                _subtypeKindDictionary.Value },
        { "TableType",              _tableTypeKindDictionary.Value},
    };

    #endregion

    public override void Initialize(AnalysisContext context)
    {
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

    private void CheckForBuiltInTypeCasingMismatch(SymbolAnalysisContext ctx)
    {
        var node = ctx.Symbol.DeclaringSyntaxReference?.GetSyntax(ctx.CancellationToken);
        if (node is null)
            return;

        var semanticModel = ctx.Compilation.GetSemanticModel(node.SyntaxTree);

        AnalyzeTokens(ctx, node);
        AnalyzeNodes(ctx, semanticModel, node);
    }

    private void AnalyzeTokens(SymbolAnalysisContext ctx, SyntaxNode root)
    {
        var tokens = root.DescendantTokens()
                         .Where(t => !string.IsNullOrEmpty(t.ValueText) &&
                                t.Kind.IsKeyword() &&
                                t.Parent is not null &&
                                !_dataTypeSyntaxKinds.Contains(t.Parent.Kind));

        foreach (SyntaxToken token in tokens)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();

            SyntaxToken syntaxToken = SyntaxFactory.Token(token.Kind);
            if (syntaxToken.Kind == SyntaxKind.None)
                continue;

            if (!syntaxToken.ValueText.AsSpan().Equals(token.ValueText.AsSpan(), StringComparison.Ordinal))
                ctx.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration,
                        token.GetLocation(),
                        syntaxToken));
        }
    }

    private void AnalyzeNodes(SymbolAnalysisContext ctx, SemanticModel semanticModel, SyntaxNode root)
    {
        var stack = new Stack<SyntaxNode>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();
            SyntaxNode node = stack.Pop();

            switch (node.Kind)
            {
                case SyntaxKind.DataType:
                case SyntaxKind.EnumDataType:
                case SyntaxKind.LengthDataType:
                case SyntaxKind.OptionDataType:
                case SyntaxKind.SubtypedDataType:
                case SyntaxKind.TextConstDataType:
                    AnalyzeDataType(ctx, node);
                    continue;

                case SyntaxKind.IdentifierName:
                    AnalyzeIdentifierName(ctx, semanticModel, node);
                    continue;

                case SyntaxKind.LabelDataType:
                    AnalyzeLabelDataType(ctx, node);
                    continue;

                case SyntaxKind.MemberAccessExpression:
                    AnalyzeMemberAccessExpression(ctx, node);
                    continue;

                case SyntaxKind.OptionAccessExpression:
                    AnalyzeOptionAccessExpression(ctx, stack, node);
                    continue;

                case SyntaxKind.PageArea:
                    AnalyzePageArea(ctx, node);
                    continue;

                case SyntaxKind.PageActionArea:
                    AnalyzePageActionArea(ctx, node);
                    continue;

                case SyntaxKind.Property:
                    AnalyzeProperty(ctx, stack, node);
                    continue;

                case SyntaxKind.QualifiedName:
                    AnalyzeQualifiedName(ctx, semanticModel, node);
                    continue;

                case SyntaxKind.TriggerDeclaration:
                    AnalyzeTriggerDeclaration(ctx, semanticModel, node);
                    continue;

                default:
                    break;
            }

            foreach (var child in node.ChildNodes().Reverse())
            {
                stack.Push(child);
            }
        }
    }

    #region Name

    private void AnalyzeDataType(SymbolAnalysisContext ctx, SyntaxNode syntaxNode)
    {
        if (syntaxNode is not DataTypeSyntax node)
            return;

        CompareAgainstDictionary(ctx, node.TypeName, _navTypeKindDictionary.Value);
    }

    private void AnalyzeIdentifierName(SymbolAnalysisContext ctx, SemanticModel semanticModel, SyntaxNode syntaxNode)
    {
        if (syntaxNode is not IdentifierNameSyntax node)
            return;

        // The GetSymbolInfo from the ctx.SemanticModel will throw an System.NullReferenceException on these ParentKind nodes
        if (node.Parent.Kind == SyntaxKind.PragmaWarningDirectiveTrivia ||
            node.Parent.Kind == SyntaxKind.UnaryNotExpression)
            return;

        if (semanticModel.GetSymbolInfo(node, ctx.CancellationToken).Symbol is not ISymbol fieldSymbol)
            return;


        CompareIdentifier(ctx, node.Identifier, fieldSymbol.Name);
    }

    private void AnalyzeLabelDataType(SymbolAnalysisContext ctx, SyntaxNode syntaxNode)
    {
        if (syntaxNode is not LabelDataTypeSyntax node)
            return;

        CompareAgainstDictionary(ctx, node.TypeName, _navTypeKindDictionary.Value);

        // Handle the properties like Comment, Locked and/or MaxLength
        var properties = node.DescendantNodes().OfType<IdentifierEqualsLiteralSyntax>();
        foreach (var prop in properties)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();
            CompareAgainstDictionary(ctx, prop.Identifier, _labelPropertyDictionary.Value);
        }
    }

    private void AnalyzeMemberAccessExpression(SymbolAnalysisContext ctx, SyntaxNode syntaxNode)
    {
        if (syntaxNode is not MemberAccessExpressionSyntax node)
            return;

        var childNodes = node.ChildNodes().OfType<IdentifierNameSyntax>();
        foreach (var childNode in childNodes)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();
            CompareAgainstDictionary(ctx, childNode.Identifier, _symbolKindDictionary.Value);
        }
    }

    private void AnalyzeOptionAccessExpression(SymbolAnalysisContext ctx, Stack<SyntaxNode> stack, SyntaxNode syntaxNode)
    {
        if (syntaxNode is not OptionAccessExpressionSyntax node)
            return;

        switch (node.Expression)
        {
            case OptionAccessExpressionSyntax optionAccessExpressionSyntax:
                stack.Push(optionAccessExpressionSyntax.Expression);
                break;
            case IdentifierNameSyntax identifierNameSyntax:
                CompareAgainstDictionary(ctx, identifierNameSyntax.Identifier, _symbolKindDictionary.Value);
                break;
        }
    }

    private void AnalyzePageArea(SymbolAnalysisContext ctx, SyntaxNode syntaxNode)
    {
        if (syntaxNode is not PageAreaSyntax node)
            return;

        var childNodes = node.ChildNodes().OfType<IdentifierNameSyntax>();
        foreach (var childNode in childNodes)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();
            CompareAgainstDictionary(ctx, childNode.Identifier, _areaKindDictionary.Value);
        }
    }

    private void AnalyzePageActionArea(SymbolAnalysisContext ctx, SyntaxNode syntaxNode)
    {
        if (syntaxNode is not PageActionAreaSyntax node)
            return;

        var childNodes = node.ChildNodes().OfType<IdentifierNameSyntax>();
        foreach (var childNode in childNodes)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();
            CompareAgainstDictionary(ctx, childNode.Identifier, _actionAreaKindDictionary.Value);
        }
    }

    private void AnalyzeProperty(SymbolAnalysisContext ctx, Stack<SyntaxNode> stack, SyntaxNode syntaxNode)
    {
        if (syntaxNode is not PropertySyntax node)
            return;

        if (node.Name is not PropertyNameSyntax propertyNameSyntax)
            return;

        CompareAgainstDictionary(ctx, propertyNameSyntax.Identifier, _propertyKindDictionary.Value);

        var propertyName = propertyNameSyntax.Identifier.ValueText;
        if (string.IsNullOrEmpty(propertyName))
            return;

        if (ordinalDictionary.ContainsKey(propertyName))
        {
            if (node.Value is EnumPropertyValueSyntax enumPropValueSyntax)
            {
                CompareAgainstDictionary(ctx, enumPropValueSyntax.Value.Identifier, GetOrdinalDictionary(propertyName));
            }

            return;
        }

        switch (propertyName.ToLower())
        {
            // Push the list of fields back into the stack, which will be processed by the SyntaxKind.IdentifierName
            case "accessbypermission":
            case "columnstoreindex":
            case "calcformula":
            case "datacaptionfields":
            case "drilldownpageid":
            case "lookuppageid":
                stack.Push(node.Value);
                break;

            // Handle the properties like Comment, Locked and/or MaxLength
            case "caption":
                var properties = node.DescendantNodes().OfType<IdentifierEqualsLiteralSyntax>();
                foreach (var prop in properties)
                {
                    ctx.CancellationToken.ThrowIfCancellationRequested();
                    CompareAgainstDictionary(ctx, prop.Identifier, GetOrdinalDictionary(propertyName));
                }
                break;
        }
    }

    private void AnalyzeQualifiedName(SymbolAnalysisContext ctx, SemanticModel semanticModel, SyntaxNode syntaxNode)
    {
        if (syntaxNode is not QualifiedNameSyntax node)
            return;

        if (semanticModel.GetSymbolInfo(node, ctx.CancellationToken).Symbol is not ISymbol fieldSymbol)
            return;

        switch (node.Left.Kind)
        {
            // without namespace
            case SyntaxKind.IdentifierName:

                if (fieldSymbol.ContainingSymbol is not IObjectTypeSymbol objectTypeSymbol)
                    return;

                if (fieldSymbol.ContainingSymbol.Kind == SymbolKind.TableExtension)
                {
                    ITableExtensionTypeSymbol tableExtension = (ITableExtensionTypeSymbol)fieldSymbol.ContainingSymbol;
                    if (tableExtension.Target is not IObjectTypeSymbol tableExtensionTypeSymbol)
                    {
                        return;
                    }
                    objectTypeSymbol = tableExtensionTypeSymbol;
                }

                if (node.Left is IdentifierNameSyntax leftNode)
                    CompareIdentifier(ctx, leftNode.Identifier, objectTypeSymbol.Name);

                if (node.Right is SimpleNameSyntax rightNode)
                    CompareIdentifier(ctx, rightNode.Identifier, fieldSymbol.Name);

                break;

            // with namespace
            default:
                CompareIdentifier(ctx, node.Right.Identifier, fieldSymbol.Name);

                break;
        }
    }

    private void AnalyzeTriggerDeclaration(SymbolAnalysisContext ctx, SemanticModel semanticModel, SyntaxNode syntaxNode)
    {
        if (syntaxNode is not TriggerDeclarationSyntax node)
            return;

        var IdentifierName = node.Name.Identifier.ValueText;
        if (string.IsNullOrEmpty(IdentifierName))
            return;

        if (ctx.Symbol.GetContainingApplicationObjectTypeSymbol() is not ISymbolWithTriggers symbolWithTriggers)
            return;

        if (symbolWithTriggers.GetTriggerTypeInfo(IdentifierName) is not TriggerTypeInfo triggerTypeInfo)
            return;

        if (!_triggerTypeKinds.Value.TryGetValue(triggerTypeInfo.Kind, out string? canonical))
            return;

        CompareIdentifier(ctx, node.Name.Identifier, canonical);
    }

    #endregion

    private void CheckForBuiltInMethodsWithCasingMismatch(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved())
            return;

        var targetName = string.Empty;

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

        if (string.IsNullOrEmpty(targetName))
            return;

        ReadOnlySpan<char> targetSpan = targetName.AsSpan();

        if (OnlyDiffersInCasing(ctx.Operation.Syntax.ToString().UnquoteIdentifier().AsSpan(), targetSpan))
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration,
                ctx.Operation.Syntax.GetLocation(),
                targetName,
                string.Empty));
            return;
        }

        foreach (var descendant in ctx.Operation.Syntax.DescendantNodes())
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();

            if (OnlyDiffersInCasing(descendant.ToString().UnquoteIdentifier().AsSpan(), targetSpan))
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration,
                    ctx.Operation.Syntax.GetLocation(),
                    targetName,
                    string.Empty));
                return;
            }
        }
    }

    #region Comparators

    private ImmutableDictionary<string, string> GetOrdinalDictionary(string identifier)
    {
        ordinalDictionary.TryGetValue(identifier, out var dictionary);
        return dictionary;
    }

    private bool OnlyDiffersInCasing(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
    {
        return left.Equals(right, StringComparison.OrdinalIgnoreCase) &&
               !left.Equals(right, StringComparison.Ordinal);
    }

    private void CompareIdentifier(SymbolAnalysisContext ctx, SyntaxToken? identifier, string canonical)
    {
        if (identifier == null)
            return;

        string? tokenText = identifier?.ValueText?.UnquoteIdentifier();
        if (string.IsNullOrEmpty(tokenText))
            return;

        var location = identifier?.GetLocation();
        if (location == null)
            return;

        if (!tokenText.AsSpan().Equals(canonical.AsSpan(), StringComparison.Ordinal))
        {
            ctx.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration,
                    location,
                    canonical,
                    string.Empty));
        }
    }

    private void CompareAgainstDictionary(SymbolAnalysisContext ctx, SyntaxToken Identifier, ImmutableDictionary<string, string> lookupDictionary)
    {
        string? tokenText = Identifier.ValueText;
        if (string.IsNullOrEmpty(tokenText))
            return;

        if (!lookupDictionary.TryGetValue(tokenText, out string? canonical))
            return;

        if (!tokenText.AsSpan().Equals(canonical.AsSpan(), StringComparison.Ordinal))
        {
            ctx.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration,
                    Identifier.GetLocation(),
                    canonical,
                    string.Empty));
        }
    }

    #endregion

    #region Dictionary Builders

    private static ImmutableDictionary<string, string> GenerateNavTypeKindDictionary()
    {
        var builder = ImmutableDictionary.CreateBuilder<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (NavTypeKind navTypeKind in Enum.GetValues(typeof(NavTypeKind)))
        {
            string KindName = navTypeKind.ToString();
            builder[KindName] = KindName;
        }
        builder["Database"] = "Database"; // for Database::"G/L Entry" (there is no NavTypeKind for this)

        return builder.ToImmutable();
    }

    private static ImmutableDictionary<string, string> GenerateSymbolKindDictionary()
    {
        var builder = ImmutableDictionary.CreateBuilder<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (SymbolKind kind in Enum.GetValues(typeof(SymbolKind)))
        {
            string kindName = kind.ToString();
            // Change "XmlPort" to "Xmlport"
            if (kindName == "XmlPort")
            {
                kindName = "Xmlport";
            }
            builder[kindName] = kindName;
        }

        builder["Database"] = "Database";  // for Database::"G/L Entry" (there is no SymbolKind for this)

        return builder.ToImmutable();
    }

    private static ImmutableDictionary<TriggerTypeKind, string> GenerateTriggerTypeKindDictionary()
    {
        var builder = ImmutableDictionary.CreateBuilder<TriggerTypeKind, string>();

        foreach (TriggerTypeKind type in Enum.GetValues(typeof(TriggerTypeKind)))
        {
            string typeName = type.ToString();
            int index = typeName.IndexOf("On");
            if (index > 0)
            {
                builder[type] = typeName.Substring(index);
            }
        }

        return builder.ToImmutable();
    }

    #endregion
}
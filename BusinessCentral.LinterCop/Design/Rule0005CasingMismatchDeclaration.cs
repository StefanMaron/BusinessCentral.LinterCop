using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.InternalSyntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0005CasingMismatchDeclaration : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
        = ImmutableArray.Create(
            DiagnosticDescriptors.Rule0000ErrorInRule,
            DiagnosticDescriptors.Rule0005CasingMismatch
            );

    // These SyntaxKind identifiers are not relevant for analysis.
    // Excluding these from the stack while collecting nodes to improve performance.
    private static readonly HashSet<SyntaxKind> _skipAnalyzeIdentifierKinds = new HashSet<SyntaxKind>
    {
        SyntaxKind.CodeunitObject,
        SyntaxKind.ControlAddInObject,
        SyntaxKind.EnumType,
        SyntaxKind.EnumValue,
        SyntaxKind.EnumExtensionType,
        SyntaxKind.Entitlement,
        SyntaxKind.Field,
        SyntaxKind.Interface,
        SyntaxKind.MethodDeclaration,
        SyntaxKind.Parameter,
        SyntaxKind.QueryColumn,
        SyntaxKind.QueryDataItem,
        SyntaxKind.QueryFilter,
        SyntaxKind.QueryObject,
        SyntaxKind.PageObject,
        SyntaxKind.PageExtensionObject,
        SyntaxKind.PageCustomizationObject,
        SyntaxKind.PermissionSet,
        SyntaxKind.PermissionSetExtension,
        SyntaxKind.ProfileObject,
        SyntaxKind.ProfileExtensionObject,
        SyntaxKind.ReportColumn,
        SyntaxKind.ReportObject,
        SyntaxKind.ReportExtensionObject,
        SyntaxKind.ReportDataItem,
        SyntaxKind.ReportLabel,
        SyntaxKind.ReportLayout,
        SyntaxKind.ReturnValue,
        SyntaxKind.TableObject,
        SyntaxKind.TableExtensionObject,
        SyntaxKind.XmlPortObject
    };

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckNodes), new SymbolKind[] {
                SymbolKind.Codeunit,
                SymbolKind.Entitlement,
                SymbolKind.Enum,
                SymbolKind.EnumExtension,
                SymbolKind.Interface,
                SymbolKind.Page,
                SymbolKind.PageExtension,
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

    // Collecting nodes by traversing from the top of the object into into's childnodes
    // For each childnode determine to add the node to the stack for analyzing
    // Grouping nodes for increase performance
    private void CheckNodes(SymbolAnalysisContext ctx)
    {
        var node = ctx.Symbol.DeclaringSyntaxReference?.GetSyntax(ctx.CancellationToken);
        if (node is null)
            return;

        Dictionary<AnalyzeKind, List<SyntaxNode>> collectedNodes = new();
        var semanticModel = ctx.Compilation.GetSemanticModel(node.SyntaxTree);

        CollectNodes(ctx, node, collectedNodes);
        AnalyzeNodes(ctx, semanticModel, collectedNodes);
    }

    #region Collect Nodes

    private void CollectNodes(SymbolAnalysisContext ctx, SyntaxNode root, Dictionary<AnalyzeKind, List<SyntaxNode>> collectedNodes)
    {
        var stack = new Stack<SyntaxNode>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();
            SyntaxNode node = stack.Pop();
            var nodeKind = node.Kind;

            switch (nodeKind)
            {
                case SyntaxKind.DataType:
                case SyntaxKind.LengthDataType:
                case SyntaxKind.OptionDataType:
                case SyntaxKind.TextConstDataType:
                    AddToCollection(AnalyzeKind.DataType, node, collectedNodes);
                    // These SyntaxKind don't have any ChildNodes (early exit)
                    continue;

                case SyntaxKind.EnumDataType:       // Possible ChildNodes (ObjectNameReference => IdentifierName)
                case SyntaxKind.LabelDataType:      // Possible ChildNodes (CommaSeparatedIdentifierEqualsLiteralList => IdentifierEqualsLiteral)
                    AddToCollection(AnalyzeKind.DataType, node, collectedNodes);
                    // Contine processing ChildNodes
                    break;

                case SyntaxKind.FieldGroup:
                    CollectFieldGroup(stack, node);
                    // The ChildNodes are handled by the CollectFieldGroup method
                    continue;

                case SyntaxKind.IdentifierEqualsLiteral:
                    AddToCollection(AnalyzeKind.IdentifierEqualsLiteral, node, collectedNodes);
                    // ChildNodes are handeld by the AnalyzeIdentifierEqualsLiteraly method
                    continue;

                case SyntaxKind.IdentifierName:
                    CollectIdentifierName(stack, node, collectedNodes);
                    // ChildNodes are handeld by the AnalyzeIdentifierEqualsLiteraly method
                    continue;

                case SyntaxKind.Key:
                    CollectKey(stack, node);
                    // ChildNodes are handled by the CollectKey method
                    continue;

                case SyntaxKind.MemberAccessExpression:
                    CollectMemberAccessExpression(stack, node);
                    // ChildNodes handled by CollectMemberAccessExpression method
                    continue;

                case SyntaxKind.MemberAttribute:
                    CollectMemberAttribute(stack, node, collectedNodes);
                    // ChildNodes handled by CollectMemberAttribute method
                    continue;

                case SyntaxKind.ObjectNameReference:
                    // TODO: Example case interface "MyInterfaceExt" extends "MyInterface"
                    // Exclude "MyInterface" from analyzing (currently not supported)
                    if (node.Parent.Kind == SyntaxKind.Interface)
                        continue;
                    else
                        break;

                case SyntaxKind.OptionAccessExpression:
                    AddToCollection(AnalyzeKind.OptionAccessExpression, node, collectedNodes);
                    // ChildNodes handled by AnalyzeOptionAccessExpression method
                    continue;

                case SyntaxKind.PageArea:
                    AddToCollection(AnalyzeKind.Area, node, collectedNodes);
                    // Contine processing ChildNodes
                    break;

                case SyntaxKind.PageActionArea:
                    AddToCollection(AnalyzeKind.ActionArea, node, collectedNodes);
                    // Contine processing ChildNodes
                    break;

                case SyntaxKind.Property:
                    CollectProperty(stack, node, collectedNodes);
                    // ChildNodes handled by AnalyzeProperty method
                    continue;

                case SyntaxKind.PropertyName:
                    AddToCollection(AnalyzeKind.PropertyName, node, collectedNodes);
                    // ChildNodes handled by AnalyzePropertyName method
                    continue;

                case SyntaxKind.SubtypedDataType:
                    CollectSubtypedDataType(node, collectedNodes);
                    // ChildNodes handled by AnalyzePropertyName method
                    continue;

                case SyntaxKind.QualifiedName:
                    AddToCollection(AnalyzeKind.QualifiedName, node, collectedNodes);
                    // ChildNodes handled by AnalyzeQualifiedName method
                    continue;

                case SyntaxKind.TriggerDeclaration:
                    AddToCollection(AnalyzeKind.TriggerDeclaration, node, collectedNodes);
                    // ChildNodes handled by AnalyzeTriggerDeclaration method
                    continue;
            }

            bool skipIdentifier = _skipAnalyzeIdentifierKinds.Contains(node.Kind);
#if DEBUG
            // The .Reverse() creates an intermediate IEnumerable<SyntaxNode> collection, leading to extra memory allocation
            // However, during debugging, it is helpful to process the ChildNodes in a top-down sequence for easier analysis
            foreach (var child in node.ChildNodes().Reverse())
#else
            foreach (var child in node.ChildNodes())
#endif
            {
                SyntaxKind childKind = child.Kind;
                if (childKind == SyntaxKind.ObjectId ||
                    childKind == SyntaxKind.LiteralAttributeArgument ||
                    childKind == SyntaxKind.LiteralExpression
                )
                    continue;

                if (childKind == SyntaxKind.IdentifierName && skipIdentifier)
                    continue;

                if (IsEmptyList(child))
                    continue;

                stack.Push(child);
            }
        }
    }

    private bool IsEmptyList(SyntaxNode node) =>
        node switch
        {
            ArgumentListSyntax argList => argList.Arguments.Count == 0,
            AttributeArgumentListSyntax attrList => attrList.Arguments.Count == 0,
            FieldGroupListSyntax fldGrpList => fldGrpList.FieldGroups.Count == 0,
            FieldGroupExtensionListSyntax fldGrpExtList => fldGrpExtList.Changes.Count == 0,
            FieldListSyntax fldList => fldList.Fields.Count == 0,
            FieldExtensionListSyntax fldExtList => fldExtList.Fields.Count == 0,
            KeyListSyntax keyList => keyList.Keys.Count == 0,
            PageActionListSyntax pageActList => pageActList.Areas.Count == 0,
            PageActionAreaSyntax pageActArea => pageActArea.Actions.Count == 0,
            PageExtensionActionListSyntax pageExtActList => pageExtActList.Changes.Count == 0,
            PageViewListSyntax pageViewList => pageViewList.Views.Count == 0,
            PageExtensionViewListSyntax pageExtViewList => pageExtViewList.Changes.Count == 0,
            ParameterListSyntax paramList => paramList.Parameters.Count == 0,
            PropertyListSyntax propList => propList.Properties.Count == 0,
            _ => false
        };

    private void AddToCollection(AnalyzeKind kind, SyntaxNode node, Dictionary<AnalyzeKind, List<SyntaxNode>> collectedNodes)
    {
        if (!collectedNodes.TryGetValue(kind, out var list))
        {
            list = new List<SyntaxNode>();
            collectedNodes[kind] = list;
        }
        list.Add(node);
    }

    private void CollectFieldGroup(Stack<SyntaxNode> stack, SyntaxNode syntaxNode)
    {
        if (syntaxNode is not FieldGroupSyntax node)
            return;

        var children = node.ChildNodes().ToArray();

        // Exclude the IdentifierName (DropDown/Brick)
        int startIndex = (children.Length > 0 && children[0] is IdentifierNameSyntax) ? 1 : 0;

        for (int i = startIndex; i < children.Length; i++)
        {
            stack.Push(children[i]);
        }
    }

    private void CollectIdentifierName(Stack<SyntaxNode> stack, SyntaxNode syntaxNode, Dictionary<AnalyzeKind, List<SyntaxNode>> collectedNodes)
    {
        if (syntaxNode is not IdentifierNameSyntax node)
            return;

        // Mitigate false positive in Links/Notes systempart
        if (node.Parent.Kind == SyntaxKind.PageSystemPart)
            return;

        if (string.Equals(node.Identifier.ValueText, "Rec", StringComparison.OrdinalIgnoreCase))
            return;

        // Handle system objects, like AccessByPermission = system "Allow Action Export To Excel" = X;
        // The AL0443 diagnostic will handle this https://learn.microsoft.com/nl-be/dynamics365/business-central/dev-itpro/developer/diagnostics/diagnostic-al443
        if (node.Parent?.Parent is PermissionSyntax permissionSyntax &&
            permissionSyntax.ObjectType.Kind == SyntaxKind.SystemKeyword)
        {
            return;
        }

        AddToCollection(AnalyzeKind.IdentifierName, node, collectedNodes);
    }

    private void CollectKey(Stack<SyntaxNode> stack, SyntaxNode syntaxNode)
    {
        if (syntaxNode is not KeySyntax node)
            return;

        foreach (var field in node.Fields)
        {
            stack.Push(field);
        }
    }

    private void CollectMemberAccessExpression(Stack<SyntaxNode> stack, SyntaxNode syntaxNode)
    {
        if (syntaxNode is not MemberAccessExpressionSyntax node)
            return;

        // Extract the Expression and push it back into the stack.
        // In case of MyRecord.Get(), we're only interested in the MyRecord-part (IdentifierNameSyntax), the .Get() is already handeld
        // or MyRecord.MyField, we're only interested in the MyRecord-part (IdentifierNameSyntax), the .MyField is already handeld
        stack.Push(node.Expression);
    }

    private void CollectMemberAttribute(Stack<SyntaxNode> stack, SyntaxNode syntaxNode, Dictionary<AnalyzeKind, List<SyntaxNode>> collectedNodes)
    {
        if (syntaxNode is not MemberAttributeSyntax node)
            return;

        var nodesToProcess = node.ChildNodes();
        foreach (var child in nodesToProcess)
        {
            if (child is IdentifierNameSyntax)
                AddToCollection(AnalyzeKind.Attribute, child, collectedNodes);
            else
                stack.Push(child);
        }
    }

    private void CollectProperty(Stack<SyntaxNode> stack, SyntaxNode syntaxNode, Dictionary<AnalyzeKind, List<SyntaxNode>> collectedNodes)
    {
        if (syntaxNode is not PropertySyntax node)
            return;

        switch (node.Value)
        {
            case EnumPropertyValueSyntax:
                AddToCollection(AnalyzeKind.Property, syntaxNode, collectedNodes);
                break;

            case CommaSeparatedPropertyValueSyntax:
                // TODO: Find a way to validate the values of Application Area property (All, Basic, Suite)
                if (string.Equals(node.Name.Identifier.ValueText, "ApplicationArea", StringComparison.OrdinalIgnoreCase))
                {
                    stack.Push(node.Name);
                }
                else
                {
                    goto default;
                }
                break;

            case CommaSeparatedIdentifierOrLiteralPropertyValueSyntax:
                //TODO: The semanticModel.GetSymbolInfo on the AnalyzeIdentifierName can't resolve these identifiers, so skip adding these to the stack
                if (string.Equals(node.Name.Identifier.ValueText, "ValuesAllowed", StringComparison.OrdinalIgnoreCase))
                    return;
                goto default;

            case ImagePropertyValueSyntax:          // Images are handeld by the AL0482 compiler diagnostic
            case StringPropertyValueSyntax:         // Do not analyze StringLiterals
            case OptionValuePropertyValueSyntax:    // Do not analyze Option
            case OptionValuesPropertyValueSyntax:   // Do not analyze Options
                stack.Push(node.Name);
                break;

            default:
                foreach (var child in syntaxNode.ChildNodes())
                    stack.Push(child);
                break;
        }
    }

    private void CollectSubtypedDataType(SyntaxNode syntaxNode, Dictionary<AnalyzeKind, List<SyntaxNode>> collectedNodes)
    {
        if (syntaxNode is not SubtypedDataTypeSyntax node)
            return;

        if (node.Subtype.Kind == SyntaxKind.ObjectReference)
            AddToCollection(AnalyzeKind.DataType, syntaxNode, collectedNodes);
    }

    #endregion

    #region Analyze Nodes

    private void AnalyzeNodes(SymbolAnalysisContext ctx, SemanticModel semanticModel, Dictionary<AnalyzeKind, List<SyntaxNode>> collectedNodes)
    {
        foreach (var kvp in collectedNodes)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();

            AnalyzeKind kind = kvp.Key;
            List<SyntaxNode> nodes = kvp.Value;

            switch (kind)
            {
                case AnalyzeKind.DataType:
                    AnalyzeDataType(ctx, nodes);
                    continue;

                case AnalyzeKind.IdentifierEqualsLiteral:
                    AnalyzeIdentifierEqualsLiteraly(ctx, nodes, GetOrdinalDictionary(kind));
                    continue;

                case AnalyzeKind.IdentifierName:
                    AnalyzeIdentifierName(ctx, semanticModel, nodes);
                    continue;

                case AnalyzeKind.Attribute:
                    AnalyzeIdentifier(ctx, nodes, GetOrdinalDictionary(kind));
                    continue;

                case AnalyzeKind.OptionAccessExpression:
                    AnalyzeOptionAccessExpression(ctx, semanticModel, nodes);
                    continue;

                case AnalyzeKind.Area:
                case AnalyzeKind.ActionArea:
                    AnalyzeChildNodeIdentifiers(ctx, nodes, GetOrdinalDictionary(kind));
                    continue;

                case AnalyzeKind.Property:
                    AnalyzeProperty(ctx, nodes, GetOrdinalDictionary(kind));
                    break;

                case AnalyzeKind.PropertyName:
                    AnalyzePropertyName(ctx, nodes, GetOrdinalDictionary(kind));
                    break;

                case AnalyzeKind.QualifiedName:
                    AnalyzeQualifiedName(ctx, semanticModel, nodes);
                    break;

                case AnalyzeKind.TriggerDeclaration:
                    AnalyzeTriggerDeclaration(ctx, semanticModel, nodes);
                    break;
            }
        }
    }

    private static List<string> dataTypes = new List<string>();

    private void AnalyzeDataType(SymbolAnalysisContext ctx, List<SyntaxNode> nodes)
    {
        foreach (DataTypeSyntax dataTypeSyntax in nodes)
        {
            var name = dataTypeSyntax.TypeName.ValueText;
            if (string.IsNullOrEmpty(name))
                continue;
            if (!dataTypes.Contains(name))
                dataTypes.Add(name);
        }

        var syntaxNodes = nodes.OfType<DataTypeSyntax>();
        var lookupDictionary = _navTypeKindDictionary;

        foreach (var syntaxNode in syntaxNodes)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();
            CompareAgainstDictionary(ctx, syntaxNode.TypeName, lookupDictionary);
        }
    }

    private void AnalyzeIdentifierEqualsLiteraly(SymbolAnalysisContext ctx, List<SyntaxNode> nodes, Lazy<ImmutableDictionary<string, string>>? lookupDictionary)
    {
        // The SyntaxKind.IdentifierEqualsLiteral currently are properties from a Caption
        // a Caption property (again currently) shares the same properties like Comment, Locked and/or MaxLength as a Label variable
        foreach (var syntaxNode in nodes)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();
            if (syntaxNode is IdentifierEqualsLiteralSyntax node)
                CompareAgainstDictionary(ctx, node.Identifier, lookupDictionary);
        }
    }

    private void AnalyzeIdentifierName(SymbolAnalysisContext ctx, SemanticModel semanticModel, List<SyntaxNode> nodes)
    {
        // Increasing performance on the GetSymbolInfo method by grouping nodes with the same Identifier
        var groupNodes = nodes
            .OfType<IdentifierNameSyntax>()
            // The GetSymbolInfo from the ctx.SemanticModel will throw an System.NullReferenceException on these ParentKind nodes
            .Where(node => node.Parent.Kind != SyntaxKind.PragmaWarningDirectiveTrivia &&
                           node.Parent.Kind != SyntaxKind.UnaryNotExpression)
            .ToLookup(node => node.Identifier.ValueText, StringComparer.Ordinal);

        foreach (var groupNode in groupNodes)
        {
            var representative = groupNode.OrderBy(node => node.Position).Last();

            if (semanticModel.GetSymbolInfo(representative, ctx.CancellationToken).Symbol is not ISymbol symbol)
            {
#if DEBUG
                var message = $"SymbolInfo not available for '{representative.Identifier.ValueText?.QuoteIdentifierIfNeeded()}' on IdentifierNameSyntax.";
                RaiseImproveRuleDiagnostic(ctx, groupNode, message);
#endif
                continue;
            }

            foreach (var node in groupNode)
            {
                ctx.CancellationToken.ThrowIfCancellationRequested();
                CompareIdentifier(ctx, node.Identifier, symbol.Name);
            }
        }
    }

    private void AnalyzeChildNodeIdentifiers(SymbolAnalysisContext ctx, IEnumerable<SyntaxNode> nodes, Lazy<ImmutableDictionary<string, string>>? lookupDictionary)
    {
        foreach (var syntaxNode in nodes)
            AnalyzeIdentifier(ctx, syntaxNode.ChildNodes(), lookupDictionary);
    }

    private void AnalyzeIdentifier(SymbolAnalysisContext ctx, IEnumerable<SyntaxNode> nodes, Lazy<ImmutableDictionary<string, string>>? lookupDictionary)
    {
        foreach (var syntaxNode in nodes)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();
            if (syntaxNode is IdentifierNameSyntax node)
                CompareAgainstDictionary(ctx, node.Identifier, lookupDictionary);
        }
    }

    private void AnalyzeOptionAccessExpression(SymbolAnalysisContext ctx, SemanticModel semanticModel, List<SyntaxNode> nodes)
    {
        var symbolKindDict = _symbolKindDictionary.Value;

        foreach (var syntaxNode in nodes)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();
            if (syntaxNode is not OptionAccessExpressionSyntax node)
                return;

            // Unwrap an inner OptionAccess if present
            var exprNode = node.Expression;
            if (exprNode is OptionAccessExpressionSyntax innerOption &&
                innerOption.Expression is IdentifierNameSyntax idName)
            {
                CompareAgainstDictionary(ctx, idName.Identifier, _symbolKindDictionary);
                exprNode = innerOption.Name;
            }

            if (exprNode is not IdentifierNameSyntax expression)
                return;

            string? expressionText = expression.Identifier.ValueText;
            if (string.IsNullOrEmpty(expressionText))
                return;

            if (node.Name is not IdentifierNameSyntax name)
                return;

            string? nameText = name.Identifier.ValueText;
            if (string.IsNullOrEmpty(nameText))
                return;

            // Check if Expression is SymbolKind
            bool isExpressionSymbolKind = symbolKindDict.ContainsKey(expressionText);
            if (isExpressionSymbolKind)
            {
                CompareAgainstDictionary(ctx, expression.Identifier, _symbolKindDictionary);

                // If the name is also a known SymbolKind; process and exit.
                if (symbolKindDict.ContainsKey(nameText))
                {
                    CompareAgainstDictionary(ctx, name.Identifier, _symbolKindDictionary);
                    return;
                }
            }

            // Otherwise, use the semantic model to get symbol info.
            if (semanticModel.GetSymbolInfo(node, ctx.CancellationToken).Symbol is not ISymbol symbol)
            {
#if DEBUG
                var message = $"SymbolInfo not available for '{name.Identifier.ValueText?.QuoteIdentifierIfNeeded()}' on OptionAccessExpressionSyntax.";
                RaiseImproveRuleDiagnostic(ctx, nodes, message);
#endif
                return;
            }
        }
    }

    private void AnalyzeProperty(SymbolAnalysisContext ctx, List<SyntaxNode> nodes, Lazy<ImmutableDictionary<string, string>>? lookupDictionary)
    {
        var propertyNames = nodes.OfType<PropertySyntax>()
                                 .Select(n => n.Name)
                                 .AsEnumerable<SyntaxNode>();
        AnalyzePropertyName(ctx, propertyNames, lookupDictionary);

        var propOrdinalDict = propertyOrdinalDictionary.Value;
        var syntaxNodes = nodes.OfType<PropertySyntax>();
        foreach (var node in syntaxNodes)
        {
            if (node.Name is not PropertyNameSyntax propertyNameSyntax)
                return;

            var propertyName = propertyNameSyntax.Identifier.ValueText;
            if (string.IsNullOrEmpty(propertyName))
                return;

            if (propOrdinalDict.ContainsKey(propertyName))
            {
                if (node.Value is EnumPropertyValueSyntax enumPropValueSyntax)
                {
                    CompareAgainstDictionary(ctx, enumPropValueSyntax.Value.Identifier, GetOrdinalDictionary(propertyName));
                }
            }
            else
            {
                if (node.Value is EnumPropertyValueSyntax)
                {
                    var message = $"missing '{propertyName}' ordinals.";
                    RaiseImproveRuleDiagnostic(ctx, node.Value, message);
                }
            }
        }
    }

    private void AnalyzePropertyName(SymbolAnalysisContext ctx, IEnumerable<SyntaxNode> nodes, Lazy<ImmutableDictionary<string, string>>? lookupDictionary)
    {
        var syntaxNodes = nodes.OfType<PropertyNameSyntax>();
        foreach (var node in syntaxNodes)
            CompareAgainstDictionary(ctx, node.Identifier, lookupDictionary);
    }

    private void AnalyzeQualifiedName(SymbolAnalysisContext ctx, SemanticModel semanticModel, List<SyntaxNode> nodes)
    {
        // Increasing performance on the GetSymbolInfo() method by grouping nodes with the same Identifier
        var groupNodes = nodes
            .OfType<QualifiedNameSyntax>()
            // A small overhead for using .ToString() here as we need to include the node.Left and node.Right
            .ToLookup(node => node.ToString(), StringComparer.OrdinalIgnoreCase);

        foreach (var groupNode in groupNodes)
        {
            var representative = groupNode.OrderBy(node => node.Position).Last();
            var symbol = semanticModel.GetSymbolInfo(representative, ctx.CancellationToken).Symbol;

            if (symbol is null)
            {
#if DEBUG
                var message = $"SymbolInfo not available for '{representative.ToString().QuoteIdentifierIfNeeded()}' on QualifiedNameSyntax.";
                RaiseImproveRuleDiagnostic(ctx, groupNode, message);
#endif
                continue;
            }

            foreach (var node in groupNode)
            {
                switch (representative.Left.Kind)
                {
                    // without namespace
                    case SyntaxKind.IdentifierName:
                        if (symbol.ContainingSymbol is not IObjectTypeSymbol objectTypeSymbol)
                            return;

                        if (symbol.ContainingSymbol.Kind == SymbolKind.TableExtension)
                        {
                            ITableExtensionTypeSymbol tableExtension = (ITableExtensionTypeSymbol)symbol.ContainingSymbol;
                            if (tableExtension.Target is not IObjectTypeSymbol tableExtensionTypeSymbol)
                            {
                                return;
                            }
                            objectTypeSymbol = tableExtensionTypeSymbol;
                        }

                        if (node.Left is IdentifierNameSyntax leftNode)
                        {
                            CompareIdentifier(ctx, leftNode.Identifier, objectTypeSymbol.Name);
                        }

                        if (node.Right is SimpleNameSyntax rightNode)
                        {
                            CompareIdentifier(ctx, rightNode.Identifier, symbol.Name);
                        }
                        break;

                    // with namespace
                    default:
                        CompareIdentifier(ctx, node.Right.Identifier, symbol.Name);
                        break;
                }
            }
        }
    }

    private void AnalyzeTriggerDeclaration(SymbolAnalysisContext ctx, SemanticModel semanticModel, List<SyntaxNode> nodes)
    {
        var syntaxNodes = nodes.OfType<TriggerDeclarationSyntax>();

        foreach (var node in syntaxNodes)
        {
            var symbol = semanticModel.GetDeclaredSymbol(node, ctx.CancellationToken);
            if (symbol is null)
                continue;

            CompareIdentifier(ctx, node.Name.Identifier, symbol.Name);
        }
    }

    #endregion

    #region Comparators

    private Lazy<ImmutableDictionary<string, string>>? GetOrdinalDictionary(string identifier)
    {
        propertyOrdinalDictionary.Value.TryGetValue(identifier, out var dictionary);
        return dictionary;
    }

    private Lazy<ImmutableDictionary<string, string>>? GetOrdinalDictionary(AnalyzeKind identifier)
    {
        analyzeKindOrdinalDictionary.Value.TryGetValue(identifier, out var dictionary);
        return dictionary;
    }

    private static readonly Lazy<ImmutableDictionary<string, string>> _accessibilityDictionary = CreateEnumDictionary<Accessibility>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _attributeKindDictionary = CreateEnumDictionary<AttributeKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _actionAreaKindDictionary = CreateEnumDictionary<ActionAreaKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _allowInCustomizationsKindDictionary = CreateEnumDictionary<AllowInCustomizationsKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _areaKindDictionary = CreateEnumDictionary<AreaKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _blankNumbersKindDictionary = CreateEnumDictionary<BlankNumbersKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _compressionTypeKindDictionary = CreateEnumDictionary<CompressionTypeKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _customActionTypeKindDictionary = CreateEnumDictionary<CustomActionTypeKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _cuegroupLayoutKindDictionary = CreateEnumDictionary<CuegroupLayoutKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _dataAccessIntentDictionary = CreateEnumDictionary(typeof(PageDataAccessIntentKind),
                                                                                                                         typeof(QueryDataAccessIntentKind),
                                                                                                                         typeof(ReportDataAccessIntentKind));
    private static readonly Lazy<ImmutableDictionary<string, string>> _dataClassificationKindDictionary = CreateEnumDictionary<DataClassificationKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _defaultLayoutKindDictionary = CreateEnumDictionary<DefaultLayoutKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _directionKindDictionary = CreateEnumDictionary<DirectionKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _encodingKindDictionary = CreateEnumDictionary<EncodingKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _entitlementRoleTypeKindDictionary = CreateEnumDictionary<EntitlementRoleTypeKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _eventSubscriberInstanceKindDictionary = CreateEnumDictionary<EventSubscriberInstanceKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _extendedDatatypeKindDictionary = CreateEnumDictionary<ExtendedDatatypeKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _externalAccessKindDictionary = CreateEnumDictionary<ExternalAccessKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _fieldClassKindDictionary = CreateEnumDictionary<FieldClassKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _fieldValidateKindDictionary = CreateEnumDictionary<FieldValidateKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _formatKindDictionary = CreateEnumDictionary<FormatKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _formatEvaluateKindDictionary = CreateEnumDictionary<FormatEvaluateKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _gestureKindDictionary = CreateEnumDictionary<GestureKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _gridLayoutKindDictionary = CreateEnumDictionary<GridLayoutKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _importanceKindDictionary = CreateEnumDictionary<ImportanceKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _labelPropertyDictionary = new(() =>
        LabelPropertyHelper.GetAllLabelProperties()
                            .ToImmutableDictionary(s => s, s => s, StringComparer.OrdinalIgnoreCase));
    private static readonly Lazy<ImmutableDictionary<string, string>> _maxOccursKindDictionary = CreateEnumDictionary<MaxOccursKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _minOccursKindDictionary = CreateEnumDictionary<MinOccursKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _multiplicityKindKindDictionary = CreateEnumDictionary<MultiplicityKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _navTypeKindDictionary = new(GenerateNavTypeKindDictionary);
    private static readonly Lazy<ImmutableDictionary<string, string>> _obsoleteStateKindDictionary = CreateEnumDictionary(typeof(FieldClassKind),
                                                                                                                          typeof(FieldObsoleteStateKind));
    private static readonly Lazy<ImmutableDictionary<string, string>> _occurrenceKindDictionary = CreateEnumDictionary<OccurrenceKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _pageTypeKindDictionary = CreateEnumDictionary<PageTypeKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _pdfFontEmbeddingKindDictionary = CreateEnumDictionary<PdfFontEmbeddingKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _readStateKindDictionary = CreateEnumDictionary<ReadStateKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _previewModeKindDictionary = CreateEnumDictionary<PreviewModeKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _promotedCategoryKindDictionary = CreateEnumDictionary<PromotedCategoryKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _propertyKindDictionary = CreateEnumDictionary<PropertyKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _runPageModeKindDictionary = CreateEnumDictionary<RunPageModeKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _queryColumnMethodKindDictionary = CreateEnumDictionary<QueryColumnMethodKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _queryTypeKindDictionary = CreateEnumDictionary<QueryTypeKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _scopeKindDictionary = CreateEnumDictionary(typeof(TableScopeKind),
                                                                                                                  typeof(PageActionScopeKind)); //ControlKind
    private static readonly Lazy<ImmutableDictionary<string, string>> _showAsKindDictionary = CreateEnumDictionary<ShowAsKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _sqlDataTypeKindDictionary = CreateEnumDictionary<SqlDataTypeKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _sqlJoinTypeKindDictionary = CreateEnumDictionary<SqlJoinTypeKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _styleKindDictionary = CreateEnumDictionary<StyleKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _subtypeKindDictionary = CreateEnumDictionary(typeof(CodeunitSubtypeKind),
                                                                                                                    typeof(FieldSubtypeKind));
    private static readonly Lazy<ImmutableDictionary<string, string>> _symbolKindDictionary = new(GenerateSymbolKindDictionary);
    private static readonly Lazy<ImmutableDictionary<string, string>> _tableTypeKindDictionary = CreateEnumDictionary<TableTypeKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _testIsolationKindDictionary = CreateEnumDictionary<TestIsolationKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _testPermissionsKindDictionary = CreateEnumDictionary<TestPermissionsKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _textEncodingKindDictionary = CreateEnumDictionary<TextEncodingKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _textTypeKindictionary = CreateEnumDictionary<TextTypeKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _typeKindDictionary = CreateEnumDictionary(typeof(TypeKind),
                                                                                                                 typeof(EntitlementTypeKind));
    private static readonly Lazy<ImmutableDictionary<string, string>> _transactionTypeKindDictionary = CreateEnumDictionary<TransactionTypeKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _treeInitialStateKindDictionary = CreateEnumDictionary<TreeInitialStateKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _updatePropagationKindDictionary = CreateEnumDictionary<UpdatePropagationKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _usageCategoryKindDictionary = CreateEnumDictionary<UsageCategoryKind>();
    private static readonly Lazy<ImmutableDictionary<string, string>> _xmlVersionNoKindDictionary = CreateEnumDictionary<XmlVersionNoKind>();
    private static readonly Lazy<Dictionary<string, Lazy<ImmutableDictionary<string, string>>>> propertyOrdinalDictionary = new(() =>
        new Dictionary<string, Lazy<ImmutableDictionary<string, string>>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Access",                 _accessibilityDictionary },
            { "AllowInCustomizations",  _allowInCustomizationsKindDictionary },
            { "BlankNumbers",           _blankNumbersKindDictionary },
            { "Caption",                _labelPropertyDictionary },
            { "CompressionType",        _compressionTypeKindDictionary },
            { "CustomActionType",       _customActionTypeKindDictionary },
            { "CueGroupLayout",         _cuegroupLayoutKindDictionary },
            { "DataAccessIntent",       _dataAccessIntentDictionary },
            { "DataClassification",     _dataClassificationKindDictionary },
            { "DefaultLayout",          _defaultLayoutKindDictionary },
            { "Direction",              _directionKindDictionary },
            { "Encoding",               _encodingKindDictionary },
            { "EventSubscriberInstance",_eventSubscriberInstanceKindDictionary },
            { "ExtendedDatatype",       _extendedDatatypeKindDictionary },
            { "ExternalAccess",         _externalAccessKindDictionary },
            { "FieldClass",             _fieldClassKindDictionary },
            { "FieldValidate",          _fieldValidateKindDictionary },
            { "Format",                 _formatKindDictionary },
            { "FormatEvaluate",         _formatEvaluateKindDictionary },
            { "Gesture",                _gestureKindDictionary },
            { "GridLayout",             _gridLayoutKindDictionary },
            { "Importance",             _importanceKindDictionary },
            { "MaxOccurs",              _maxOccursKindDictionary },
            { "Method",                 _queryColumnMethodKindDictionary },
            { "MinOccurs",              _minOccursKindDictionary },
            { "Multiplicity",           _multiplicityKindKindDictionary },
            { "ObsoleteState",          _obsoleteStateKindDictionary },
            { "Occurrence",             _occurrenceKindDictionary },
            { "QueryType",              _queryTypeKindDictionary },
            { "Scope",                  _scopeKindDictionary },
            { "ShowAs",                 _showAsKindDictionary },
            { "SqlDataType",            _sqlDataTypeKindDictionary },
            { "SqlJoinType",            _sqlJoinTypeKindDictionary },
            { "PageType",               _pageTypeKindDictionary },
            { "PdfFontEmbedding",       _pdfFontEmbeddingKindDictionary },
            { "PreviewMode",            _previewModeKindDictionary },
            { "PromotedCategory",       _promotedCategoryKindDictionary },
            { "ReadState",              _readStateKindDictionary },
            { "RoleType",               _entitlementRoleTypeKindDictionary },
            { "RunPageMode",            _runPageModeKindDictionary },
            { "Style",                  _styleKindDictionary },
            { "Subtype",                _subtypeKindDictionary },
            { "TableType",              _tableTypeKindDictionary} ,
            { "TestIsolation",          _testIsolationKindDictionary },
            { "TestPermissions",        _testPermissionsKindDictionary },
            { "TextEncoding",           _textEncodingKindDictionary },
            { "TextType",               _textTypeKindictionary },
            { "Type",                   _typeKindDictionary },
            { "TransactionType",        _transactionTypeKindDictionary },
            { "TreeInitialState",       _treeInitialStateKindDictionary },
            { "UpdatePropagation",      _updatePropagationKindDictionary },
            { "UsageCategory",          _usageCategoryKindDictionary },
            { "XmlVersionNo",           _xmlVersionNoKindDictionary }
        });

    private static readonly Lazy<Dictionary<AnalyzeKind, Lazy<ImmutableDictionary<string, string>>>> analyzeKindOrdinalDictionary = new(() =>
        new Dictionary<AnalyzeKind, Lazy<ImmutableDictionary<string, string>>>()
        {
            { AnalyzeKind.Attribute,                _attributeKindDictionary },
            { AnalyzeKind.Area,                     _areaKindDictionary },
            { AnalyzeKind.ActionArea,               _actionAreaKindDictionary },
            { AnalyzeKind.Property,                 _propertyKindDictionary },
            { AnalyzeKind.PropertyName,             _propertyKindDictionary },
            { AnalyzeKind.IdentifierEqualsLiteral,  _labelPropertyDictionary }
        });

    private void CompareIdentifier(SymbolAnalysisContext ctx, SyntaxToken identifier, string? canonical)
    {
        string? tokenText = identifier.ValueText?.UnquoteIdentifier();
        if (string.IsNullOrEmpty(tokenText))
            return;

        if (string.IsNullOrEmpty(canonical))
            return;

        CompareIdentifier(ctx, identifier, tokenText, canonical);
    }

    private void CompareIdentifier(
        SymbolAnalysisContext ctx,
        string identifierValueText,
        IEnumerable<SyntaxToken> identifiers,
        string canonical)
    {
        // Use spans to increase performance to compare the token text with the canonical value.
        ReadOnlySpan<char> tokenSpan = identifierValueText.AsSpan();
        ReadOnlySpan<char> canonicalSpan = canonical.AsSpan();

        if (!tokenSpan.Equals(canonicalSpan, StringComparison.Ordinal))
        {
            foreach (var identifier in identifiers)
            {
                var location = identifier.GetLocation();
                if (location is null)
                    return;

                ctx.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.Rule0005CasingMismatch,
                        location,
                        canonical));
            }
        }
    }

    private void CompareAgainstDictionary(
        SymbolAnalysisContext ctx,
        SyntaxToken identifier,
        Lazy<ImmutableDictionary<string, string>>? lookupDictionary)
    {
        string? tokenText = identifier.ValueText?.UnquoteIdentifier();
        if (string.IsNullOrEmpty(tokenText))
            return;

        var lookupDict = lookupDictionary?.Value;

        if (lookupDict is null)
        {
            var message = $"missing ordinals for '{tokenText}'.";
            RaiseImproveRuleDiagnostic(ctx, identifier, message);
            return;
        }

        if (!lookupDict.TryGetValue(tokenText, out string? canonical))
        {
            var message = $"redundant analysis of '{tokenText}'.";
            RaiseImproveRuleDiagnostic(ctx, identifier, message);
            return;
        }

        CompareIdentifier(ctx, identifier, tokenText, canonical);
    }

    private void CompareIdentifier(SymbolAnalysisContext ctx, SyntaxToken identifier, string token, string canonical)
    {
        // Use spans to increase performance to compare the token text with the canonical value.
        ReadOnlySpan<char> tokenSpan = token.AsSpan();
        ReadOnlySpan<char> canonicalSpan = canonical.AsSpan();

        if (!tokenSpan.Equals(canonicalSpan, StringComparison.Ordinal))
        {
            var location = identifier.GetLocation();
            if (location is null)
                return;

            ctx.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.Rule0005CasingMismatch,
                    location,
                    canonical));
        }
    }

    #endregion

    private void RaiseImproveRuleDiagnostic(SymbolAnalysisContext ctx, IEnumerable<SyntaxNode> nodes, string message)
    {
        foreach (var node in nodes)
            RaiseImproveRuleDiagnostic(ctx, node.GetLocation(), message);
    }

    private void RaiseImproveRuleDiagnostic(SymbolAnalysisContext ctx, IEnumerable<SyntaxToken> tokens, string message)
    {
        foreach (var token in tokens)
            RaiseImproveRuleDiagnostic(ctx, token.GetLocation(), message);
    }

    private void RaiseImproveRuleDiagnostic(SymbolAnalysisContext ctx, SyntaxNode node, string message)
    {
        RaiseImproveRuleDiagnostic(ctx, node.GetLocation(), message);
    }

    private void RaiseImproveRuleDiagnostic(SymbolAnalysisContext ctx, SyntaxToken token, string message)
    {
        RaiseImproveRuleDiagnostic(ctx, token.GetLocation(), message);
    }

    private void RaiseImproveRuleDiagnostic(SymbolAnalysisContext ctx, Location location, string message)
    {
        ctx.ReportDiagnostic(
            Diagnostic.Create(
                DiagnosticDescriptors.Rule0000ErrorInRule,
                location,
                "LC0005",
                message,
                "Help improving this rule and open a GitHub issue, pretty please? :-)"));
    }

    private enum AnalyzeKind
    {
        DataType,
        IdentifierEqualsLiteral,
        IdentifierName,
        Attribute,
        OptionAccessExpression,
        Area,
        ActionArea,
        Property,
        PropertyName,
        QualifiedName,
        TriggerDeclaration
    }


    #region Dictionary Builders

    private static Lazy<ImmutableDictionary<string, string>> CreateEnumDictionary<TEnum>() where TEnum : struct, Enum
    {
        return new Lazy<ImmutableDictionary<string, string>>(() =>
            Enum.GetNames(typeof(TEnum))
                .ToImmutableDictionary(s => s, s => s, StringComparer.OrdinalIgnoreCase));
    }

    private static Lazy<ImmutableDictionary<string, string>> CreateEnumDictionary(params Type[] enumTypes)
    {
        return new Lazy<ImmutableDictionary<string, string>>(() =>
            enumTypes
                .SelectMany(type => Enum.GetNames(type))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToImmutableDictionary(s => s, s => s, StringComparer.OrdinalIgnoreCase));
    }

    private static ImmutableDictionary<string, string> GenerateNavTypeKindDictionary()
    {
        var builder = ImmutableDictionary.CreateBuilder<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var kind in Enum.GetNames(typeof(NavTypeKind)))
        {
            builder[kind] = kind;
        }

        // Add additional entry for Database::"G/L Entry" (there is no NavTypeKind for this)
        builder["Database"] = "Database";

        return builder.ToImmutable();
    }

    private static ImmutableDictionary<string, string> GenerateSymbolKindDictionary()
    {
        var builder = ImmutableDictionary.CreateBuilder<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var kind in Enum.GetNames(typeof(SymbolKind)))
        {
            // Change "XmlPort" to "Xmlport"
            var key = kind == "XmlPort" ? "Xmlport" : kind;
            builder[key] = key;
        }

        // Add additional entry for Database::"G/L Entry" (there is no NavTypeKind for this)
        builder["Database"] = "Database";

        // Add additional entry for ObjectType::Table (there is no NavTypeKind for this)
        builder["ObjectType"] = "ObjectType";

        return builder.ToImmutable();
    }

    #endregion
}
using System.Collections.Immutable;
using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0068CheckObjectPermission : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0068CheckObjectPermission);

        private static readonly List<string> buildInTableDataReadMethodNames = new List<string>
        {
            "find",
            "findfirst",
            "findlast",
            "findset",
            "get",
            "isempty"
        };
        private static readonly List<string> buildInTableDataModifyMethodNames = new List<string>
        {
            "modify",
            "modifyall",
            "rename"
        };
        private static readonly List<string> buildInTableDataInsertMethodNames = new List<string>
        {
            "insert"
        };
        private static readonly List<string> buildInTableDataDeleteMethodNames = new List<string>
        {
            "delete",
            "deleteall"
        };
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckObjectPermission), OperationKind.InvocationExpression);
            context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckReportDataItemObjectPermission), SymbolKind.ReportDataItem);
            context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckQueryDataItemObjectPermission), SymbolKind.QueryDataItem);
            context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckXmlportNodeObjectPermission), SymbolKind.XmlPortNode);
        }

        private void CheckXmlportNodeObjectPermission(SymbolAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;
            if (((IXmlPortNodeSymbol)ctx.Symbol.OriginalDefinition).SourceTypeKind != XmlPortSourceTypeKind.Table) return;

            string direction = "";

            IXmlPortTypeSymbol xmlPort = (IXmlPortTypeSymbol)ctx.Symbol.GetContainingObjectTypeSymbol();

            IPropertySymbol? objectPermissions = xmlPort.GetProperty(PropertyKind.Permissions);
            ITypeSymbol targetSymbol = ((IXmlPortNodeSymbol)ctx.Symbol.OriginalDefinition).GetTypeSymbol();
            var directionProperty = xmlPort.Properties.FirstOrDefault(property => property.Name == "Direction");

            if (directionProperty == null)
                direction = DirectionKind.Both.ToString();
            else
                direction = directionProperty.ValueText;

            bool? AutoReplace = (bool?)ctx.Symbol.Properties.FirstOrDefault(property => property.PropertyKind == PropertyKind.AutoReplace)?.Value; // modify permissions
            bool? AutoUpdate = (bool?)ctx.Symbol.Properties.FirstOrDefault(property => property.PropertyKind == PropertyKind.AutoUpdate)?.Value; // modify permissions
            bool? AutoSave = (bool?)ctx.Symbol.Properties.FirstOrDefault(property => property.PropertyKind == PropertyKind.AutoSave)?.Value; // insert permissions

            AutoReplace ??= true;
            AutoUpdate ??= true;
            AutoSave ??= true;

            direction = direction.ToLowerInvariant();

            if (direction == "import" || direction == "both")
            {
                if (AutoReplace == true || AutoUpdate == true)
                    CheckProcedureInvocation(objectPermissions, targetSymbol, 'm', ctx.ReportDiagnostic, ctx.Symbol.GetLocation(), (ITableTypeSymbol)targetSymbol.OriginalDefinition);
                if (AutoSave == true)
                    CheckProcedureInvocation(objectPermissions, targetSymbol, 'i', ctx.ReportDiagnostic, ctx.Symbol.GetLocation(), (ITableTypeSymbol)targetSymbol.OriginalDefinition);
            }
            if (direction == "export" || direction == "both")
                CheckProcedureInvocation(objectPermissions, targetSymbol, 'r', ctx.ReportDiagnostic, ctx.Symbol.GetLocation(), (ITableTypeSymbol)targetSymbol.OriginalDefinition);
        }

        private void CheckQueryDataItemObjectPermission(SymbolAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            IPropertySymbol? objectPermissions = ctx.Symbol.GetContainingApplicationObjectTypeSymbol()?.GetProperty(PropertyKind.Permissions);
            ITypeSymbol targetSymbol = ((IQueryDataItemSymbol)ctx.Symbol).GetTypeSymbol();
            CheckProcedureInvocation(objectPermissions, targetSymbol, 'r', ctx.ReportDiagnostic, ctx.Symbol.GetLocation(), (ITableTypeSymbol)targetSymbol.OriginalDefinition);
        }

        private void CheckReportDataItemObjectPermission(SymbolAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;
            if (ctx.Symbol.GetBooleanPropertyValue(PropertyKind.UseTemporary) == true) return;
            if (((ITableTypeSymbol)((IRecordTypeSymbol)((IReportDataItemSymbol)ctx.Symbol).GetTypeSymbol()).OriginalDefinition).TableType == TableTypeKind.Temporary) return;

            IPropertySymbol? objectPermissions = ctx.Symbol.GetContainingApplicationObjectTypeSymbol()?.GetProperty(PropertyKind.Permissions);
            ITypeSymbol targetSymbol = ((IReportDataItemSymbol)ctx.Symbol).GetTypeSymbol();
            CheckProcedureInvocation(objectPermissions, targetSymbol, 'r', ctx.ReportDiagnostic, ctx.Symbol.GetLocation(), (ITableTypeSymbol)targetSymbol.OriginalDefinition);
        }

        private void CheckObjectPermission(OperationAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
            if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod) return;

            ITypeSymbol? variableType = operation.Instance?.GetSymbol()?.GetTypeSymbol();
            if (variableType?.GetNavTypeKindSafe() != NavTypeKind.Record) return;

            ITableTypeSymbol targetTable = (ITableTypeSymbol)((IRecordTypeSymbol)variableType).OriginalDefinition;

            if (ctx.ContainingSymbol.GetContainingApplicationObjectTypeSymbol()?.NavTypeKind == NavTypeKind.Page)
            {
                IPropertySymbol? sourceTableProperty = ctx.ContainingSymbol.GetContainingApplicationObjectTypeSymbol()?.GetProperty(PropertyKind.SourceTable);
                if (sourceTableProperty != null)
                    if (sourceTableProperty.Value == targetTable)
                        return;
            }

            if (variableType.ToString().ToLower().EndsWith("temporary") || (targetTable.TableType == TableTypeKind.Temporary)) return;

            IEnumerable<IAttributeSymbol> inherentPermissions = [];

            if (ctx.ContainingSymbol is IMethodSymbol symbol)
                inherentPermissions = symbol.Attributes.Where(attribute => attribute.Name == "InherentPermissions");

            IPropertySymbol? objectPermissions = ctx.ContainingSymbol.GetContainingApplicationObjectTypeSymbol()?.GetProperty(PropertyKind.Permissions);
            //variableType.OriginalDefinition.ContainingNamespace
            if (buildInTableDataReadMethodNames.Contains(operation.TargetMethod.Name.ToLowerInvariant()))
            {
                if (!ProcedureHasInherentPermission(inherentPermissions, variableType, 'r'))
                    CheckProcedureInvocation(objectPermissions, variableType, 'r', ctx.ReportDiagnostic, ctx.Operation.Syntax.GetLocation(), targetTable);
            }
            else if (buildInTableDataInsertMethodNames.Contains(operation.TargetMethod.Name.ToLowerInvariant()))
            {
                if (!ProcedureHasInherentPermission(inherentPermissions, variableType, 'i'))
                    CheckProcedureInvocation(objectPermissions, variableType, 'i', ctx.ReportDiagnostic, ctx.Operation.Syntax.GetLocation(), targetTable);
            }
            else if (buildInTableDataModifyMethodNames.Contains(operation.TargetMethod.Name.ToLowerInvariant()))
            {
                if (!ProcedureHasInherentPermission(inherentPermissions, variableType, 'm'))
                    CheckProcedureInvocation(objectPermissions, variableType, 'm', ctx.ReportDiagnostic, ctx.Operation.Syntax.GetLocation(), targetTable);
            }
            else if (buildInTableDataDeleteMethodNames.Contains(operation.TargetMethod.Name.ToLowerInvariant()))
            {
                if (!ProcedureHasInherentPermission(inherentPermissions, variableType, 'd'))
                    CheckProcedureInvocation(objectPermissions, variableType, 'd', ctx.ReportDiagnostic, ctx.Operation.Syntax.GetLocation(), targetTable);
            }
        }

        private bool ProcedureHasInherentPermission(IEnumerable<IAttributeSymbol> inherentPermissions, ITypeSymbol variableType, char requestedPermission)
        {
            //[InherentPermissions(PermissionObjectType::TableData, Database::"SomeTable", 'r'),InherentPermissions(PermissionObjectType::TableData, Database::"SomeOtherTable", 'w')]

            if (inherentPermissions == null || inherentPermissions.Count() == 0) return false;

            foreach (var inherentPermission in inherentPermissions)
            {
                var inherentPermissionAsString = inherentPermission.DeclaringSyntaxReference?.GetSyntax().ToString();



                var permissions = inherentPermissionAsString?.Split(new[] { '[', ']', '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (permissions?[1].Trim() != "PermissionObjectType::TableData") continue;

                var typeAndObjectName = permissions[2].Trim();
                var permissionValue = permissions[3].Trim().Trim(new[] { '\'', ' ' }).ToLowerInvariant();

                var typeParts = typeAndObjectName.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                if (typeParts.Length < 2) continue;

                var objectName = typeParts[1].Trim().Trim('"');
                if (objectName.ToLowerInvariant() != variableType.Name.ToLowerInvariant())
#if !Fall2023OrLower
                    if (objectName.UnquoteIdentifier().ToLowerInvariant() != (variableType.OriginalDefinition.ContainingNamespace?.QualifiedName.ToLowerInvariant() + "." + variableType.Name.ToLowerInvariant()))
#endif
                        continue;

                if (permissionValue.Contains(requestedPermission.ToString().ToLowerInvariant()[0]))
                {
                    return true;
                }
            }
            return false;
        }

        private void CheckProcedureInvocation(IPropertySymbol? objectPermissions, ITypeSymbol variableType, char requestedPermission, Action<Diagnostic> ReportDiagnostic, Microsoft.Dynamics.Nav.CodeAnalysis.Text.Location location, ITableTypeSymbol targetTable)
        {
            if (targetTable.Id > 2000000000) return;
            if (TableHasInherentPermission(targetTable, requestedPermission)) return;

            if (objectPermissions == null)
            {
                ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0068CheckObjectPermission, location, requestedPermission, variableType.Name));
                return;
            }

            bool permissionContainRequestedObject = false;

            foreach (var permission in objectPermissions.Value.ToString().ToLowerInvariant().Split(','))
            {
                var parts = permission.Trim().Split(new[] { '=' }, 2);
                if (parts.Length != 2)
                {
                    // Handle invalid permission format
                    continue;
                }

                var typeAndObjectName = parts[0].Trim();
                var permissionValue = parts[1].Trim();

                // Extract type and object name, handling quoted object names
                var typeEndIndex = typeAndObjectName.IndexOf(' ');
                if (typeEndIndex == -1)
                {
                    // Handle invalid type/object name format
                    continue;
                }

                var type = typeAndObjectName[..typeEndIndex].Trim();
                var objectName = typeAndObjectName[typeEndIndex..].Trim().Trim('"');

                bool nameSpaceNameMatch = false;
#if !Fall2023OrLower
                nameSpaceNameMatch = objectName.UnquoteIdentifier() == (variableType.OriginalDefinition.ContainingNamespace?.QualifiedName.ToLowerInvariant() + "." + variableType.Name.ToLowerInvariant());
#endif

                // Match against the parameters of the procedure
                if (type == "tabledata" && (objectName == variableType.Name.ToLowerInvariant() || nameSpaceNameMatch))
                {
                    permissionContainRequestedObject = true;
                    // Handle tabledata permissions
                    var permissions = permissionValue.ToCharArray();
                    if (!permissions.Contains(requestedPermission))
                    {
                        ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0068CheckObjectPermission, location, requestedPermission, variableType.Name));
                    }
                }
            }
            if (!permissionContainRequestedObject)
            {
                ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0068CheckObjectPermission, location, requestedPermission, variableType.Name));
            }
        }

        private bool TableHasInherentPermission(ITableTypeSymbol table, char requestedPermission)
        {
            IPropertySymbol? permissionProperty = table.GetProperty(PropertyKind.InherentPermissions);
            // InherentPermissions = RIMD;
            char[]? permissions = permissionProperty?.Value.ToString().ToLowerInvariant().Split(new[] { '=' }, 2)[0].Trim().ToCharArray();

            if (permissions is not null && permissions.Contains(requestedPermission.ToString().ToLowerInvariant()[0]))
                return true;

            return false;
        }

        public static class DiagnosticDescriptors
        {
            public static readonly DiagnosticDescriptor Rule0068CheckObjectPermission = new(
            id: LinterCopAnalyzers.AnalyzerPrefix + "0068",
            title: LinterCopAnalyzers.GetLocalizableString("Rule0068CheckObjectPermissionTitle"),
            messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0068CheckObjectPermissionFormat"),
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Info, isEnabledByDefault: true,
            description: LinterCopAnalyzers.GetLocalizableString("Rule0068CheckObjectPermissionDescription"),
            helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0068");
        }
    }
}
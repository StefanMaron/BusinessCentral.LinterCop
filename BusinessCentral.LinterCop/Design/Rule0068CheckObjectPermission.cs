using System.Collections.Immutable;
using System.Dynamic;
using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.Analyzers.Common;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;

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

            IPropertySymbol objectPermissions = xmlPort.GetProperty(PropertyKind.Permissions);
            ITypeSymbol targetSymbol = ((IXmlPortNodeSymbol)ctx.Symbol.OriginalDefinition).GetTypeSymbol();
            var directionProperty = xmlPort.Properties.FirstOrDefault(property => property.Name == "Direction");

            if (directionProperty == null)
                direction = DirectionKind.Both.ToString();
            else
                direction = directionProperty.ValueText;

            direction = direction.ToLowerInvariant();

            if (direction == "import" || direction == "both")
            {
                CheckProcedureInvocation(objectPermissions, targetSymbol.Name, 'm', ctx.ReportDiagnostic, ctx.Symbol.GetLocation(), (ITableTypeSymbol)targetSymbol.OriginalDefinition);
                CheckProcedureInvocation(objectPermissions, targetSymbol.Name, 'i', ctx.ReportDiagnostic, ctx.Symbol.GetLocation(), (ITableTypeSymbol)targetSymbol.OriginalDefinition);
            }
            if (direction == "export" || direction == "both")
                CheckProcedureInvocation(objectPermissions, targetSymbol.Name, 'r', ctx.ReportDiagnostic, ctx.Symbol.GetLocation(), (ITableTypeSymbol)targetSymbol.OriginalDefinition);
        }

        private void CheckQueryDataItemObjectPermission(SymbolAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            IPropertySymbol objectPermissions = ctx.Symbol.GetContainingApplicationObjectTypeSymbol().GetProperty(PropertyKind.Permissions);
            ITypeSymbol targetSymbol = ((IQueryDataItemSymbol)ctx.Symbol).GetTypeSymbol();
            CheckProcedureInvocation(objectPermissions, targetSymbol.Name, 'r', ctx.ReportDiagnostic, ctx.Symbol.GetLocation(), (ITableTypeSymbol)targetSymbol.OriginalDefinition);
        }

        private void CheckReportDataItemObjectPermission(SymbolAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;
            if (ctx.Symbol.GetBooleanPropertyValue(PropertyKind.UseTemporary) == true) return;
            if (((ITableTypeSymbol)((IRecordTypeSymbol)((IReportDataItemSymbol)ctx.Symbol).GetTypeSymbol()).OriginalDefinition).TableType == TableTypeKind.Temporary) return;

            IPropertySymbol objectPermissions = ctx.Symbol.GetContainingApplicationObjectTypeSymbol().GetProperty(PropertyKind.Permissions);
            ITypeSymbol targetSymbol = ((IReportDataItemSymbol)ctx.Symbol).GetTypeSymbol();
            CheckProcedureInvocation(objectPermissions, targetSymbol.Name, 'r', ctx.ReportDiagnostic, ctx.Symbol.GetLocation(), (ITableTypeSymbol)targetSymbol.OriginalDefinition);
        }

        private void CheckObjectPermission(OperationAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
            if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod) return;

            ITypeSymbol variableType = operation.Instance?.GetSymbol().GetTypeSymbol();
            if (variableType.GetNavTypeKindSafe() != NavTypeKind.Record) return;

            ITableTypeSymbol targetTable = (ITableTypeSymbol)((IRecordTypeSymbol)variableType).OriginalDefinition;

            if (ctx.ContainingSymbol.GetContainingApplicationObjectTypeSymbol().NavTypeKind == NavTypeKind.Page)
            {
                IPropertySymbol sourceTableProperty = ctx.ContainingSymbol.GetContainingApplicationObjectTypeSymbol().GetProperty(PropertyKind.SourceTable);
                if (sourceTableProperty != null)
                    if (sourceTableProperty.Value == targetTable)
                        return;
            }

            if (variableType.ToString().ToLower().EndsWith("temporary") || (targetTable.TableType == TableTypeKind.Temporary)) return;

            IEnumerable<IAttributeSymbol> inherentPermissions = [];

            if (ctx.ContainingSymbol is IMethodSymbol symbol)            
               inherentPermissions = symbol.Attributes.Where(attribute => attribute.Name == "InherentPermissions");            

            IPropertySymbol objectPermissions = ctx.ContainingSymbol.GetContainingApplicationObjectTypeSymbol().GetProperty(PropertyKind.Permissions);

            if (buildInTableDataReadMethodNames.Contains(operation.TargetMethod.Name.ToLowerInvariant()))
            {
                if (!ProcedureHasInherentPermission(inherentPermissions, variableType.Name, 'r'))
                    CheckProcedureInvocation(objectPermissions, variableType.Name, 'r', ctx.ReportDiagnostic, ctx.Operation.Syntax.GetLocation(), targetTable);
            }
            else if (buildInTableDataInsertMethodNames.Contains(operation.TargetMethod.Name.ToLowerInvariant()))
            {
                if (!ProcedureHasInherentPermission(inherentPermissions, variableType.Name, 'i'))
                    CheckProcedureInvocation(objectPermissions, variableType.Name, 'i', ctx.ReportDiagnostic, ctx.Operation.Syntax.GetLocation(), targetTable);
            }
            else if (buildInTableDataModifyMethodNames.Contains(operation.TargetMethod.Name.ToLowerInvariant()))
            {
                if (!ProcedureHasInherentPermission(inherentPermissions, variableType.Name, 'm'))
                    CheckProcedureInvocation(objectPermissions, variableType.Name, 'm', ctx.ReportDiagnostic, ctx.Operation.Syntax.GetLocation(), targetTable);
            }
            else if (buildInTableDataDeleteMethodNames.Contains(operation.TargetMethod.Name.ToLowerInvariant()))
            {
                if (!ProcedureHasInherentPermission(inherentPermissions, variableType.Name, 'd'))
                    CheckProcedureInvocation(objectPermissions, variableType.Name, 'd', ctx.ReportDiagnostic, ctx.Operation.Syntax.GetLocation(), targetTable);
            }
        }

        private bool ProcedureHasInherentPermission(IEnumerable<IAttributeSymbol> inherentPermissions, string variableTypeName, char requestedPermission)
        {
            //[InherentPermissions(PermissionObjectType::TableData, Database::"SomeTable", 'r'),InherentPermissions(PermissionObjectType::TableData, Database::"SomeOtherTable", 'w')]

            if (inherentPermissions == null || inherentPermissions.Count() == 0) return false;

            foreach (var inherentPermission in inherentPermissions)
            {
                var inherentPermissionAsString = inherentPermission.DeclaringSyntaxReference.GetSyntax().ToString();



                var permissions = inherentPermissionAsString.Split(new[] { '[', ']', '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (permissions[1].Trim() != "PermissionObjectType::TableData") continue;

                var typeAndObjectName = permissions[2].Trim();
                var permissionValue = permissions[3].Trim().Trim(new[] { '\'', ' ' });

                var typeParts = typeAndObjectName.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                if (typeParts.Length < 2) continue;

                var objectName = typeParts[1].Trim().Trim('"');
                if (objectName.ToLowerInvariant() != variableTypeName.ToLowerInvariant()) continue;

                if (permissionValue.Contains(requestedPermission))
                {
                    return true;
                }
            }
            return false;
        }

        private void CheckProcedureInvocation(IPropertySymbol objectPermissions, string variableTypeName, char requestedPermission, Action<Diagnostic> ReportDiagnostic, Microsoft.Dynamics.Nav.CodeAnalysis.Text.Location location, ITableTypeSymbol targetTable)
        {
            if (TableHasInherentPermission(targetTable, requestedPermission)) return;

            if (objectPermissions == null)
            {
                ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0068CheckObjectPermission, location, requestedPermission, variableTypeName));
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

                // Match against the parameters of the procedure
                if (type == "tabledata" && objectName == variableTypeName.ToLowerInvariant())
                {
                    permissionContainRequestedObject = true;
                    // Handle tabledata permissions
                    var permissions = permissionValue.ToCharArray();
                    if (!permissions.Contains(requestedPermission))
                    {
                        ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0068CheckObjectPermission, location, requestedPermission, variableTypeName));
                    }
                }
            }
            if (!permissionContainRequestedObject)
            {
                ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0068CheckObjectPermission, location, requestedPermission, variableTypeName));
            }
        }

        private bool TableHasInherentPermission(ITableTypeSymbol table, char requestedPermission)
        {
            IPropertySymbol permissionProperty = table.GetProperty(PropertyKind.InherentPermissions);
            if (permissionProperty == null) return false;
            // InherentPermissions = RIMD;
            var permissions = permissionProperty.Value.ToString().ToLowerInvariant().Split(new[] { '=' }, 2)[0].Trim().ToCharArray();

            if (permissions.Contains(requestedPermission.ToString().ToLowerInvariant()[0]))
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
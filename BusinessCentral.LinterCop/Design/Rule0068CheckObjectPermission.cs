using System.Collections.Immutable;
using BusinessCentral.LinterCop.AnalysisContextExtension;
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
            "Find",
            "FindFirst",
            "FindLast",
            "FindSet",
            "Get",
            "IsEmpty"
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
        private static readonly List<NavTypeKind> allowedNavTypeKinds = new List<NavTypeKind>
        {
            NavTypeKind.Table,
            NavTypeKind.Page,
            NavTypeKind.Report,
            NavTypeKind.XmlPort,
            NavTypeKind.Codeunit,
            NavTypeKind.Query
        };
        public override void Initialize(AnalysisContext context) => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckObjectPermission), OperationKind.InvocationExpression);

        private void CheckObjectPermission(OperationAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
            if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod) return;

            ITypeSymbol variableType = operation.Instance?.GetSymbol().GetTypeSymbol();
            // if (!allowedNavTypeKinds.Contains(ctx.ContainingSymbol.GetContainingApplicationObjectTypeSymbol().GetNavTypeKindSafe())) return;
            IPropertySymbol objectPermissions = ctx.ContainingSymbol.GetContainingApplicationObjectTypeSymbol().GetProperty(PropertyKind.Permissions);

            if (variableType.GetNavTypeKindSafe() != NavTypeKind.Record) return;

            if (buildInTableDataReadMethodNames.Contains(operation.TargetMethod.Name.ToLowerInvariant()))
                CheckProcedureInvocation(objectPermissions, variableType, 'r',  ctx);
            else if (buildInTableDataInsertMethodNames.Contains(operation.TargetMethod.Name.ToLowerInvariant()))
                CheckProcedureInvocation(objectPermissions, variableType, 'i', ctx);
            else if (buildInTableDataModifyMethodNames.Contains(operation.TargetMethod.Name.ToLowerInvariant()))
                CheckProcedureInvocation(objectPermissions, variableType, 'm', ctx);
            else if (buildInTableDataDeleteMethodNames.Contains(operation.TargetMethod.Name.ToLowerInvariant()))
                CheckProcedureInvocation(objectPermissions, variableType, 'd', ctx);

        }

        private void CheckProcedureInvocation(IPropertySymbol objectPermissions, ITypeSymbol variableType, char requestedPermission, OperationAnalysisContext ctx)
        {
            if (objectPermissions == null) 
            {
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0068CheckObjectPermission, ctx.Operation.Syntax.GetLocation(), requestedPermission, variableType.Name));
                return;
            }

            foreach (var permission in objectPermissions.Value.ToString().Split(','))
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
                if (type == "tabledata" && objectName == variableType.Name)
                {
                    // Handle tabledata permissions
                    var permissions = permissionValue.ToCharArray();
                    if (!permissions.Contains(requestedPermission))
                    {
                        ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0068CheckObjectPermission, ctx.Operation.Syntax.GetLocation(), requestedPermission, variableType.Name));
                    }
                }
            }
        }

        public static class DiagnosticDescriptors
        {
                public static readonly DiagnosticDescriptor Rule0068CheckObjectPermission = new(
                id: LinterCopAnalyzers.AnalyzerPrefix + "0068",
                title: LinterCopAnalyzers.GetLocalizableString("Rule0068CheckObjectPermissionTitle"),
                messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0068CheckObjectPermissionFormat"),
                category: "Design",
                defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true,
                description:  LinterCopAnalyzers.GetLocalizableString("Rule0068CheckObjectPermissionDescription"),
                helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0068");
        }
    }
}
using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0092ValidateReportLayoutFiles : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0092ValidateReportLayoutFiles);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterCompilationStartAction(compilationStartContext =>
        {
            string? directoryPath = compilationStartContext.Compilation.FileSystem?.GetDirectoryPath();
            // For testing purposes, use a default path if none available
            if (string.IsNullOrEmpty(directoryPath))
                directoryPath = ".";

            compilationStartContext.RegisterSyntaxNodeAction(
                syntaxContext => AnalyzeReportLayoutFiles(syntaxContext, directoryPath),
                SyntaxKind.ReportObject,
                SyntaxKind.ReportExtensionObject);
        });
    }

    private static void AnalyzeReportLayoutFiles(SyntaxNodeAnalysisContext context, string projectRoot)
    {
        if (context.IsObsoletePendingOrRemoved())
            return;

        var reportNode = context.Node;

        // Check modern rendering section layouts
        ValidateRenderingLayouts(context, reportNode, projectRoot);

        // Check legacy layout properties (only one can exist at a time)
        ValidateLegacyLayoutProperties(context, reportNode, projectRoot);
    }

    private static void ValidateRenderingLayouts(SyntaxNodeAnalysisContext context, SyntaxNode reportNode, string projectRoot)
    {
        // Find all layout definitions within the report
        var layouts = reportNode.DescendantNodes()
            .Where(n => n.IsKind(SyntaxKind.ReportLayout));

        foreach (var layout in layouts)
        {
            // Find LayoutFile property within this layout
            var layoutFileProperty = layout.DescendantNodes()
                .OfType<PropertySyntax>()
                .FirstOrDefault(p => p.Name.Identifier.ToString().Equals("LayoutFile", StringComparison.OrdinalIgnoreCase));

            if (layoutFileProperty?.Value is StringPropertyValueSyntax stringProperty)
            {
                var fileName = GetStringPropertyValue(stringProperty);
                if (!string.IsNullOrEmpty(fileName))
                {
                    var fullPath = Path.Combine(projectRoot, fileName);
                    if (!File.Exists(fullPath))
                    {
                        var diagnostic = Diagnostic.Create(
                            DiagnosticDescriptors.Rule0092ValidateReportLayoutFiles,
                            layoutFileProperty.GetLocation(),
                            fileName);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }

    private static void ValidateLegacyLayoutProperties(SyntaxNodeAnalysisContext context, SyntaxNode reportNode, string projectRoot)
    {
        var legacyProperties = new[] { "RDLCLayout", "WordLayout", "ExcelLayout" };

        foreach (var propertyName in legacyProperties)
        {
            var property = reportNode.DescendantNodes()
                .OfType<PropertySyntax>()
                .FirstOrDefault(p => p.Name.Identifier.ToString().Equals(propertyName, StringComparison.OrdinalIgnoreCase));

            if (property?.Value is StringPropertyValueSyntax stringProperty)
            {
                var fileName = GetStringPropertyValue(stringProperty);
                if (!string.IsNullOrEmpty(fileName))
                {
                    var fullPath = Path.Combine(projectRoot, fileName);
                    if (!File.Exists(fullPath))
                    {
                        var diagnostic = Diagnostic.Create(
                            DiagnosticDescriptors.Rule0092ValidateReportLayoutFiles,
                            property.GetLocation(),
                            fileName);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }

    private static string? GetStringPropertyValue(StringPropertyValueSyntax stringProperty)
    {
        return stringProperty.Value?.Value.ValueText;
    }
}
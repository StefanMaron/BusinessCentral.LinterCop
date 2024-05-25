using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.InternalSyntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Packaging;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;
using System.Collections.Immutable;
#if ManifestHelper
using Microsoft.Dynamics.Nav.Analyzers.Common;
#else
using Microsoft.Dynamics.Nav.Analyzers.Common.AppSourceCopConfiguration;
#endif


namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0052InternalProceduresNotReferencedAnalyzer : DiagnosticAnalyzer
    {

        private class MethodSymbolAnalyzer : IDisposable
        {
            private readonly PooledDictionary<IMethodSymbol, string> methodSymbols = PooledDictionary<IMethodSymbol, string>.GetInstance();

            private readonly PooledDictionary<IMethodSymbol, string> internalMethodsUnused = PooledDictionary<IMethodSymbol, string>.GetInstance();
            private readonly PooledDictionary<IMethodSymbol, string> internalMethodsUsedInCurrentObject = PooledDictionary<IMethodSymbol, string>.GetInstance();
            private readonly PooledDictionary<IMethodSymbol, string> internalMethodsUsedInOtherObjects = PooledDictionary<IMethodSymbol, string>.GetInstance();

            private readonly AttributeKind[] attributeKindsOfMethodsToSkip = new AttributeKind[] { AttributeKind.ConfirmHandler, AttributeKind.FilterPageHandler, AttributeKind.HyperlinkHandler, AttributeKind.MessageHandler, AttributeKind.ModalPageHandler, AttributeKind.PageHandler, AttributeKind.RecallNotificationHandler, AttributeKind.ReportHandler, AttributeKind.RequestPageHandler, AttributeKind.SendNotificationHandler, AttributeKind.SessionSettingsHandler, AttributeKind.StrMenuHandler, AttributeKind.Test };

            public MethodSymbolAnalyzer(CompilationAnalysisContext compilationAnalysisContext)
            {
#if ManifestHelper
                NavAppManifest manifest = ManifestHelper.GetManifest(compilationAnalysisContext.Compilation);
#else
                NavAppManifest manifest = AppSourceCopConfigurationProvider.GetManifest(compilationAnalysisContext.Compilation);
#endif

                if (manifest.InternalsVisibleTo != null && manifest.InternalsVisibleTo.Any())
                {
                    return;
                }

                ImmutableArray<IApplicationObjectTypeSymbol>.Enumerator objectEnumerator = compilationAnalysisContext.Compilation.GetDeclaredApplicationObjectSymbols().GetEnumerator();
                while (objectEnumerator.MoveNext())
                {
                    IApplicationObjectTypeSymbol applicationSymbol = objectEnumerator.Current;
                    ImmutableArray<ISymbol>.Enumerator objectMemberEnumerator = applicationSymbol.GetMembers().GetEnumerator();
                    while (objectMemberEnumerator.MoveNext())
                    {
                        ISymbol objectMember = objectMemberEnumerator.Current;
                        if (objectMember.Kind == SymbolKind.Method)
                        {
                            IMethodSymbol methodSymbol = objectMember as IMethodSymbol;
                            if (MethodNeedsReferenceCheck(methodSymbol))
                            {
                                methodSymbols.Add(methodSymbol, methodSymbol.Name.ToLowerInvariant());
                                internalMethodsUnused.Add(methodSymbol, methodSymbol.Name.ToLowerInvariant());
                            }
                        }
                    }
                }
            }

            private bool MethodNeedsReferenceCheck(IMethodSymbol methodSymbol)
            {
                if (methodSymbol.MethodKind != MethodKind.Method)
                {
                    return false;
                }
                if (methodSymbol.IsObsoletePending)
                {
                    return false;
                }
                if (methodSymbol.Attributes.Any(attr => attributeKindsOfMethodsToSkip.Contains(attr.AttributeKind)))
                {
                    return false;
                }
                // If the procedure and implements an interface, then we do not need to check for references for this procedure
                IApplicationObjectTypeSymbol objectSymbol = methodSymbol.GetContainingApplicationObjectTypeSymbol();
                if (objectSymbol != null && HelperFunctions.MethodImplementsInterfaceMethod(objectSymbol, methodSymbol))
                {
                    return false;
                }
                if (!methodSymbol.IsInternal)
                {
                    // Check if public procedure in internal object
                    if (methodSymbol.DeclaredAccessibility == Accessibility.Public && objectSymbol != null)
                    {
                        // If the containing object is not an internal object, then we do not need to check for references for this public procedure.
                        if (objectSymbol.DeclaredAccessibility != Accessibility.Internal)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                // If the procedure has signature ProcedureName(HostNotification: Notification) or ProcedureName(ErrorInfo: ErrorInfo), then the procedure does not need a reference check
                if (methodSymbol.Parameters.Length == 1)
                {
                    ITypeSymbol firstParameterTypeSymbol = methodSymbol.Parameters[0].ParameterType;
                    if (firstParameterTypeSymbol.GetNavTypeKindSafe() == NavTypeKind.Notification || firstParameterTypeSymbol.GetNavTypeKindSafe() == NavTypeKind.ErrorInfo)
                    {
                        return false;
                    }
                }

                return true;
            }

            public void AnalyzeObjectSyntax(CompilationAnalysisContext compilationAnalysisContext)
            {
                if (methodSymbols.Count == 0)
                {
                    return;
                }

                Compilation compilation = compilationAnalysisContext.Compilation;
                ImmutableArray<SyntaxTree>.Enumerator enumerator = compilation.SyntaxTrees.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (methodSymbols.Count == 0)
                    {
                        break;
                    }

                    SyntaxTree syntaxTree = enumerator.Current;
                    SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                    syntaxTree.GetRoot().WalkDescendantsAndPerformAction(delegate (SyntaxNode syntaxNode)
                    {
                        if (methodSymbols.Count == 0)
                        {
                            return;
                        }
                        if (syntaxNode.Parent.IsKind(SyntaxKind.MethodDeclaration) || !syntaxNode.IsKind(SyntaxKind.IdentifierName))
                        {
                            return;
                        }
                        IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)syntaxNode;
                        if (methodSymbols.ContainsValue(identifierNameSyntax.Identifier.ValueText.ToLowerInvariant()) && TryGetSymbolFromIdentifier(semanticModel, (IdentifierNameSyntax)syntaxNode, SymbolKind.Method, out var methodSymbol))
                        {
                            if (methodSymbol.IsInternal)
                            {
                                var objectSyntax = syntaxNode.GetContainingApplicationObjectSyntax();
                                var objectSyntaxName = objectSyntax.Name.Identifier.ValueText.ToLowerInvariant();

                                var methodObjectSymbol = methodSymbol.GetContainingApplicationObjectTypeSymbol();
                                var methodObjectSymbolName = methodObjectSymbol.Name.ToLowerInvariant();

                                if (
                                    (methodObjectSymbolName == objectSyntaxName) &&
                                    (objectSyntax.Kind.ToString().Replace("Object", "").ToLowerInvariant() == methodObjectSymbol.Kind.ToString().ToLowerInvariant())
                                )
                                {
                                    internalMethodsUsedInCurrentObject[methodSymbol] = methodSymbol.Name.ToLowerInvariant();
                                }
                                else
                                {
                                    internalMethodsUsedInOtherObjects[methodSymbol] = methodSymbol.Name.ToLowerInvariant();
                                }
                            }
                            internalMethodsUnused.Remove(methodSymbol);
                        }
                    });
                }
            }

            internal static bool TryGetSymbolFromIdentifier(SemanticModel semanticModel, IdentifierNameSyntax identifierName, SymbolKind symbolKind, out IMethodSymbol methodSymbol)
            {
                methodSymbol = null;
                SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(identifierName);
                ISymbol symbol = symbolInfo.Symbol;
                if (symbol == null || symbol.Kind != symbolKind)
                {
                    return false;
                }
                methodSymbol = symbolInfo.Symbol as IMethodSymbol;
                if (methodSymbol == null)
                {
                    return false;
                }
                return true;
            }

            public void ReportUnchangedReferencePassedParameters(Action<Diagnostic> action)
            {
                if (internalMethodsUnused.Count == 0)
                {
                    return;
                }
                foreach (KeyValuePair<IMethodSymbol, string> unusedInternalMethod in internalMethodsUnused)
                {
                    IMethodSymbol methodSymbol = unusedInternalMethod.Key;
                    IApplicationObjectTypeSymbol objectSymbol = methodSymbol.GetContainingApplicationObjectTypeSymbol();

                    Diagnostic diagnostic = Diagnostic.Create(DiagnosticDescriptors.Rule0052InternalProceduresNotReferencedAnalyzerDescriptor, methodSymbol.OriginalDefinition.GetLocation(), methodSymbol.DeclaredAccessibility.ToString().ToLowerInvariant(), StringExtensions.QuoteIdentifierIfNeeded(methodSymbol.Name), objectSymbol.NavTypeKind, objectSymbol.Name, objectSymbol.DeclaredAccessibility);
                    action(diagnostic);
                }
            }

            public void ReportInternalMethodOnlyReferencedInCurrentObject(Action<Diagnostic> action)
            {
                var internalMethodsUsedOnlyInCurrentObject = internalMethodsUsedInCurrentObject.Except(internalMethodsUsedInOtherObjects);

                foreach (KeyValuePair<IMethodSymbol, string> internalMethodPair in internalMethodsUsedOnlyInCurrentObject)
                {
                    IMethodSymbol methodSymbol = internalMethodPair.Key;
                    IApplicationObjectTypeSymbol objectSymbol = methodSymbol.GetContainingApplicationObjectTypeSymbol();

                    Diagnostic diagnostic = Diagnostic.Create(DiagnosticDescriptors.Rule0053InternalProcedureOnlyUsedInCurrentObjectAnalyzerDescriptor, methodSymbol.OriginalDefinition.GetLocation(), methodSymbol.DeclaredAccessibility.ToString().ToLowerInvariant(), StringExtensions.QuoteIdentifierIfNeeded(methodSymbol.Name), objectSymbol.NavTypeKind, objectSymbol.Name, objectSymbol.DeclaredAccessibility);
                    action(diagnostic);
                }
            }

            public void Dispose()
            {
                methodSymbols.Free();
            }
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.Rule0052InternalProceduresNotReferencedAnalyzerDescriptor, DiagnosticDescriptors.Rule0053InternalProcedureOnlyUsedInCurrentObjectAnalyzerDescriptor);


        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationAction(CheckApplicationObjects);
        }

        private static void CheckApplicationObjects(CompilationAnalysisContext compilationAnalysisContext)
        {
            MethodSymbolAnalyzer methodSymbolAnalyzer = new MethodSymbolAnalyzer(compilationAnalysisContext);
            methodSymbolAnalyzer.AnalyzeObjectSyntax(compilationAnalysisContext);
            methodSymbolAnalyzer.ReportUnchangedReferencePassedParameters(compilationAnalysisContext.ReportDiagnostic);
            methodSymbolAnalyzer.ReportInternalMethodOnlyReferencedInCurrentObject(compilationAnalysisContext.ReportDiagnostic);
        }
    }
}

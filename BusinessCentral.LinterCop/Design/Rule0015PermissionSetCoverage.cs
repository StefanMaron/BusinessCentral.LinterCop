using System.Collections.Immutable;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Dynamics.Nav.Analyzers.Common;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0015PermissionSetCoverage : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.Rule0015PermissionSetCoverage);

    private static readonly Dictionary<NavTypeKind, PermissionObjectKind> navTypeToPermObjectKind = new()
    {
        { NavTypeKind.Codeunit, PermissionObjectKind.Codeunit },
        { NavTypeKind.Page, PermissionObjectKind.Page },
        { NavTypeKind.Query, PermissionObjectKind.Query },
        { NavTypeKind.Report, PermissionObjectKind.Report },
        { NavTypeKind.Record, PermissionObjectKind.Table },
        { NavTypeKind.XmlPort, PermissionObjectKind.Xmlport }
    };

    public override void Initialize(AnalysisContext context)
        => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckPermissionSetCoverage), SymbolKind.Module);

    private void CheckPermissionSetCoverage(SymbolAnalysisContext ctx)
    {
        if (ctx.Compilation.FileSystem is null)
            return;

        if (ctx.Symbol is not IModuleSymbol moduleSymbol)
            return;

        ImmutableHashSet<(PermissionObjectKind, int)> permissionSymbols = GetPermissionSymbols(moduleSymbol);
        IEnumerable<XDocument> permissionSetDocuments = FileSystemExtensions.GetPermissionSetDocuments(ctx.Compilation.FileSystem);
        IEnumerable<ISymbol> objects = moduleSymbol.GetObjectSymbols(SymbolKind.Codeunit);
        objects = objects.Concat(moduleSymbol.GetObjectSymbols(SymbolKind.Page));
        objects = objects.Concat(moduleSymbol.GetObjectSymbols(SymbolKind.Query));
        objects = objects.Concat(moduleSymbol.GetObjectSymbols(SymbolKind.Report));
        objects = objects.Concat(moduleSymbol.GetObjectSymbols(SymbolKind.Table));
        objects = objects.Concat(moduleSymbol.GetObjectSymbols(SymbolKind.XmlPort));
        IEnumerator<ISymbol> enumerator = objects.GetEnumerator();

        while (enumerator.MoveNext())
        {
            if (enumerator.Current is not IApplicationObjectTypeSymbol appObjTypeSymbol)
                continue;

            if (appObjTypeSymbol.IsObsoleteRemoved)
                continue;

            if (appObjTypeSymbol.Properties.Where(currentProperty => currentProperty.PropertyKind == PropertyKind.InherentPermissions).Any())
                continue;

            int permObjectId = appObjTypeSymbol.Id;

            PermissionObjectKind permObjectKind = PermissionObjectKind.Table;
            if (navTypeToPermObjectKind.TryGetValue(appObjTypeSymbol.NavTypeKind, out var kind))
                permObjectKind = kind;

            if (!(permissionSymbols.Contains((permObjectKind, permObjectId)) || permissionSymbols.Contains((permObjectKind, 0)) || XmlPermissionExistsForObject(permissionSetDocuments, permObjectKind, permObjectId)))
                ctx.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0015PermissionSetCoverage,
                    enumerator.Current.GetLocation(),
                    permObjectKind.ToString(),
                    appObjTypeSymbol.Name));

            if (appObjTypeSymbol.NavTypeKind == NavTypeKind.Record)
            {
                if (((ITableTypeSymbol)appObjTypeSymbol.OriginalDefinition).TableType == TableTypeKind.Normal)
                {
                    permObjectKind = PermissionObjectKind.TableData;

                    if (!(permissionSymbols.Contains((permObjectKind, permObjectId)) || permissionSymbols.Contains((permObjectKind, 0)) || XmlPermissionExistsForObject(permissionSetDocuments, permObjectKind, permObjectId)))
                        ctx.ReportDiagnostic(Diagnostic.Create(
                            DiagnosticDescriptors.Rule0015PermissionSetCoverage,
                            enumerator.Current.GetLocation(),
                            permObjectKind.ToString(),
                            appObjTypeSymbol.Name));
                }
            }
        }
    }

    private static ImmutableHashSet<(PermissionObjectKind, int)> GetPermissionSymbols(IModuleSymbol module)
    {
        ImmutableHashSet<(PermissionObjectKind, int)> immutableHashSet;
        IEnumerable<ISymbol> symbols = module.GetObjectSymbols(SymbolKind.PermissionSet).Concat(module.GetObjectSymbols(SymbolKind.PermissionSetExtension));
        if (!symbols.Any())
        {
            return ImmutableHashSet<(PermissionObjectKind, int)>.Empty;
        }
        PooledHashSet<(PermissionObjectKind, int)> instance = PooledHashSet<(PermissionObjectKind, int)>.GetInstance();
        try
        {
            foreach (ISymbol symbol in symbols)
            {
                ImmutableArray<IPermissionSymbol>.Enumerator enumerator = ((IPermissionSetSymbol)symbol).Permissions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    IPermissionSymbol current = enumerator.Current;
                    instance.Add((current.ObjectType, current.ObjectId));
                }
            }
            immutableHashSet = instance.ToImmutableHashSet();
        }
        finally
        {
            instance.Free();
        }
        return immutableHashSet;
    }

    private bool XmlPermissionExistsForObject(IEnumerable<XDocument> permissionSetDocuments, PermissionObjectKind objectType, int objectId)
    {
        using (IEnumerator<XDocument> permSetEnumerator = permissionSetDocuments.GetEnumerator())
        {
            while (permSetEnumerator.MoveNext())
            {
                using (IEnumerator<XElement>? permissionEnumerator = permSetEnumerator.Current.Root?.XPathSelectElements(Constants.PermissionNodeXPath).GetEnumerator())
                {
                    while (permissionEnumerator is not null && permissionEnumerator.MoveNext())
                    {
                        XElement current = permissionEnumerator.Current;

                        string? xmlObjectType = current.Element("ObjectType")?.Value;

                        if (xmlObjectType != objectType.ToString())
                        {
                            int xmlObjectTypeAsInteger = -1;
                            if (!Int32.TryParse(xmlObjectType, out xmlObjectTypeAsInteger))
                            {
                                continue;
                            }
                            if (xmlObjectTypeAsInteger != (int)objectType)
                            {
                                continue;
                            }
                        }

                        int xmlObjectId = -1;
                        if (!Int32.TryParse(current.Element("ObjectID")?.Value, out xmlObjectId))
                        {
                            continue;
                        }
                        if (xmlObjectId != objectId)
                        {
                            continue;
                        }

                        return true;
                    }
                }
            }
            return false;
        }
    }
}
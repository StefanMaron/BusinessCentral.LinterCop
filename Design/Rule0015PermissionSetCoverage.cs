using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.Analyzers.Common;
using System.Xml.Linq;
using System.Xml.XPath;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    class Rule0015PermissionSetCoverage : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0015PermissionSetCoverage);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckPermissionSetCoverage), SymbolKind.Module);

        private void CheckPermissionSetCoverage(SymbolAnalysisContext context)
        {
            IModuleSymbol moduleSymbol = context.Symbol as IModuleSymbol;
            if (moduleSymbol == null)
            {
                return;
            }

            ImmutableHashSet<(PermissionObjectKind, int)> permissionSymbols = GetPermissionSymbols(moduleSymbol);
            IEnumerable<XDocument> permissionSetDocuments = FileSystemExtensions.GetPermissionSetDocuments(context.Compilation.FileSystem);
            IEnumerable<ISymbol> objects = moduleSymbol.GetObjectSymbols(SymbolKind.Codeunit);
            objects = objects.Concat(moduleSymbol.GetObjectSymbols(SymbolKind.Page));
            objects = objects.Concat(moduleSymbol.GetObjectSymbols(SymbolKind.Query));
            objects = objects.Concat(moduleSymbol.GetObjectSymbols(SymbolKind.Report));
            objects = objects.Concat(moduleSymbol.GetObjectSymbols(SymbolKind.Table));
            objects = objects.Concat(moduleSymbol.GetObjectSymbols(SymbolKind.XmlPort));
            IEnumerator<ISymbol> enumerator = objects.GetEnumerator();

            while (enumerator.MoveNext())
            {
                ISymbol current = enumerator.Current;
                IApplicationObjectTypeSymbol appObjTypeSymbol = (IApplicationObjectTypeSymbol)current;
                PermissionObjectKind permObjectKind = PermissionObjectKind.Table;
                int permObjectId = appObjTypeSymbol.Id;

                if (appObjTypeSymbol.IsObsoleteRemoved)
                {
                    continue;
                }

                if (appObjTypeSymbol.Properties.Where(currentProperty => currentProperty.PropertyKind == PropertyKind.InherentPermissions).Any()) continue;

                switch (appObjTypeSymbol.NavTypeKind)
                {
                    case NavTypeKind.Codeunit:
                        permObjectKind = PermissionObjectKind.Codeunit;
                        break;
                    case NavTypeKind.Page:
                        permObjectKind = PermissionObjectKind.Page;
                        break;
                    case NavTypeKind.Query:
                        permObjectKind = PermissionObjectKind.Query;
                        break;
                    case NavTypeKind.Report:
                        permObjectKind = PermissionObjectKind.Report;
                        break;
                    case NavTypeKind.Record:
                        permObjectKind = PermissionObjectKind.Table;
                        break;
                    case NavTypeKind.XmlPort:
                        permObjectKind = PermissionObjectKind.Xmlport;
                        break;
                }

                if (!(permissionSymbols.Contains((permObjectKind, permObjectId)) || permissionSymbols.Contains((permObjectKind, 0)) || XmlPermissionExistsForObject(permissionSetDocuments, permObjectKind, permObjectId)))
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0015PermissionSetCoverage, current.GetLocation(), new Object[] { permObjectKind.ToString(), appObjTypeSymbol.Name }));

                if (appObjTypeSymbol.NavTypeKind == NavTypeKind.Record)
                {
                    if (((ITableTypeSymbol)(appObjTypeSymbol.OriginalDefinition)).TableType == TableTypeKind.Normal)
                    {
                        permObjectKind = PermissionObjectKind.TableData;

                        if (!(permissionSymbols.Contains((permObjectKind, permObjectId)) || permissionSymbols.Contains((permObjectKind, 0)) || XmlPermissionExistsForObject(permissionSetDocuments, permObjectKind, permObjectId)))
                            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0015PermissionSetCoverage, current.GetLocation(), new Object[] { permObjectKind.ToString(), appObjTypeSymbol.Name }));
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
                    using (IEnumerator<XElement> permissionEnumerator = permSetEnumerator.Current.Root.XPathSelectElements(Constants.PermissionNodeXPath).GetEnumerator())
                    {
                        while (permissionEnumerator.MoveNext())
                        {
                            XElement current = permissionEnumerator.Current;

                            string xmlObjectType = current.Element("ObjectType").Value;

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
                            if (!Int32.TryParse(current.Element("ObjectID").Value, out xmlObjectId))
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
}

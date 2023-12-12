using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;
using System.Reflection;

namespace BusinessCentral.LinterCop.Design
{
    public delegate bool Filter(IGrouping<int, Rule0044AnalyzeTransferFields.Field> fieldGroup);

    [DiagnosticAnalyzer]
    public class Rule0044AnalyzeTransferFields : DiagnosticAnalyzer
    {
        private List<Tuple<string, string>> tablePairs = new List<Tuple<string, string>>();

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0044AnalyzeTransferFields);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(AnalyzeTableExtension), SyntaxKind.TableExtensionObject);
            context.RegisterOperationAction(new Action<OperationAnalysisContext>(AnalyzeTransferFields), OperationKind.InvocationExpression);
            LoadTablePairs();
        }

        private void AnalyzeTableExtension(SyntaxNodeAnalysisContext ctx)
        {
            TableExtensionSyntax tableExtensionSyntax = (TableExtensionSyntax)ctx.Node;
            string? baseObject = GetIdentifierName(tableExtensionSyntax.BaseObject.Identifier);

            if (baseObject == null)
                return;

            IEnumerable<Tuple<string, string>> tables = tablePairs.Where(x => x.Item1.Equals(baseObject) || x.Item2.Equals(baseObject));

            if (tables.Count() == 0)
                return;

            Table table1 = new Table(baseObject);
            table1.PopulateFields(tableExtensionSyntax.Fields);

            Dictionary<string, TableExtensionSyntax> tableExtensions = GetTableExtensions(ctx.SemanticModel.Compilation);

            foreach (Tuple<string, string> tablePair in tables)
            {
                string tableName = baseObject.Equals(tablePair.Item1) ? tablePair.Item2 : tablePair.Item1;

                Table table2 = GetTableWithFieldsByTableName(ctx.SemanticModel.Compilation, tableName, tableExtensions);

                List<IGrouping<int, Field>> fieldGroups = GetFieldsWithSameIDAndApplyFilter(table1.Fields, table2.Fields, DifferentNameAndTypeFilter);

                ReportFieldDiagnostics(ctx, table1, fieldGroups);
            }
        }

        private async void AnalyzeTransferFields(OperationAnalysisContext ctx)
        {
            if (ctx.Operation.Syntax.GetType() != typeof(InvocationExpressionSyntax))
                return;

            InvocationExpressionSyntax invocationExpression = (InvocationExpressionSyntax)ctx.Operation.Syntax;

            Tuple<string, string>? records = GetInvokingRecordNames(invocationExpression);

            if (records == null)
                return;

            Task<SyntaxNode> localVariablesTask = ctx.ContainingSymbol.DeclaringSyntaxReference.GetSyntaxAsync();
            Task<SyntaxNode> globalVariablesTask = ctx.ContainingSymbol.ContainingSymbol.DeclaringSyntaxReference.GetSyntaxAsync();

            List<VariableDeclarationBaseSyntax> variables = new List<VariableDeclarationBaseSyntax>();

            SyntaxNode localVariables = await localVariablesTask;
            variables.AddRange(FindLocalVariables(localVariables));
            SyntaxNode globalVariables = await globalVariablesTask;
            variables.AddRange(FindGlobalVariables(globalVariables));

            string? tableName1 = GetObjectName(variables.FirstOrDefault(x =>
            {
                string? name = x.GetNameStringValue();

                if (name == null)
                    return false;

                return name.Equals(records.Item1);
            }));

            string? tableName2 = GetObjectName(variables.FirstOrDefault(x =>
            {
                string? name = x.GetNameStringValue();

                if (name == null)
                    return false;

                return name.Equals(records.Item2);
            }));

            if (tableName1 == null && (records.Item1.ToLower().Equals("rec") || records.Item1.ToLower().Equals("xrec")))
                tableName1 = GetObjectSourceTable(globalVariables, ctx.Compilation);

            if (tableName2 == null && (records.Item2.ToLower().Equals("rec") || records.Item2.ToLower().Equals("xrec")))
                tableName2 = GetObjectSourceTable(globalVariables, ctx.Compilation);

            if (tableName1 == tableName2 || tableName1 == null || tableName2 == null)
                return;

            Dictionary<string, TableExtensionSyntax> tableExtensions = GetTableExtensions(ctx.Compilation);

            Table table1 = GetTableWithFieldsByTableName(ctx.Compilation, tableName1);
            Table table2 = GetTableWithFieldsByTableName(ctx.Compilation, tableName2);

            List<IGrouping<int, Field>> fieldGroups = GetFieldsWithSameIDAndApplyFilter(table1.Fields, table2.Fields, DifferentNameAndTypeFilter);

            if (fieldGroups.Any())
            {
                ReportFieldDiagnostics(ctx, table1, fieldGroups);
                ReportFieldDiagnostics(ctx, table2, fieldGroups);

                if (table1.Fields.Any(x => x.Location != null) || table2.Fields.Any(x => x.Location != null))
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0044AnalyzeTransferFields, invocationExpression.GetLocation(), table1.Name, table2.Name));
            }
        }

        private void ReportFieldDiagnostics(OperationAnalysisContext ctx, Table table, List<IGrouping<int, Field>> fieldGroups)
        {
            foreach (IGrouping<int, Field> fieldGroup in fieldGroups)
            {
                IEnumerable<Field> fieldGroupValues = fieldGroup.ToList();

                Field field = fieldGroupValues.First(x => x.Table.Equals(table));

                if (field.Location == null)
                    continue;

                foreach (Field fieldGroupValue in fieldGroupValues)
                {
                    if (fieldGroupValue.Equals(field))
                        continue;

                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0044AnalyzeTableExtension, field.Location, fieldGroupValue.Table.Name));
                }
            }
        }

        private void ReportFieldDiagnostics(SyntaxNodeAnalysisContext ctx, Table table, List<IGrouping<int, Field>> fieldGroups)
        {
            foreach (IGrouping<int, Field> fieldGroup in fieldGroups)
            {
                IEnumerable<Field> fieldGroupValues = fieldGroup.ToList();

                Field field = fieldGroupValues.First(x => x.Table.Equals(table));

                if (field.Location == null)
                    continue;

                foreach(Field fieldGroupValue in fieldGroupValues)
                {
                    if (fieldGroupValue.Equals(field))
                        continue;

                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0044AnalyzeTableExtension, field.Location, fieldGroupValue.Table.Name));
                }
            }
        }

        private string? GetObjectSourceTable(SyntaxNode node, Compilation compilation)
        {
            Type nodeType = node.GetType();

            if(nodeType == typeof(TableSyntax))
            {
                TableSyntax tableSyntax = (TableSyntax)node;

                return tableSyntax.Name.Identifier.ToString().Replace("\"", "");
            }

            if(nodeType == typeof(TableExtensionSyntax))
            {
                TableExtensionSyntax tableExtensionSyntax = (TableExtensionSyntax)node;

                return GetIdentifierName(tableExtensionSyntax.BaseObject.Identifier);
            }

            if(nodeType == typeof(PageSyntax))
            {
                PageSyntax pageSyntax = (PageSyntax)node;

                foreach(PropertySyntaxOrEmpty property in  pageSyntax.PropertyList.Properties)
                {
                    if (property.GetType() != typeof(PropertySyntax))
                        continue;

                    PropertySyntax propertySyntax = (PropertySyntax)property;

                    if (!propertySyntax.Name.Identifier.ToString().Equals("SourceTable"))
                        continue;

                    if (propertySyntax.Value.GetType() != typeof(ObjectReferencePropertyValueSyntax))
                        continue;

                    ObjectReferencePropertyValueSyntax objectReferencePropertyValueSyntax = (ObjectReferencePropertyValueSyntax) propertySyntax.Value;

                    return GetIdentifierName(objectReferencePropertyValueSyntax.ObjectNameOrId.Identifier);
                }

                return null;
            }

            if(nodeType == typeof(PageExtensionSyntax))
            {
                PageExtensionSyntax pageExtensionSyntax = (PageExtensionSyntax) node;

                string? pageExtensionName = GetIdentifierName(pageExtensionSyntax.BaseObject.Identifier);

                if (pageExtensionName == null)
                    return null;

                return GetSourceTableByPageName(compilation, pageExtensionName);
            }


            return null;
        }

        private string? GetIdentifierName(SyntaxNode identifier)
        {
            if (identifier.GetType() != typeof(IdentifierNameSyntax))
                return null;

            IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)identifier;

            return identifierNameSyntax.ToString().Replace("\"", "");
        }

        private Tuple<string, string>? GetInvokingRecordNames(InvocationExpressionSyntax invocationExpression)
        {
            if (invocationExpression.ArgumentList.Arguments.Count < 1)
                return null;

            if (invocationExpression.Expression.GetType() == typeof(MemberAccessExpressionSyntax))
            {
                MemberAccessExpressionSyntax memberAccessExpression = (MemberAccessExpressionSyntax)invocationExpression.Expression;
                if (!memberAccessExpression.Name.ToString().Equals("TransferFields"))
                    return null;

                return new Tuple<string, string>(memberAccessExpression.Expression.ToString(), invocationExpression.ArgumentList.Arguments[0].ToString()); 
            }
                    
            if(invocationExpression.Expression.GetType() ==  typeof(IdentifierNameSyntax))
            {
                IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)invocationExpression.Expression;

                if (!identifierNameSyntax.Identifier.ToString().Equals("TransferFields"))
                    return null;

                return new Tuple<string, string>("Rec", invocationExpression.ArgumentList.Arguments[0].ToString());
            }

            return null;
        }

        private Dictionary<string, TableExtensionSyntax> GetTableExtensions(Compilation compilation)
        {
            Dictionary<string, TableExtensionSyntax> tableExtensions = new Dictionary<string, TableExtensionSyntax>();

            foreach (SyntaxTree syntaxTree in compilation.SyntaxTrees)
            {
                foreach(SyntaxNode node in syntaxTree.GetRoot().ChildNodes())
                {
                    if (node.GetType() != typeof(TableExtensionSyntax))
                        continue;

                    TableExtensionSyntax tableExtension = (TableExtensionSyntax) node;

                    string? extendedTable = GetIdentifierName(tableExtension.BaseObject.Identifier);

                    if (extendedTable == null)
                        continue;

                    try
                    {
                        tableExtensions.Add(extendedTable, tableExtension);
                    }
                    catch { }
                }
            }

            return tableExtensions;
        }

        private Table GetTableWithFieldsByTableName(Compilation compilation, string tableName, Dictionary<string, TableExtensionSyntax>? tableExtensions = null)
        {
            IApplicationObjectTypeSymbol tableSymbol = compilation.GetApplicationObjectTypeSymbolsByNameAcrossModules(SymbolKind.Table, tableName).FirstOrDefault();

            Table table = new Table(tableName);

            if(tableSymbol != null)
            {
                SyntaxReference? syntaxReference = tableSymbol.DeclaringSyntaxReference;

                if (syntaxReference != null)
                {
                    TableSyntax tableSyntax = (TableSyntax) syntaxReference.GetSyntax();

                    table.PopulateFields(tableSyntax.Fields);
                }
                else
                    table.PopulateFields(tableSymbol);
            }

            if (tableExtensions == null)
                return table;

            TableExtensionSyntax? tableExtension;
            if (tableExtensions.TryGetValue(tableName, out tableExtension))
            {
                table.PopulateFields(tableExtension.Fields);
            }

            return table;
        }

        private string? GetSourceTableByPageName(Compilation compilation, string pageName)
        {
            IApplicationObjectTypeSymbol pageSymbol = compilation.GetApplicationObjectTypeSymbolsByNameAcrossModules(SymbolKind.Page, pageName).FirstOrDefault();

            if (pageSymbol != null)
            {
                SyntaxReference? syntaxReference = pageSymbol.DeclaringSyntaxReference;

                if (syntaxReference != null)
                {
                    PageSyntax pageSyntax = (PageSyntax)syntaxReference.GetSyntax();

                    foreach (PropertySyntaxOrEmpty property in pageSyntax.PropertyList.Properties)
                    {
                        if (property.GetType() != typeof(PropertySyntax))
                            continue;

                        PropertySyntax propertySyntax = (PropertySyntax)property;

                        if (!propertySyntax.Name.Identifier.ToString().Equals("SourceTable"))
                            continue;

                        if (propertySyntax.Value.GetType() != typeof(ObjectReferencePropertyValueSyntax))
                            continue;

                        ObjectReferencePropertyValueSyntax objectReferencePropertyValueSyntax = (ObjectReferencePropertyValueSyntax)propertySyntax.Value;

                        return GetIdentifierName(objectReferencePropertyValueSyntax.ObjectNameOrId.Identifier);
                    }
                }
                else
                {
                    Assembly assembly = typeof(Microsoft.Dynamics.Nav.CodeAnalysis.Symbols.VariableKind).Assembly;

                    Type type = assembly.GetType(pageSymbol.GetType().ToString());

                    MethodInfo method = type.GetMethod("GetRelatedTable", BindingFlags.NonPublic | BindingFlags.Instance);

                    object obj = method.Invoke(pageSymbol, null);

                    if (obj == null)
                        return null;

                    type = assembly.GetType(obj.GetType().ToString());

                    method = type.GetMethod("get_Name", BindingFlags.Public | BindingFlags.Instance);

                    return (string) method.Invoke(obj, null);
                }
            }

            return null;
        }

        private List<IGrouping<int, Field>> GetFieldsWithSameIDAndApplyFilter(List<Field> fields1, List<Field> fields2, Filter filter)
        {
            List<Field> result = new List<Field>(fields1);
            result.AddRange(fields2);

            return result.GroupBy(x => x.Id).Where(x => x.Count() > 1 && filter(x)).ToList();
        }

        private bool DifferentNameAndTypeFilter(IGrouping<int, Field> fieldGroup)
        {
            string name = fieldGroup.First().Name;
            string type = fieldGroup.First().Type;

            foreach(Field field in fieldGroup) {
                if (!(name.Equals(field.Name) && type.Equals(field.Type)))
                    return true;
            }

            return false;
        }

        private string? GetObjectName(VariableDeclarationBaseSyntax variable)
        {
            if (variable == null)
                return null;

            SubtypedDataTypeSyntax subtypedData = (SubtypedDataTypeSyntax) variable.Type.DataType;
            return GetIdentifierName(subtypedData.Subtype.Identifier);
        }

        private List<VariableDeclarationBaseSyntax> FindLocalVariables(SyntaxNode node)
        {
            VarSectionSyntax varSection = (VarSectionSyntax) node.DescendantNodes().FirstOrDefault(x => x.Kind == SyntaxKind.VarSection);

            if (varSection == null)
                return new List<VariableDeclarationBaseSyntax>();

            return varSection.Variables.ToList();
        }

        private List<VariableDeclarationBaseSyntax> FindGlobalVariables(SyntaxNode node)
        {
            GlobalVarSectionSyntax varSection = (GlobalVarSectionSyntax)node.DescendantNodes().FirstOrDefault(x => x.Kind == SyntaxKind.GlobalVarSection);

            if (varSection == null)
                return new List<VariableDeclarationBaseSyntax>();

            return varSection.Variables.ToList();
        }

        private void LoadTablePairs()
        {
            tablePairs = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("Contact", "Customer"),
                new Tuple<string, string>("Contact", "Vendor"),
                new Tuple<string, string>("Contact", "Employee"),
                new Tuple<string, string>("Contact", "BankAccount"),

                new Tuple<string, string>("Deferral Line", "Deferral Line Archive"),
                new Tuple<string, string>("Deferral Line", "Posted Deferral Line"),
                new Tuple<string, string>("Deferral Header", "Deferral Header Archive"),
                new Tuple<string, string>("Deferral Header", "Posted Deferral Header"),

                new Tuple<string, string>("Gen. Journal Line", "Posted Gen. Journal Line"),
                new Tuple<string, string>("Gen. Journal Line", "Standard General Journal Line"),

                new Tuple<string, string>("Item Journal Line", "Standard Item Journal Line"),

                new Tuple<string, string>("Whse. Item Tracking Line", "Tracking Specification"),
                new Tuple<string, string>("Whse. Item Tracking Line", "Reservation Entry"),
                new Tuple<string, string>("Reservation Entry", "Tracking Specification"),

                new Tuple<string, string>("Detailed CV Ledg. Entry Buffer", "Detailed Cust. Ledg. Entry"),
                new Tuple<string, string>("Detailed CV Ledg. Entry Buffer", "Detailed Vendor Ledg. Entry"),
                new Tuple<string, string>("Detailed CV Ledg. Entry Buffer", "Detailed Employee Ledger Entry"),

                new Tuple<string, string>("CV Ledger Entry Buffer", "Cust. Ledger Entry"),
                new Tuple<string, string>("CV Ledger Entry Buffer", "Cust. Ledger Entry"),

                new Tuple<string, string>("Tracking Specification", "Reservation Entry"),
                new Tuple<string, string>("Assembly Line", "Posted Assembly Line"),
                new Tuple<string, string>("Assembly Header", "Posted Assembly Header"),

                new Tuple<string, string>("Invt. Document Header", "Invt. Receipt Header"),
                new Tuple<string, string>("Invt. Document Header", "Invt. Shipment Header"),

                new Tuple<string, string>("Bank Acc. Reconciliation", "Bank Account Statement"),
                new Tuple<string, string>("Bank Acc. Reconciliation Line", "Bank Account Statement Line"),

                new Tuple<string, string>("Posted Payment Recon. Line", "Bank Acc. Reconciliation Line"),
                new Tuple<string, string>("Posted Payment Recon. Hdr", "Posted Payment Recon. Hdr"),

                new Tuple<string, string>("Prod. Order Rtng Comment Line", "Routing Comment Line"),
                new Tuple<string, string>("Prod. Order Routing Personnel", "Routing Personnel"),
                new Tuple<string, string>("Prod. Order Rtng Qlty Meas.", "Prod. Order Rtng Qlty Meas."),
                new Tuple<string, string>("Prod. Order Routing Tool", "Routing Tool"),

                new Tuple<string, string>("Posted Approval Entry", "Posted Approval Entry"),
                new Tuple<string, string>("Posted Approval Comment Line", "Approval Comment Line"),

                new Tuple<string, string>("Sales Header", "Sales Shipment Header"),
                new Tuple<string, string>("Sales Header", "Sales Invoice Header"),
                new Tuple<string, string>("Sales Header", "Sales Cr.Memo Header"),
                new Tuple<string, string>("Sales Header", "Return Receipt Header"),
                new Tuple<string, string>("Sales Header", "Sales Header Archive"),
                new Tuple<string, string>("Sales Comment Line", "Sales Comment Line Archive"),

                new Tuple<string, string>("Purchase Header", "Purch. Rcpt. Header"),
                new Tuple<string, string>("Purchase Header", "Purch. Inv. Header"),
                new Tuple<string, string>("Purchase Header", "Purch. Cr. Memo Hdr."),
                new Tuple<string, string>("Purchase Header", "Return Shipment Header"),
                new Tuple<string, string>("Purchase Header", "Purchase Header Archive"),
                new Tuple<string, string>("Purchase Comment Line", "Purchase Comment Line Archive"),

                new Tuple<string, string>("Sales Line", "Sales Shipment Line"),
                new Tuple<string, string>("Sales Line", "Sales Invoice Line"),
                new Tuple<string, string>("Sales Line", "Sales Cr.Memo Line"),
                new Tuple<string, string>("Sales Line", "Return Receipt Line"),
                new Tuple<string, string>("Sales Line", "Sales Line Archive"),

                new Tuple<string, string>("Purchase Line", "Purch. Rcpt. Line"),
                new Tuple<string, string>("Purchase Line", "Purch. Inv. Line"),
                new Tuple<string, string>("Purchase Line", "Purch. Cr. Memo Line"),
                new Tuple<string, string>("Purchase Line", "Return Shipment Line"),
                new Tuple<string, string>("Purchase Line", "Purchase Line Archive"),

                new Tuple<string, string>("Warehouse Activity Header", "Registered Whse. Activity Hdr."),
                new Tuple<string, string>("Warehouse Activity Header", "Registered Invt. Movement Hdr."),
                new Tuple<string, string>("Warehouse Activity Header", "Posted Invt. Pick Header"),
                new Tuple<string, string>("Warehouse Activity Header", "Posted Invt. Put-away Header"),

                new Tuple<string, string>("Warehouse Activity Line", "Registered Whse. Activity Line"),
                new Tuple<string, string>("Warehouse Activity Line", "Registered Invt. Movement Line"),
                new Tuple<string, string>("Warehouse Activity Line", "Posted Invt. Pick Line"),
                new Tuple<string, string>("Warehouse Activity Line", "Posted Invt. Put-away Line"),

                new Tuple<string, string>("Time Sheet Header", "Time Sheet Header Archive"),
                new Tuple<string, string>("Time Sheet Line", "Time Sheet Line Archive"),
                new Tuple<string, string>("Time Sheet Detail Archive", "Time Sheet Detail"),
                new Tuple<string, string>("Time Sheet Comment Line", "Time Sheet Cmt. Line Archive")
            };
        }

        public class Field
        {
            public int Id { get; }
            public string Name { get; }
            public string Type { get; }
            public Microsoft.Dynamics.Nav.CodeAnalysis.Text.Location? Location { get; }
            public Table Table { get; }

            public Field(int id, string name, string type, Microsoft.Dynamics.Nav.CodeAnalysis.Text.Location? location, Table table)
            {
                Id = id;
                Name = name;
                Type = type;
                Location = location;
                Table = table;
            }
        }

        public class Table
        {
            public string Name { get; }
            public List<Field> Fields { get; }

            public Table(string name)
            {
                Name = name;
                Fields = new List<Field>();
            }

            public void PopulateFields(IApplicationObjectTypeSymbol table)
            {
                if (table == null)
                    return;

                Assembly assembly = typeof(Microsoft.Dynamics.Nav.CodeAnalysis.Symbols.VariableKind).Assembly;

                Type type = assembly.GetType(table.GetType().ToString());

                MethodInfo method = type.GetMethod("GetFields", BindingFlags.NonPublic | BindingFlags.Instance);

                var collection = (System.Collections.IEnumerable)method.Invoke(table, null);

                foreach (var field in collection)
                {
                    type = assembly.GetType(field.GetType().ToString());

                    PropertyInfo idprop = type.GetProperty("Id", BindingFlags.Instance | BindingFlags.Public);
                    PropertyInfo nameprop = type.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public);
                    PropertyInfo typeprop = type.GetProperty("Type", BindingFlags.Instance | BindingFlags.Public);

                    int id = (int)idprop.GetValue(field);
                    string name = (string)nameprop.GetValue(field);
                    string objtype = typeprop.GetValue(field).ToString();

                    if (id < 2000000000)
                        Fields.Add(new Field(id, name, objtype, null, this));
                }
            }

            public void PopulateFields(FieldExtensionListSyntax fieldList)
            {
                foreach (FieldSyntax field in fieldList.Fields)
                {
                    if(!FieldIsObsolete(field))
                        Fields.Add(new Field((int)field.No.Value, field.Name.Identifier.ToString().Replace("\"", ""), field.Type.ToString(), field.GetLocation(), this));
                }
            }

            public void PopulateFields(FieldListSyntax fieldList)
            {

                foreach (FieldSyntax field in fieldList.Fields)
                {
                    if(!FieldIsObsolete(field))
                        Fields.Add(new Field((int)field.No.Value, field.Name.Identifier.ToString().Replace("\"", ""), field.Type.ToString(), field.GetLocation(), this));
                }
            }

            private bool FieldIsObsolete(FieldSyntax field)
            {
                foreach(PropertySyntax property in field.PropertyList.Properties)
                {
                    if (!property.Name.Identifier.ToString().Equals("ObsoleteState"))
                        continue;

                    if (property.Value.GetType() != typeof(EnumPropertyValueSyntax))
                        continue;

                    EnumPropertyValueSyntax enumPropertyValueSyntax = (EnumPropertyValueSyntax)property.Value;

                    if (enumPropertyValueSyntax.Value.Identifier.ToString().Equals("Removed"))
                        return true;
                }

                return false;
            }
        }
    }
}
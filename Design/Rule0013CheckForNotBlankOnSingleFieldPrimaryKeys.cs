﻿using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System;
using System.Linq;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForSingleFieldPrimaryKeysNotBlank), SymbolKind.Field);
        }
        private void CheckForSingleFieldPrimaryKeysNotBlank(SymbolAnalysisContext context)
        {
            IFieldSymbol field = (IFieldSymbol)context.Symbol;
            if (GetExitCondition(field))
                return;

            ITableTypeSymbol table = (ITableTypeSymbol)field.GetContainingObjectTypeSymbol();
            if (table.PrimaryKey.Fields.Length != 1)
                return;

            if (table.PrimaryKey.Fields[0].Equals(field))
                if (field.GetBooleanPropertyValue(PropertyKind.NotBlank) != true)
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys, field.GetLocation()));
        }

        private static bool GetExitCondition(IFieldSymbol field)
        {
            return
                field.IsObsoletePending ||
                field.IsObsoleteRemoved ||
                field.FieldClass != FieldClassKind.Normal ||
                field.GetContainingObjectTypeSymbol().IsObsoletePending ||
                field.GetContainingObjectTypeSymbol().IsObsoleteRemoved ||
                !field.DeclaringSyntaxReference.GetSyntax().DescendantNodes().Any(Token => Token.Kind == SyntaxKind.LengthDataType);
        }
    }
}

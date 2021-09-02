using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System;

namespace BusinessCentral.LinterCop
{
  public static class DiagnosticDescriptors
  {
        public static readonly DiagnosticDescriptor Rule0001FlowFieldsShouldNotBeEditable = new DiagnosticDescriptor(LinterCopAnalyzers.AnalyzerPrefix + "0001", (LocalizableString)new LocalizableResourceString("Rule0001FlowFieldsShouldNotBeEditable", LinterCopAnalyzers.ResourceManager, typeof(LinterCopAnalyzers)), (LocalizableString)new LocalizableResourceString("Rule0001FlowFieldsShouldNotBeEditableFormat", LinterCopAnalyzers.ResourceManager, typeof(LinterCopAnalyzers)), "Design", DiagnosticSeverity.Warning, true, (LocalizableString)new LocalizableResourceString("Rule0001FlowFieldsShouldNotBeEditableDescription", LinterCopAnalyzers.ResourceManager, typeof(LinterCopAnalyzers)), (string)null, Array.Empty<string>());
    }
}

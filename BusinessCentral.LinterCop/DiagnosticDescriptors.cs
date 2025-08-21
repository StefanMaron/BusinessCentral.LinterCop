using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;

namespace BusinessCentral.LinterCop;

public static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor Rule0000ErrorInRule = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0000",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0000ErrorInRuleTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0000ErrorInRuleFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0000ErrorInRuleDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0000");

    public static readonly DiagnosticDescriptor Rule0001FlowFieldsShouldNotBeEditable = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0001",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0001FlowFieldsShouldNotBeEditable"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0001FlowFieldsShouldNotBeEditableFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0001FlowFieldsShouldNotBeEditableDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0001");

    public static readonly DiagnosticDescriptor Rule0002CommitMustBeExplainedByComment = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0002",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0002CommitMustBeExplainedByComment"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0002CommitMustBeExplainedByCommentFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0002CommitMustBeExplainedByCommentDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0002");


    public static readonly DiagnosticDescriptor Rule0003DoNotUseObjectIDsInVariablesOrProperties = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0003",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0003DoNotUseObjectIDsInVariablesOrProperties"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0003DoNotUseObjectIDsInVariablesOrPropertiesFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0003DoNotUseObjectIDsInVariablesOrPropertiesDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0003");

    public static readonly DiagnosticDescriptor Rule0004LookupPageIdAndDrillDownPageId = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0004",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0004LookupPageIdAndDrillDownPageIdTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0004LookupPageIdAndDrillDownPageIdFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0004LookupPageIdAndDrillDownPageIdDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0004");

    public static readonly DiagnosticDescriptor Rule0005CasingMismatch = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0005",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0005VariableCasingShouldNotDifferFromDeclarationTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0005VariableCasingShouldNotDifferFromDeclarationFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0005VariableCasingShouldNotDifferFromDeclarationDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0005");

    public static readonly DiagnosticDescriptor Rule0006FieldNotAutoIncrementInTemporaryTable = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0006",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0006FieldNotAutoIncrementInTemporaryTableTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0006FieldNotAutoIncrementInTemporaryTableFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0006FieldNotAutoIncrementInTemporaryTableDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0006");

    public static readonly DiagnosticDescriptor Rule0007DataPerCompanyShouldAlwaysBeSet = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0007",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0007DataPerCompanyShouldAlwaysBeSetTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0007DataPerCompanyShouldAlwaysBeSetFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Hidden,
        isEnabledByDefault: false,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0007DataPerCompanyShouldAlwaysBeSetDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0007");

    public static readonly DiagnosticDescriptor Rule0008NoFilterOperatorsInSetRange = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0008",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0008NoFilterOperatorsInSetRangeTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0008NoFilterOperatorsInSetRangeFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0008NoFilterOperatorsInSetRangeDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0008");

    public static readonly DiagnosticDescriptor Rule0009CodeMetricsInfo = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0009",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0009CodeMetricsInfoTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0009CodeMetricsInfoFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: false,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0009CodeMetricsInfoDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0009");

    public static readonly DiagnosticDescriptor Rule0010CodeMetricsWarning = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0010",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0009CodeMetricsInfoTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0009CodeMetricsInfoFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0009CodeMetricsInfoDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0010");

    public static readonly DiagnosticDescriptor Rule0011AccessPropertyShouldAlwaysBeSet = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0011",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0011AccessPropertyShouldAlwaysBeSetTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0011AccessPropertyShouldAlwaysBeSetFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Hidden,
        isEnabledByDefault: false,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0011AccessPropertyShouldAlwaysBeSetDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0011");

    public static readonly DiagnosticDescriptor Rule0012DoNotUseObjectIdInSystemFunctions = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0012",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0012DoNotUseObjectIdInSystemFunctionsTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0012DoNotUseObjectIdInSystemFunctionsFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0012DoNotUseObjectIdInSystemFunctionsDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0012");

    public static readonly DiagnosticDescriptor Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0013",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0013CheckForNotBlankOnSingleFieldPrimaryKeysTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0013CheckForNotBlankOnSingleFieldPrimaryKeysFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0013CheckForNotBlankOnSingleFieldPrimaryKeysDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0013");

    public static readonly DiagnosticDescriptor Rule0014PermissionSetCaptionLength = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0014",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0014PermissionSetCaptionLengthTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0014PermissionSetCaptionLengthFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0014PermissionSetCaptionLengthDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0014");

    public static readonly DiagnosticDescriptor Rule0015PermissionSetCoverage = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0015",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0015PermissionSetCoverageTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0015PermissionSetCoverageFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0015PermissionSetCoverageDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0015");

    public static readonly DiagnosticDescriptor Rule0016CheckForMissingCaptions = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0016",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0016CheckForMissingCaptionsTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0016CheckForMissingCaptionsFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0016CheckForMissingCaptionsDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0016");

    public static readonly DiagnosticDescriptor Rule0017WriteToFlowField = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0017",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0017WriteToFlowFieldTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0017WriteToFlowFieldFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0017WriteToFlowFieldDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0017");

    public static readonly DiagnosticDescriptor Rule0018NoEventsInInternalCodeunitsAnalyzerDescriptor = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0018",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0018NoEventsInInternalCodeunitsTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0018NoEventsInInternalCodeunitsFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0018NoEventsInInternalCodeunitsDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0018");

    public static readonly DiagnosticDescriptor Rule0019DataClassificationFieldEqualsTable = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0019",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0019DataClassificationFieldEqualsTableTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0019DataClassificationFieldEqualsTableFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0019DataClassificationFieldEqualsTableDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0019");

    public static readonly DiagnosticDescriptor Rule0020ApplicationAreaEqualsToPage = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0020",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0020ApplicationAreaEqualsToPageTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0020ApplicationAreaEqualsToPageFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0020ApplicationAreaEqualsToPageDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0020");

    public static readonly DiagnosticDescriptor Rule0021ConfirmImplementConfirmManagement = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0021",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0021ConfirmImplementConfirmManagement"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0021ConfirmImplementConfirmManagement"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0021ConfirmImplementConfirmManagement"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0021");

    public static readonly DiagnosticDescriptor Rule0022GlobalLanguageImplementTranslationHelper = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0022",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0022GlobalLanguageImplementTranslationHelperTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0022GlobalLanguageImplementTranslationHelperFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0022GlobalLanguageImplementTranslationHelperDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0022");

    public static readonly DiagnosticDescriptor Rule0023AlwaysSpecifyFieldgroups = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0023",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0023AlwaysSpecifyFieldgroups"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0023AlwaysSpecifyFieldgroups"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0023AlwaysSpecifyFieldgroups"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0023");

    public static readonly DiagnosticDescriptor Rule0024SemicolonAfterMethodOrTriggerDeclaration = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0024",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0024SemicolonAfterMethodOrTriggerDeclarationTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0024SemicolonAfterMethodOrTriggerDeclarationFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0024SemicolonAfterMethodOrTriggerDeclarationDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0024");

    public static readonly DiagnosticDescriptor Rule0025InternalProcedureModifier = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0025",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0025InternalProcedureModifierTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0025InternalProcedureModifierFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Hidden,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0025InternalProcedureModifierDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0025");

    public static readonly DiagnosticDescriptor Rule0026ToolTipMustEndWithDot = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0026",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0026ToolTipMustEndWithDotTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0026ToolTipMustEndWithDotFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0026ToolTipMustEndWithDotDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0026");

    public static readonly DiagnosticDescriptor Rule0027RunPageImplementPageManagement = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0027",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0027RunPageImplementPageManagementTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0027RunPageImplementPageManagementFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0027RunPageImplementPageManagementDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0027");

    public static readonly DiagnosticDescriptor Rule0028IdentifiersInEventSubscribers = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0028",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0028IdentifiersInEventSubscribersTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0028IdentifiersInEventSubscribersFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0028IdentifiersInEventSubscribersDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0028");

    public static readonly DiagnosticDescriptor Rule0029CompareDateTimeThroughCodeunit = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0029",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0029CompareDateTimeThroughCodeunitTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0029CompareDateTimeThroughCodeunitFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0029CompareDateTimeThroughCodeunitDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0029");

    public static readonly DiagnosticDescriptor Rule0030AccessInternalForInstallAndUpgradeCodeunits = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0030",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0030AccessInternalForInstallAndUpgradeCodeunitsTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0030AccessInternalForInstallAndUpgradeCodeunitsFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0030AccessInternalForInstallAndUpgradeCodeunitsDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0030");

    public static readonly DiagnosticDescriptor Rule0031RecordInstanceIsolationLevel = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0031",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0031RecordInstanceIsolationLevelTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0031RecordInstanceIsolationLevelFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0031RecordInstanceIsolationLevelDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0031");

    public static readonly DiagnosticDescriptor Rule0032ClearCodeunitSingleInstance = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0032",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0032ClearCodeunitSingleInstanceTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0032ClearCodeunitSingleInstanceFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0032ClearCodeunitSingleInstanceDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0032");

    public static readonly DiagnosticDescriptor Rule0033AppManifestRuntimeBehind = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0033",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0033AppManifestRuntimeBehindTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0033AppManifestRuntimeBehindFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0033AppManifestRuntimeBehindTitleDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0033");

    public static readonly DiagnosticDescriptor Rule0034ExtensiblePropertyShouldAlwaysBeSet = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0034",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0034ExtensiblePropertyShouldAlwaysBeSetTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0034ExtensiblePropertyShouldAlwaysBeSetFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Hidden,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0034ExtensiblePropertyShouldAlwaysBeSetDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0034");

    public static readonly DiagnosticDescriptor Rule0035ExplicitSetAllowInCustomizations = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0035",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0035ExplicitSetAllowInCustomizationsTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0035ExplicitSetAllowInCustomizationsFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0035ExplicitSetAllowInCustomizationsDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0035");

    public static readonly DiagnosticDescriptor Rule0036ToolTipShouldStartWithSpecifies = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0036",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0036ToolTipShouldStartWithSpecifiesTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0036ToolTipShouldStartWithSpecifiesFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0036ToolTipShouldStartWithSpecifiesDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0036");

    public static readonly DiagnosticDescriptor Rule0037ToolTipDoNotUseLineBreaks = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0037",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0037ToolTipDoNotUseLineBreaksTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0037ToolTipDoNotUseLineBreaksFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0037ToolTipDoNotUseLineBreaksDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0037");

    public static readonly DiagnosticDescriptor Rule0038ToolTipMaximumLength = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0038",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0038ToolTipMaximumLengthTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0038ToolTipMaximumLengthFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0038ToolTipMaximumLengthDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0038");

    public static readonly DiagnosticDescriptor Rule0039ArgumentDifferentTypeThenExpected = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0039",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0039ArgumentDifferentTypeThenExpectedTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0039ArgumentDifferentTypeThenExpectedFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0039ArgumentDifferentTypeThenExpectedDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0039");

    public static readonly DiagnosticDescriptor Rule0040ExplicitlySetRunTrigger = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0040",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0040ExplicitlySetRunTriggerTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0040ExplicitlySetRunTriggerFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0040ExplicitlySetRunTriggerDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0040");

    public static readonly DiagnosticDescriptor Rule0041EmptyCaptionLocked = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0041",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0041EmptyCaptionLockedTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0041EmptyCaptionLockedFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0041EmptyCaptionLockedDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0041");

    public static readonly DiagnosticDescriptor Rule0042AutoCalcFieldsOnNormalFields = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0042",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0042AutoCalcFieldsOnNormalFieldsTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0042AutoCalcFieldsOnNormalFieldsFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0042AutoCalcFieldsOnNormalFieldsDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0042");

    public static readonly DiagnosticDescriptor Rule0043SecretText = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0043",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0043SecretTextTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0043SecretTextFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0043SecretTextDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0043");

    public static readonly DiagnosticDescriptor Rule0044AnalyzeTableExtension = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0044",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0044AnalyzeTableExtensionTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0044AnalyzeTableExtensionFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0044AnalyzeTableExtensionDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0044");

    public static readonly DiagnosticDescriptor Rule0044AnalyzeTransferFields = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0044",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0044AnalyzeTransferFieldsTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0044AnalyzeTransferFieldsFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0044AnalyzeTransferFieldsDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0044");

    public static readonly DiagnosticDescriptor Rule0045ZeroEnumValueReservedForEmpty = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0045",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0045ZeroEnumValueReservedForEmptyTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0045ZeroEnumValueReservedForEmptyFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0045ZeroEnumValueReservedForEmptyDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0045");

    public static readonly DiagnosticDescriptor Rule0046TokLabelsLocked = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0046",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0046TokLabelsLockedTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0046TokLabelsLockedFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0046TokLabelsLockedDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0046");

    public static readonly DiagnosticDescriptor Rule0047LockedLabelsTok = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0047",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0047LockedLabelsTokTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0047LockedLabelsTokFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Hidden,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0047LockedLabelsTokDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0047");

    public static readonly DiagnosticDescriptor Rule0048ErrorWithTextConstant = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0048",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0048ErrorWithTextConstantTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0048ErrorWithTextConstantFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0048ErrorWithTextConstantDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0048");

    public static readonly DiagnosticDescriptor Rule0049PageWithoutSourceTable = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0049",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0049PageWithoutSourceTableTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0049PageWithoutSourceTableFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0049PageWithoutSourceTableDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0049");

    public static readonly DiagnosticDescriptor Rule0050OperatorAndPlaceholderInFilterExpression = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0050",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0050OperatorAndPlaceholderInFilterExpressionTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0050OperatorAndPlaceholderInFilterExpressionFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0050OperatorAndPlaceholderInFilterExpressionDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0050");

    public static readonly DiagnosticDescriptor Rule0051PossibleOverflowAssigning = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0051",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0051PossibleOverflowAssigningTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0051PossibleOverflowAssigningFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0051PossibleOverflowAssigningDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0051");

    public static readonly DiagnosticDescriptor Rule0052InternalProceduresNotReferencedAnalyzerDescriptor = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0052",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0052InternalProceduresNotReferencedAnalyzer"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0052InternalProceduresNotReferencedAnalyzerFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0052InternalProceduresNotReferencedAnalyzerDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0052");

    public static readonly DiagnosticDescriptor Rule0053InternalProcedureOnlyUsedInCurrentObjectAnalyzerDescriptor = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0053",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0053InternalProcedureOnlyUsedInCurrentObjectAnalyzer"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0053InternalProcedureOnlyUsedInCurrentObjectAnalyzerFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0053InternalProcedureOnlyUsedInCurrentObjectAnalyzerDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0053");

    public static readonly DiagnosticDescriptor Rule0054FollowInterfaceObjectNameGuide = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0054",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0054FollowInterfaceObjectNameGuideTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0054FollowInterfaceObjectNameGuideFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0054FollowInterfaceObjectNameGuideDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0054");

    public static readonly DiagnosticDescriptor Rule0055TokSuffixForTokenLabels = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0055",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0055TokSuffixForTokenLabelsTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0055TokSuffixForTokenLabelsFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0055TokSuffixForTokenLabelsDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0055");

    public static readonly DiagnosticDescriptor Rule0056EmptyEnumValueWithCaption = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0056",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0056EmptyEnumValueWithCaptionTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0056EmptyEnumValueWithCaptionFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0056EmptyEnumValueWithCaptionDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0056");

    public static readonly DiagnosticDescriptor Rule0057EnumValueWithEmptyCaption = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0057",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0057EnumValueWithEmptyCaptionTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0057EnumValueWithEmptyCaptionFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0057EnumValueWithEmptyCaptionDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0057");

    public static readonly DiagnosticDescriptor Rule0058PageVariableMethodOnTemporaryTable = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0058",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0058PageVariableMethodOnTemporaryTableTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0058PageVariableMethodOnTemporaryTableFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0058PageVariableMethodOnTemporaryTableDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0058");

    public static readonly DiagnosticDescriptor Rule0059SingleQuoteEscapingIssueDetected = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0059",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0059SingleQuoteEscapingIssueDetectedTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0059SingleQuoteEscapingIssueDetectedFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0059SingleQuoteEscapingIssueDetectedDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0059");

    public static readonly DiagnosticDescriptor Rule0060PropertyApplicationAreaOnApiPage = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0060",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0060PropertyApplicationAreaOnApiPageTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0060PropertyApplicationAreaOnApiPageFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0060PropertyApplicationAreaOnApiPageDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0060");

    public static readonly DiagnosticDescriptor Rule0061SetODataKeyFieldsWithSystemIdField = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0061",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0061SetODataKeyFieldsWithSystemIdFieldTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0061SetODataKeyFieldsWithSystemIdFieldFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0061SetODataKeyFieldsWithSystemIdFieldDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0061");

    public static readonly DiagnosticDescriptor Rule0062MandatoryFieldMissingOnApiPage = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0062",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0062MandatoryFieldMissingOnApiPageTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0062MandatoryFieldMissingOnApiPageFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0062MandatoryFieldMissingOnApiPageDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0062");

    public static readonly DiagnosticDescriptor Rule0063GiveFieldMoreDescriptiveName = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0063",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0063GiveFieldMoreDescriptiveNameTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0063GiveFieldMoreDescriptiveNameFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0063GiveFieldMoreDescriptiveNameDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0063");

    public static readonly DiagnosticDescriptor Rule0064TableFieldMissingToolTip = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0064",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0064TableFieldMissingToolTipTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0064TableFieldMissingToolTipFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0064TableFieldMissingToolTipDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0064");

    public static readonly DiagnosticDescriptor Rule0065EventSubscriberVarCheck = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0065",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0065EventSubscriberVarCheckTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0065EventSubscriberVarCheckFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0065EventSubscriberVarCheckDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0065");

    public static readonly DiagnosticDescriptor Rule0066DuplicateToolTipBetweenPageAndTable = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0066",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0066DuplicateToolTipBetweenPageAndTableTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0066DuplicateToolTipBetweenPageAndTableFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info, isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0066DuplicateToolTipBetweenPageAndTableDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0066");


    public static readonly DiagnosticDescriptor Rule0067DisableNotBlankOnSingleFieldPrimaryKey = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0067",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0067DisableNotBlankOnSingleFieldPrimaryKeyTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0067DisableNotBlankOnSingleFieldPrimaryKeyFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0067DisableNotBlankOnSingleFieldPrimaryKeyDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0067");

    public static readonly DiagnosticDescriptor Rule0068CheckObjectPermission = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0068",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0068CheckObjectPermissionTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0068CheckObjectPermissionFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0068CheckObjectPermissionDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0068");

    public static readonly DiagnosticDescriptor Rule0069EmptyStatements = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0069",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0069EmptyStatementsTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0069EmptyStatementsFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0069EmptyStatementsDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0069");

    public static readonly DiagnosticDescriptor Rule0070ListObjectsAreOneBased = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0070",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0070ListObjectsAreOneBasedTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0070ListObjectsAreOneBasedFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0070ListObjectsAreOneBasedDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0070");

    public static readonly DiagnosticDescriptor Rule0071DoNotSetIsHandledToFalse = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0071",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0071DoNotSetIsHandledToFalseTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0071DoNotSetIsHandledToFalseFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0071DoNotSetIsHandledToFalseDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0071");

    public static readonly DiagnosticDescriptor Rule0072CheckProcedureDocumentationComment = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0072",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0072CheckProcedureDocumentationCommentTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0072CheckProcedureDocumentationCommentFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0072CheckProcedureDocumentationCommentDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0072");

    public static readonly DiagnosticDescriptor Rule0073EventPublisherIsHandledByVar = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0073",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0073EventPublisherIsHandledByVarTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0073EventPublisherIsHandledByVarFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0073EventPublisherIsHandledByVarDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0073");

    public static readonly DiagnosticDescriptor Rule0074FlowFilterAssignment = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0074",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0074FlowFilterAssignmentTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0074FlowFilterAssignmentFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0074FlowFilterAssignmentDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0074");

    public static readonly DiagnosticDescriptor Rule0075RecordGetProcedureArguments = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0075",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0075RecordGetProcedureArgumentsTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0075RecordGetProcedureArgumentsFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0075RecordGetProcedureArgumentsDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0075");

    public static readonly DiagnosticDescriptor Rule0076TableRelationTooLong = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0076",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0076TableRelationTooLongTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0076TableRelationTooLongFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0076TableRelationTooLongDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0076");

    public static readonly DiagnosticDescriptor Rule0077UseParenthesisForFunctionCall = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0077",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0077UseParenthesisForFunctionCallTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0077UseParenthesisForFunctionCallFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0077UseParenthesisForFunctionCallDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0077");

    public static readonly DiagnosticDescriptor Rule0078TemporaryRecordsShouldNotTriggerTableTriggers = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0078",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0078TemporaryRecordsTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0078TemporaryRecordsFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0078TemporaryRecordsDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0078");

    public static readonly DiagnosticDescriptor Rule0079NonPublicEventPublisher = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0079",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0079NonPublicEventPublisherTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0079NonPublicEventPublisherFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0079NonPublicEventPublisherDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0079");

    public static readonly DiagnosticDescriptor Rule0080AnalyzeJsonTokenJPath = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0080",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0080AnalyzeJsonTokenJPathTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0080AnalyzeJsonTokenJPathFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0080AnalyzeJsonTokenJPathDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0080");

    public static readonly DiagnosticDescriptor Rule0081UseIsEmptyMethod = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0081",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0081UseIsEmptyMethodTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0081UseIsEmptyMethodFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0081UseIsEmptyMethodDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0081");

    public static readonly DiagnosticDescriptor Rule0082UseQueryOrFindWithNext = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0082",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0082UseQueryOrFindWithNextTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0082UseQueryOrFindWithNextFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0082UseQueryOrFindWithNextDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0082");

    public static readonly DiagnosticDescriptor Rule0083BuiltInDateTimeMethod = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0083",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0083BuiltInDateTimeMethodTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0083BuiltInDateTimeMethodFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0083BuiltInDateTimeMethodDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0083");

    public static readonly DiagnosticDescriptor Rule0084UseReturnValueForErrorHandling = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0084",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0084UseReturnValueForErrorHandlingTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0084UseReturnValueForErrorHandlingFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0084UseReturnValueForErrorHandlingDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0084");

    public static readonly DiagnosticDescriptor Rule0085LFSeparator = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0085",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0085LFSeparatorTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0085LFSeparatorFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0085LFSeparatorDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0085");

    public static readonly DiagnosticDescriptor Rule0086PageStyleDataType = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0086",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0086PageStyleDataTypeTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0086PageStyleDataTypeFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0086PageStyleDataTypeDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0086");

    public static readonly DiagnosticDescriptor Rule0087UseIsNullGuid = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0087",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0087UseIsNullGuidTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0087UseIsNullGuidFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0087UseIsNullGuidDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0087");

    public static readonly DiagnosticDescriptor Rule0088AvoidOptionTypes = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0088",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0088AvoidOptionTypesTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0088AvoidOptionTypesFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0088AvoidOptionTypesDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0088");

    public static readonly DiagnosticDescriptor Rule0089CognitiveComplexity = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0089",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0089CognitiveComplexityTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0089CognitiveComplexityFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: false,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0089CognitiveComplexityDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0089");

    public static readonly DiagnosticDescriptor Rule0089IncrementCognitiveComplexity = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0089i",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0089CognitiveComplexityTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0089IncrementCognitiveComplexityFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: false);

    public static readonly DiagnosticDescriptor Rule0090CognitiveComplexity = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0090",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0089CognitiveComplexityTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0089CognitiveComplexityFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0089CognitiveComplexityDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0090");

    public static readonly DiagnosticDescriptor Rule0091TranslatableTextShouldBeTranslated = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0091",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0091TranslatableTextShouldBeTranslatedTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0091TranslatableTextShouldBeTranslatedFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0091TranslatableTextShouldBeTranslatedDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0091");

    public static readonly DiagnosticDescriptor Rule0092ProcedureNamePattern = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0092",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0092ProcedureNamePatternTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0092ProcedureNamePatternFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0092ProcedureNamePatternDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0092");

    public static readonly DiagnosticDescriptor Rule0093GlobalTestMethodRequiresTestAttribute = new(
        id: LinterCopAnalyzers.AnalyzerPrefix + "0093",
        title: LinterCopAnalyzers.GetLocalizableString("Rule0093GlobalTestMethodRequiresTestAttributeTitle"),
        messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0093GlobalTestMethodRequiresTestAttributeFormat"),
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: LinterCopAnalyzers.GetLocalizableString("Rule0093GlobalTestMethodRequiresTestAttributeDescription"),
        helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0093");
}
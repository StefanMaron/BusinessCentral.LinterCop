# BusinessCentral.LinterCop

[![Github All Releases](https://img.shields.io/github/downloads/StefanMaron/BusinessCentral.LinterCop/total?label=Downloads%20total)]()
[![Github All Releases](https://img.shields.io/github/v/release/StefanMaron/BusinessCentral.LinterCop?label=latest%20version)]()
[![Github All Releases](https://img.shields.io/github/downloads/StefanMaron/BusinessCentral.LinterCop/latest/total?label=downloads%20latest%20version)]()

This code analyzer is meant to check your code for all sorts of problems. Be it code that tecnically compiles but will generate errors during runtime or more a kind of guideline check to achieve cleaner code. Some rule even are disabled by default as they may not go along the main coding guidelines but are maybe helpful in certain projects. In general all rule ideas are welcome, even if they should be and maybe will be covered by Microsoft at some point but could be part of the linter in the meantime.

If you are not happy with some rules or only feel like you need one rule of this analyzer, you can always control the rules with a [Custom.ruleset.json](LinterCop.ruleset.json) and disable all rules you dont need.

## Please Contribute!

The Linter is not finished yet (and probably never will be :D ) If you have any rule on mind that would be nice to be covered, **please start a new [discussion](https://github.com/StefanMaron/BusinessCentral.LinterCop/discussions)!** then we can maybe sharpen the rule a bit if necessary. This way we can build value for all of us. If you want to write the rule yourself you can of course also submit a pull request ;)

### Contribute to Unit Tests

You can also contribute to the collection of unit tests. If you want to add a new test case, please create a new test class in the [BusinessCentral.LinterCop.Test](./BusinessCentral.LinterCop.Test/) folder. The test class name should match the rule name. Use one of the existing test classes as an example.

Test class:

```CSharp
namespace BusinessCentral.LinterCop.Test;

public class RuleXXXX  // Id of the rule that is being tested
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "RuleXXXX");  // Set path to subfolder with test cases
    }

    [Test]
    [TestCase("1")]  // Test case 1.al
    [TestCase("2")]  // Test case 2.al
    ...
    [TestCase("n")]
    public async Task HasDiagnostic(string testCase)  // Positive test
    {
        // Load code file for test case
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        // Create fixture for rule
        var fixture = RoslynFixtureFactory.Create<RuleXXXXMyCodeRule>();
        // Check for reported diagnostic
        fixture.HasDiagnostic(code, DiagnosticDescriptors.RuleXXXXMyCodeRule.Id);
    }

    [Test]
    [TestCase("1")]  // Test case 1.al
    [TestCase("2")]  // Test case 2.al
    ...
    [TestCase("n")]
    public async Task NoDiagnostic(string testCase)  // Negative test
    {
        // Load code file for test case
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        // Create fixture for rule
        var fixture = RoslynFixtureFactory.Create<RuleXXXXMyCodeRule>();
        // Check for no reported diagnostic
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.RuleXXXXMyCodeRule.Id);
    }
}
```

Test cases can be *positive* (a diagnostic is expected) or *negative* (no diagnostic is expected) and are contained in the [BusinessCentral.LinterCop.Test/TestCases/](./BusinessCentral.LinterCop.Test/TestCases/) folder, with a subfolder per rule and a sub-subfolder for positive *HasDiagnostic* and negative *NoDiagnostic* test cases. The individual test cases are numbered (if you have an idea for a better naming scheme, please let me know) and must be compilable AL code.

Basic folder structure:

```bash
├───BusinessCentral.LinterCop.Test
│   ├───RoslynTestKit
│   │   ├───CodeActionLocators
│   │   └───Utils
│   └───TestCases
│       ├───Rule0001
│       │   ├───HasDiagnostic
│       │   │   ├───1.al
│       │   │   ├───2.al
│       │   │   └───X.al
│       │   └───NoDiagnostic
│       │   │   ├───1.al
│       │   │   ├───2.al
│       │   │   └───X.al
│       ├───Rule0002
│       │   ├───HasDiagnostic
│       │   │   ├───X.al
│       │   └───NoDiagnostic
│       │   │   ├───X.al
│       ├───RuleXXXX
│       │   ├───HasDiagnostic
│       │   │   ├───X.al
│       │   └───NoDiagnostic
│       │   │   ├───X.al
├───Rule0001.cs
├───Rule0002.cs
└───RuleXXXX.cs
```

You surround the range in the code you want to check for the diagnostic with `[|` and`|]`.

Test case:

```AL
codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        MyVariable: Integer;
    begin
        // We want to check for a diagnostic between the [| and |] markers
        [|MyVariable := 1;|]
    end;
}
```

## Setup
The LinterCop is compatible with various approaches and solutions for the AL Language extension for Microsoft Dynamics 365 Business Central.

- [Command Line](/.assets/CommandLine.md)
    - [BcContainerHelper](/.assets/CommandLine.md#BcContainerHelper)
- [Visual Studio Code](/.assets/VSCode.md)
- [DevOps](/.assets/DevOps.md)
    - [AL-Go! for GitHub](/.assets/DevOps.md#AL-Go!-for-GitHub)  
    - [BcContainerHelper](/.assets/DevOps.md#BcContainerHelper)
    - [Azure DevOps](/.assets/DevOps.md#Azure-DevOps)

## Configuration

Some rules can be configured by adding a file named `LinterCop.json` in the root of your project.
**Important:** The file will only be read on startup of the linter, meaning if you make any changes you need to reload VS Code once.

For an example and the default values see: [LinterCop.json](LinterCop.json)

If you want to use the `LinterCop.json` file in a pipeline, using BcContainerHelper, you need to copy the file to `C:\build\vsix\extension\bin\win32\LinterCop.json` inside the container before calling `Compile-AppInBcContainer`. See [this issue](https://github.com/StefanMaron/BusinessCentral.LinterCop/issues/263) for details on how to accomplish that.

## Can I disable certain rules?

Since the linter integrates with the AL compiler directly, you can use the custom rule sets like you are used to from the other code cops.
https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/developer/devenv-rule-set-syntax-for-code-analysis-tools

Of course you can also use pragmas for disabling a rule just for a certain place in code.
https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/developer/directives/devenv-directive-pragma-warning

For an example and the default values see: [LinterCop.ruleset.json](LinterCop.ruleset.json)

## Rules

|Id| Title|Default Severity|AL version|
|---|---|---|---|
|[LC0000](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0000)|An error ocurred in a given rule. Please create an issue on GitHub|Info|
|[LC0001](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0001)|FlowFields should not be editable.|Warning|
|[LC0002](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0002)|`Commit()` needs a comment to justify its existence. Either a leading or a trailing comment.|Warning|
|[LC0003](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0003)|Do not use an Object ID for properties or variable declarations.|Warning|
|[LC0004](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0004)|`DrillDownPageId` and `LookupPageId` must be filled in table when table is used in list page.|Warning|
|[LC0005](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0005)|The casing of variable/method usage must align with the definition.|Warning|
|[LC0006](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0006)|Fields with property `AutoIncrement` cannot be used in temporary table (`TableType = Temporary`).|Error|
|[LC0007](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0007)|Every table needs to specify a value for the `DataPerCompany` property. Either `true` or `false`.|Disabled|
|[LC0008](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0008)|Filter operators should not be used in `SetRange`.|Warning|
|[LC0009](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0009)|Show info message about code metrics for each function or trigger.|Disabled|
|[LC0010](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0010)|Show warning about code metrics for each function or trigger if either cyclomatic complexity is 8 or greater or maintainability index 20 or lower.|Warning|
|[LC0011](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0011)|Every object needs to specify a value for the `Access` property. Either `true` or `false`. Optionally this can also be activated for table fields with the setting `enableRule0011ForTableFields`.|Disabled|
|[LC0012](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0012)|Using hardcoded IDs in functions like `Codeunit.Run()` is not allowed.|Warning|
|[LC0013](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0013)|Any table with a single field in the PK of type code or text, should have set `NotBlank` on the PK field.|Warning|
|[LC0014](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0014)|The Caption of permissionset objects should not exceed the maximum length.|Warning|
|[LC0015](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0015)|All application objects should be covered by at least one permission set in the extension.|Warning|
|[LC0016](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0016)|Caption is missing. Optionally this can also be activated for fields on API objects with the setting `enableRule0016ForApiObjects`.|Warning|
|[LC0017](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0017)|Writing to a FlowField is not common. Add a comment to explain this.|Warning|
|[LC0018](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0018)|Events in internal codeunits are not accessible to extensions and should therefore be avoided.|Info|
|[LC0019](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0019)|If Data Classification is set on the Table. Fields do not need the same classification.|Info|
|[LC0020](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0020)|If Application Area is set on the TablePage. Controls do not need the same classification.|Info|
|[LC0021](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0021)|`Confirm()` must be implemented through the `Confirm Management` codeunit from the System Application.|Info|
|[LC0022](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0022)|`GlobalLanguage()` must be implemented through the `Translation Helper` codeunit from the Base Application.|Info|
|[LC0023](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0023)|Always provide fieldsgroups `DropDown` and `Brick` on tables.|Info|
|[LC0024](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0024)|Procedure or Trigger declaration should not end with semicolon.|Info|
|[LC0025](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0025)|Procedure must be either local, internal or define a documentation comment.|Info|
|[LC0026](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0026)|ToolTip must end with a dot.|Info|
|[LC0027](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0027)|Utilize the `Page Management` codeunit for launching page.|Info|
|[LC0028](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0028)|Event subscriber arguments now use identifier syntax instead of string literals.|Info|
|[LC0029](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0029)|Use `CompareDateTime` method in `Type Helper` codeunit for DateTime variable comparisons.|Info|
|[LC0030](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0030)|Set Access property to Internal for Install/Upgrade codeunits.|Info|
|[LC0031](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0031)|Set ReadIsolation property instead of LockTable method.|Info|
|[LC0032](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0032)|Clear(All) does not affect or change values for global variables in single instance codeunits.|Warning|
|[LC0033](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0033)|The specified runtime version in app.json is falling behind.|Info|
|[LC0034](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0034)|The property `Extensible` should be explicitly set for public objects.|Disabled|
|[LC0035](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0035)|Explicitly set `AllowInCustomizations` for fields omitted on pages.|Info|12.1|
|[LC0036](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0036)|ToolTip must start with the verb "Specifies".|Info|
|[LC0037](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0037)|Do not use line breaks in ToolTip.|Info|
|[LC0038](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0038)|Try to not exceed 200 characters (including spaces).|Info|
|[LC0039](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0039)|The given argument has a different type from the one expected.|Warning|
|[LC0040](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0040)|Explicitly set the `RunTrigger` parameter on build-in methods.|Info|
|[LC0041](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0041)|Empty Captions should be `Locked`.|Info|
|[LC0042](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0042)|`AutoCalcFields` should only be used for FlowFields or Blob fields.|Warning|
|[LC0043](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0043)|Use `SecretText` type to protect credentials and sensitive textual values from being revealed.|Info|14.0|
|[LC0044](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0044)|Tables coupled with `TransferFields` must have matching fields.|Warning|
|[LC0045](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0045)|Zero (0) `Enum` value should be reserved for Empty Value.|Info|
|[LC0046](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0046)|`Label` with suffix Tok must be locked.|Info|
|[LC0047](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0047)|Locked `Label` must have a suffix Tok.|None|
|[LC0048](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0048)|Use Error with a `ErrorInfo` or `Label` variable to improve telemetry details.|Info|
|[LC0049](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0049)|`SourceTable` property not defined on Page.|Info|
|[LC0050](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0050)|`SetFilter` with unsupported operator in filter expression.|Info|
|[LC0051](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0051)|Do not assign a text to a target with smaller size.|Warning|12.1|
|[LC0052](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0052)|The internal procedure is declared but never used.|Info|
|[LC0053](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0053)|The internal procedure is only used in the object in which it is declared. Consider making the procedure local.|Info|
|[LC0054](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0054)|Interface name must start with the capital 'I' without any spaces following it.|Info|
|[LC0055](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0055)|The suffix `Tok` is meant to be used when the value of the label matches the name.|Info|
|[LC0056](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0056)|Empty Enum values should not have a specified `Caption` property.|Info|
|[LC0057](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0057)|Enum values must have non-empty a `Caption` to be selectable in the client|Info|
|[LC0058](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0058)|PageVariable.SetRecord(): You cannot use a temporary record for the Record parameter.|Warning|
|[LC0059](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0059)|Single quote escaping issue detected.|Warning|
|[LC0060](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0060)|The `ApplicationArea` property is not applicable to API pages.|Info|
|[LC0061](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0061)|Pages of type API must have the `ODataKeyFields` property set to the SystemId field.|Info|
|[LC0062](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0062)|Mandatory field is missing on API page.|Info|
|[LC0063](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0063)|Consider naming field with a more descriptive name.|Info|
|[LC0064](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0064)|Missing ToolTip property on table field.|Info|
|[LC0065](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0065)|Event subscriber var keyword mismatch.|Info|
|[LC0066](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0066)|Duplicate ToolTip between page and table field.|Info|
|[LC0067](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0067)|Set `NotBlank` property to `false` when 'No. Series' TableRelation exists.|Warning|
|[LC0068](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0068)|Informs the user that there are missing permission to access tabledata.|Warning|

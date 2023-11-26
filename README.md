# BusinessCentral.LinterCop

[![Github All Releases](https://img.shields.io/github/downloads/StefanMaron/BusinessCentral.LinterCop/total?label=Downloads%20total)]()
[![Github All Releases](https://img.shields.io/github/v/release/StefanMaron/BusinessCentral.LinterCop?label=latest%20version)]()
[![Github All Releases](https://img.shields.io/github/downloads/StefanMaron/BusinessCentral.LinterCop/latest/total?label=downloads%20latest%20version)]()

This code analyzer is meant to check your code for all sorts of problems. Be it code that tecnically compiles but will generate errors during runtime or more a kind of guideline check to achieve cleaner code. Some rule even are disabled by default as they may not go along the main coding guidelines but are maybe helpful in certain projects. In general all rule ideas are welcome, even if they should be and maybe will be covered by Microsoft at some point but could be part of the linter in the meantime.

If you are not happy with some rules or only feel like you need one rule of this analyzer, you can always control the rules with a [Custom.ruleset.json](LinterCop.ruleset.json) and disable all rules you dont need.

## Please Contribute!

The Linter is not finished yet (and probably never will be :D ) If you have any rule on mind that would be nice to be covered, **please start a new [discussion](https://github.com/StefanMaron/BusinessCentral.LinterCop/discussions)!** then we can maybe sharpen the rule a bit if necessary. This way we can build value for all of us. If you want to write the rule yourself you can of course also submit a pull request ;)

## How to use

### Manual Compile

1. Download the `BusinessCentral.LinterCop.dll` and place it into your AL Extension folder. For Example `%userprofile%\.vscode\extensions\ms-dynamics-smb.al-7.4.502459\bin\`
2. Run the AL Compiler `. %userprofile%\.vscode\extensions\ms-dynamics-smb.al-7.4.502459\bin\alc.exe /project:"<PathToYourAlProject>" /packagecachepath:"<PathToYour.alpackages>" /analyzer:"userprofile%\.vscode\extensions\ms-dynamics-smb.al-7.4.502459\bin\BusinessCentral.LinterCop.dll"`

### In VS Code

1. Get the [BusinessCentral.LinterCop](https://marketplace.visualstudio.com/items?itemName=StefanMaron.businesscentral-lintercop) VSCode helper extension from the Visual Studio Marketplace
2. Add `"${analyzerfolder}BusinessCentral.LinterCop.dll"` to the `"al.codeAnalyzers"` setting in either user, workspace or folder settings
3. Be aware that folder settings overwrite workspace and workspace overwrite user settings. If you have codecops defined in folder settings, the codecops defined in the user settings won't be applied.

### BcContainerHelper

For manual compile you can use the `Compile-AppInBcContainer` command and pass the path to the `BusinessCentral.LinterCop.dll` in via the parameter `-CustomCodeCops`.

If you are using `Run-ALPipeline` in your build pipelines you can also pass in the `BusinessCentral.LinterCop.dll` in via the parameter `-CustomCodeCops`. To have the correct compiler dependecies you should also load the latest compiler from the marketplace. Add `-vsixFile (Get-LatestAlLanguageExtensionUrl)` to do so.

Be aware though, the `BusinessCentral.LinterCop.dll` needs to be placed in a folder shared with the container.

Further note that you should have BcContainerHelper version 2.0.16 (or newer) installed.

**Tip:** You also can let your build pipelines download the latest version of the `BusinessCentral.LinterCop.dll` via the GitHub API. You can find an example for this in the [DownloadFile.ps1](https://github.com/StefanMaron/vsc-lintercop/blob/master/DownloadFile.ps1) script used by the BusinessCentral.LinterCop VSCode helper extension for automatic updates.

## Rules

|Id| Title|Default Severity|
|---|---|---|
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
|[LC0024](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0024)|Procedure declaration should not end with semicolon.|Info|
|[LC0025](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0025)|Procedure must be either local or internal.|Info|
|[LC0026](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0026)|Guidelines for ToolTip text.|Info|
|[LC0027](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0027)|Utilize the `Page Management` codeunit for launching page.|Info|
|[LC0028](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0028)|Event subscriber arguments now use identifier syntax instead of string literals.|Info|
|[LC0029](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0029)|Use `CompareDateTime` method in `Type Helper` codeunit for DateTime variable comparisons.|Info|
|[LC0030](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0030)|Set Access property to Internal for Install/Upgrade codeunits.|Info|
|[LC0031](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0031)|Set ReadIsolation property instead of LockTable method.|Info|
|[LC0032](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0032)|Clear(All) does not affect or change values for global variables in single instance codeunits.|Warning|
|[LC0033](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0033)|The specified runtime version in app.json is falling behind.|Info|
|[LC0034](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0034)|The property `Extensible` should be explicitly set for public objects.|Disabled|
|[LC0035](https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0035)|Explicitly set `AllowInCustomizations` for fields omitted on pages.|Info|

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


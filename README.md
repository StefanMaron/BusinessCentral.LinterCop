# BusinessCentral.LinterCop

## Please Contribute!

The Linter is not finished yet (and probably never will be :D ) If you have any rule on mind that would be nice to be covered, **please start a new [discussion](https://github.com/StefanMaron/BusinessCentral.LinterCop/discussions)!** then we can maybe sharpen the rule a bit if necessary and I will create an issue afterwards. This way we can build value for all of us. If you want to write the rule yourself you can of course also submit a pull request ;)

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

If you are using `Run-ALPipeline` in your build pipelines you can also pass in the `BusinessCentral.LinterCop.dll` in via the parameter `-CustomCodeCops`.

Be aware though, the `BusinessCentral.LinterCop.dll` needs to be placed in a folder shared with the container.

Further note that you should have BcContainerHelper version 2.0.16 (or newer) installed.

**Tip:** You also can let your build pipelines download the latest version of the `BusinessCentral.LinterCop.dll` via the GitHub API. You can find an example for this in the [DownloadFile.ps1](https://github.com/StefanMaron/vsc-lintercop/blob/master/DownloadFile.ps1) script used by the BusinessCentral.LinterCop VSCode helper extension for automatic updates.

## Rules

|Id| Title|Default Severity|
|---|---|---|
|LC0001|FlowFields should not be editable.|Warning|
|LC0002|`Commit()` needs a comment to justify its existence. Either a leading or a trailing comment.|Warning|
|LC0003|Do not use an Object ID for properties or variable declarations. |Warning|
|LC0004|`DrillDownPageId` and `LookupPageId` must be filled in table when table is used in list page|Warning|
|LC0005|The casing of variable/method usage must align with the definition|Warning|
|LC0006|Fields with property `AutoIncrement` cannot be used in temporary table (`TableType = Temporary`).|Error|
|LC0007|Every table needs to specify a value for the `DataPerCompany` property. Either `true` or `false`|Disabled|
|LC0008|Filter operators should not be used in `SetRange`.|Warning|
|LC0009|Show info message about code metrics for each function or trigger|Disabled|
|LC0010|Show warning about code metrics for each function or trigger if either cyclomatic complexity is 8 or greater or maintainability index 20 or lower|Warning|
|LC0011|Every object needs to specify a value for the `Access` property. Either `true` or `false`. Optionally this can also be activated for table fields with the setting `enableRule0011ForTableFields`|Disabled|

## Configuration

Some rules can be configured by adding a file named `LinterCop.json` in the root of your project.
**Important:** The file will only be read on startup of the linter, meaning if you make any changes you need to reload VS Code once.

For an example and the default values see: [LinterCop.json](LinterCop.json)

## Can I disable certain rules?

Since the linter integrates with the AL compiler directly, you can use the custom rule sets like you are used to from the other code cops.
https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/developer/devenv-rule-set-syntax-for-code-analysis-tools

Of course you can also use pragmas for disabling a rule just for a certain place in code.
https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/developer/directives/devenv-directive-pragma-warning

For an example and the default values see: [LinterCop.ruleset.json](LinterCop.ruleset.json)

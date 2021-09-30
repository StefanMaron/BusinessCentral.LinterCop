# BusinessCentral.LinterCop

## Please Contribute!

The Linter is not finished yet (and probably never will be :D ) If you have any rule on mind that would be nice to be covered, **please start a new [discussion](https://github.com/StefanMaron/BusinessCentral.LinterCop/discussions)!** then we can maybe sharpen the rule a bit if necessary and I will create an issue afterwards. This way we can build value for all of us. If you want to write the rule yourself you can of course also submit a pull request ;)

## How to use

### Manual Compile

1. Download the `BusinessCentral.LinterCop.dll` and place it into your AL Extension folder. For Example `%userprofile%\.vscode\extensions\ms-dynamics-smb.al-7.4.502459\bin\`
2. Run the AL Compiler `. %userprofile%\.vscode\extensions\ms-dynamics-smb.al-7.4.502459\bin\alc.exe /project:"<PathToYourAlProject>" /packagecachepath:"<PathToYour.alpackages>" /analyzer:"userprofile%\.vscode\extensions\ms-dynamics-smb.al-7.4.502459\bin\BusinessCentral.LinterCop.dll"`

### In VS Code

1. Get my helper extension https://marketplace.visualstudio.com/items?itemName=StefanMaron.businesscentral-lintercop
2. Add `"${analyzerfolder}BusinessCentral.LinterCop.dll"` to the `"al.codeAnalyzers"` in either user, workspace or folder settings
3. Be aware that folder settings overwrite workspace and workspace overwrite user settings. If you have codecops defined in folder settings, the codecops defined in the user settings wont be applied

### BCContainerhelper `[Only in preview of bccontainerhelper]`

For manual compile you can use the `Compile-AppInBcContainer` command and pass the path to the `BusinessCentral.LinterCop.dll` in via the parameter `-CustomCodeCops`

If you are using `Run-ALPipeline` in you build pipelines you can also pass in the `BusinessCentral.LinterCop.dll` in via the parameter `-CustomCodeCops`

Be aware tho, the `BusinessCentral.LinterCop.dll` needs to be placed in a folder shared with the container.

## Rules

|Id| Title|Default Severity|
|---|---|---|
|LC0001|FlowFields should not be editable.|Warning|
|LC0002|Commit() needs a comment to justify its existance. Either a leading or a trailing comment.|Warning|
|LC0003|Do not use an Object ID for properties or variables declaration. |Warning|
|LC0004|"DrillDownPageId" and "LookupPageId" must be filled in table when table is used in list page|Warning|
|LC0005|The casing of variable/method usage must allign with the definition|Warning|
|LC0006|Fields with property "AutoIncrement" cannot be used in temporary table (TableType = Temporary).|Error|
|LC0007|Every table needs to specify "DataPerCompany". Either true or false|Disabled|
|LC0008|Filter operators should not be used in SetRange.|Warning|
|LC0009|Show info message about code metrics for each function or trigger|Disabled|
|LC0010|Show warning about code metrics for each function or trigger if either cyclomatic complexity is 8 or greater or maintainability index 20 or lower|Warning|

## Configuration

Some rules can be configured by adding a file named `LinterCop.json` in the root of your project.  
**Important:** The file will only be read on start up of the linter, meaning if you make any changes you need to reload vs code once.

These are the default values:

``` json
{
    "cyclomaticComplexetyThreshold": 8,
    "maintainablityIndexThreshold": 20
}
```

## Can I disable certain rules?

Since the linter integrates with the AL compiler directly, you can use the custom rule sets like you are used to from the other code cops.  
https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/developer/devenv-rule-set-syntax-for-code-analysis-tools

Of cource you can also use pragmas for disabling a rule just for a certain place in code.  
https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/developer/directives/devenv-directive-pragma-warning

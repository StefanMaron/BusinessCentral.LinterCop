# BusinessCentral.LinterCop

## How to use

### Manual Compile

1. Download the `BusinessCentral.LinterCop.dll` and place it into your AL Extension folder. For Example `%userprofile%\.vscode\extensions\ms-dynamics-smb.al-7.4.502459\bin\`
2. Run the AL Compiler `. %userprofile%\.vscode\extensions\ms-dynamics-smb.al-7.4.502459\bin\alc.exe /project:"<PathToYourAlProject>" /packagecachepath:"<PathToYour.alpackages>" /analyzer:"userprofile%\.vscode\extensions\ms-dynamics-smb.al-7.4.502459\bin\BusinessCentral.LinterCop.dll"`

### In VS Code

1. Get my helper extension https://marketplace.visualstudio.com/items?itemName=StefanMaron.businesscentral-lintercop
2. Add `"${analyzerfolder}BusinessCentral.LinterCop.dll"` to the `"al.codeAnalyzers"` in either user, workspace or folder settings
3. Be aware that folder settings overwrite workspace and workspace overwrite user settings. If you have codecops defined in folder settings, the codecops defined in the user settings wont be applied


## Rules

|Id| Title|Default Severity|
|---|---|---|
|LC0001|FlowFields should not be editable.|Warning|
|LC0002|Commit() needs a comment to justify its existance. Either a leading or a trailing comment.|Warning|
|LC0003|Do not use an Object ID for properties or variables declaration. |Warning|

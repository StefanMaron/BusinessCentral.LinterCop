# BusinessCentral.LinterCop

## How to use

For now the only way to run the checks is, to run the al compiler manually via powershell and to pass in the LinterCop as analyzer.

1. Download the `BusinessCentral.LinterCop.dll` and place it into your AL Extension folder. For Example `%userprofile%\.vscode\extensions\ms-dynamics-smb.al-7.4.502459\bin\`
2. Run the AL Compiler `. %userprofile%\.vscode\extensions\ms-dynamics-smb.al-7.4.502459\bin\alc.exe /project:"<PathToYourAlProject>" /packagecachepath:"<PathToYour.alpackages>" /analyzer:"userprofile%\.vscode\extensions\ms-dynamics-smb.al-7.4.502459\bin\BusinessCentral.LinterCop.dll"`

## Rules

|Id| Title|Default Severity|
|---|---|---|
|LC0001|FlowFields should not be editable.|Warining|
|LC0002|Commit() needs a comment to justify its existance. Either a leading or a trailing comment.|Warning|
|LC0003|Do not use an Object ID for properties or variables declaration. [InDevelopment]|Warning|

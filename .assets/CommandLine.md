# Command Line

The LinterCop can be referenced as a code analyser on executing the Business Central AL Compiler from the command line.

## AL Compiler

Download the `BusinessCentral.LinterCop.AL-14.0.983287.dll` from the [releases](https://github.com/StefanMaron/BusinessCentral.LinterCop/releases) and place it into your AL Language extension folder. For Example `%userprofile%\.vscode\extensions\ms-dynamics-smb.al-14.0.983287\bin\`

Run the AL Compiler `. %userprofile%\.vscode\extensions\ms-dynamics-smb.al-14.0.983287\bin\alc.exe /project:"<PathToYourAlProject>" /packagecachepath:"<PathToYour.alpackages>" /analyzer:"userprofile%\.vscode\extensions\ms-dynamics-smb.al-14.0.983287\bin\usinessCentral.LinterCop.AL-14.0.983287.dll"`

**Note:** It's important to match the specific AL version of the LinterCop.dll with the exact same version of the AL Compiler.


## BcContainerHelper

For manual compile you can use the `Compile-AppInBcContainer` command and pass the path or url to the `BusinessCentral.LinterCop.dll` in via the parameter `-CustomCodeCops`. For example `-CustomCodeCops "https://github.com/StefanMaron/BusinessCentral.LinterCop/releases/latest/download/BusinessCentral.LinterCop.dll"`.

If passing the `BusinessCentral.LinterCop.dll` as path instead of a url, be aware that the file needs to be placed in a folder shared with the container. Further note that you should have BcContainerHelper version 6.0.16 (or newer) installed.
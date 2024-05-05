# DevOps

Integrating additional Code Cops, like the LinterCop, in your DevOps workflow will improve the maintainability and transparency of the AL code.

Automate tedious manual checks during pull requestlike checking for coding conventions and identifying possible run-time errors.


## AL-Go for GitHub

[AL-Go! for GitHub](https://github.com/microsoft/AL-Go) is a plug-and-play DevOps solution for Business Central app development on GitHub.

Built-in support for custom Code Cops is available out-of-the-box, allowing for easy configuration of the LinterCop.

![AL-Go settings](/.assets/AL-Go_settings.png)  
https://github.com/microsoft/AL-Go/blob/main/Scenarios/settings.md#advanced-settings

In the [settings.json](https://github.com/microsoft/AL-Go/blob/main/Scenarios/settings.md)  you can specify the url to dll file of the LinterCop, where you also need to enable downloading the latest VSIX.

```json
{
  "vsixFile": "latest",
  "customCodeCops": [
    "https://github.com/StefanMaron/BusinessCentral.LinterCop/releases/latest/download/BusinessCentral.LinterCop.dll"
  ]
}
```

## BcContainerHelper

If you are using `Run-ALPipeline` in your build pipelines you can also pass the path or url to the `BusinessCentral.LinterCop.dll` in via the parameter `-CustomCodeCops`.

```PowerShell
-CustomCodeCops "https://github.com/StefanMaron/BusinessCentral.LinterCop/releases/latest/download/BusinessCentral.LinterCop.dll"
```

If passing the `BusinessCentral.LinterCop.dll` as path instead of a url, be aware that the file needs to be placed in a folder shared with the container. Further note that you should have BcContainerHelper version 6.0.16 (or newer) installed.

To have the correct compiler dependecies you should also load the latest compiler from the marketplace. Add `-vsixFile (Get-LatestAlLanguageExtensionUrl)` to do so.

## Azure DevOps

You can let your build pipelines download the latest version of the `BusinessCentral.LinterCop.dll` via the GitHub API. You can find an example for this in the [DownloadFile.ps1](https://github.com/StefanMaron/vsc-lintercop/blob/master/DownloadFile.ps1) script used by the BusinessCentral.LinterCop VS Code helper extension for automatic updates.
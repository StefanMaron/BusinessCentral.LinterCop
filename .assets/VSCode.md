# Visual Studio Code

Get the [BusinessCentral.LinterCop](https://marketplace.visualstudio.com/items?itemName=StefanMaron.businesscentral-lintercop) VS Code extension from the Visual Studio Code Marketplace.

**Tip:** It's also possible to manually add `"${analyzerfolder}BusinessCentral.LinterCop.dll"` to the `"al.codeAnalyzers"` setting in either user, workspace or folder settings.

# Example

An example `settings.json` where the LinterCop is configured.

```json
  "al.codeAnalyzers": [
    "${CodeCop}",
    "${UICop}",
    "${PerTenantExtensionCop}",
    "${analyzerfolder}BusinessCentral.LinterCop.dll",
  ],
```

**Tip:** Be aware that folder settings overwrite workspace and workspace overwrite user settings. If you have codecops defined in folder settings, the codecops defined in the user settings won't be applied.
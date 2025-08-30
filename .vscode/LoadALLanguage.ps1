# Get latest AL extension VSIX
$listing = Invoke-WebRequest -Method POST -UseBasicParsing -Uri https://marketplace.visualstudio.com/_apis/public/gallery/extensionquery?api-version=3.0-preview.1 -Body '{"filters":[{"criteria":[{"filterType":8,"value":"Microsoft.VisualStudio.Code"},{"filterType":12,"value":"4096"},{"filterType":7,"value":"ms-dynamics-smb.al"}],"pageNumber":1,"pageSize":50,"sortBy":0,"sortOrder":0}],"assetTypes":[],"flags":147}' -ContentType application/json | ConvertFrom-Json;
$vsixUrl = $listing.results.extensions.versions `
| Where-Object properties -ne $null `
| Where-Object { $_.properties.key -notcontains 'Microsoft.VisualStudio.Code.PreRelease' } `
| Select-Object -First 1 -ExpandProperty files `
| Where-Object { $_.assetType -eq 'Microsoft.VisualStudio.Services.VSIXPackage' } `
| Select-Object -ExpandProperty source;

$archivePath = "ALLanguage.vsix"
Invoke-WebRequest $vsixUrl -OutFile $archivePath;

$extractor = Join-Path $PSScriptRoot '..\.github\actions\platform-artifacts\Extract-RequiredFiles.ps1'
& $extractor -DestinationPath 'ALLanguage' -ArchivePath $archivePath -PathInArchive 'extension\bin\Analyzers'

Remove-Item -Path $archivePath -Force -ErrorAction SilentlyContinue
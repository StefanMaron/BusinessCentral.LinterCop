
$PackageId = 'Microsoft.Dynamics.BusinessCentral.Development.Tools'
$IncludePrerelease = $true

# Resolve latest version from NuGet flat container
$indexUrl = "https://api.nuget.org/v3-flatcontainer/$($PackageId.ToLower())/index.json"
try {
    $idx = Invoke-RestMethod -UseBasicParsing -Uri $indexUrl -Method GET -ErrorAction Stop
}
catch {
    throw "Failed to query NuGet index for '$PackageId'. $_"
}

$versions = @($idx.versions)
if (-not $IncludePrerelease) {
    $versions = $versions | Where-Object { $_ -notmatch '-' }
}
if (-not $versions -or $versions.Count -eq 0) { throw "No versions found for $PackageId." }
$version = $versions[-1]

# Setup temporary folder
$tempFolder = Join-Path $env:TEMP ([System.IO.Path]::GetRandomFileName())
Remove-Item -Path $tempFolder -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Path $tempFolder | Out-Null

# Download the .nupkg
$archivePath = "$PackageId.$version.nupkg"
$nupkgUrl = "https://api.nuget.org/v3-flatcontainer/$($PackageId.ToLower())/$version/$($PackageId.ToLower()).$version.nupkg"
Invoke-WebRequest -Uri $nupkgUrl -OutFile $archivePath -UseBasicParsing

$extractor = Join-Path $PSScriptRoot '..\.github\actions\platform-artifacts\Extract-RequiredFiles.ps1'
& $extractor -DestinationPath "ALLanguage" -ArchivePath $archivePath -PathInArchive 'tools\net8.0\any'

Remove-Item -Path $archivePath -Force -ErrorAction SilentlyContinue
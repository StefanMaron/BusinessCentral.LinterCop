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
if (-not $versions -or $versions.Count -eq 0) {
    throw "No versions found for $PackageId."
}

$version = $versions[-1]
$source = "https://api.nuget.org/v3-flatcontainer/$($PackageId.ToLower())/$version/$($PackageId.ToLower()).$version.nupkg"

$listing = [PSCustomObject]@{
    version = $version
    source  = $source
    type    = 'NuGetPackage'
}

Write-Output $listing | ConvertTo-Json -Compress
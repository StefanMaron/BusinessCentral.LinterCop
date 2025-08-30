param(
    [Parameter(Mandatory = $true)]
    [string]$DestinationPath,

    [Parameter(Mandatory = $true)]
    [string]$ArchivePath,

    [Parameter(Mandatory = $true)]
    [string]$PathInArchive
)

# Always clean destination path
Remove-Item -Path $DestinationPath -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Path $DestinationPath | Out-Null

#Add-Type -AssemblyName System.IO.Compression
#Add-Type -AssemblyName System.IO.Compression.FileSystem

# Normalize the archive subpath to ZIP's forward-slash form and ensure trailing slash
$norm = ($PathInArchive -replace '\\', '/').TrimStart('/')
if ($norm.Length -gt 0 -and -not $norm.EndsWith('/')) { $norm += '/' }

# # Detect ExtractToFile overloads
# $has3 = [System.IO.Compression.ZipFileExtensions].GetMethods() |
# Where-Object { $_.Name -eq 'ExtractToFile' -and $_.GetParameters().Length -eq 3 } |
# Select-Object -First 1

$archive = [System.IO.Compression.ZipFile]::OpenRead($ArchivePath)

try {
    # Validate the path exists in the archive
    $exists = $archive.Entries | Where-Object {
        ($_.FullName -replace '\\', '/').StartsWith($norm, [StringComparison]::OrdinalIgnoreCase)
    } | Select-Object -First 1
    if (-not $exists) {
        throw "Path '$PathInArchive' not found in archive '$ArchivePath'."
    }

    foreach ($entry in $archive.Entries) {
        $full = ($entry.FullName -replace '\\', '/')

        if (-not $full.StartsWith($norm, [StringComparison]::OrdinalIgnoreCase)) { continue }
        if ([string]::IsNullOrEmpty($entry.Name)) { continue } # directory pseudo-entries

        # Relative path after the given folder
        $rel = $full.Substring($norm.Length)

        # Guard against traversal
        if ($rel.Contains('..')) { continue }

        $destPath = Join-Path $DestinationPath ($rel -replace '/', '\')
        $destDir = Split-Path $destPath -Parent
        if (-not (Test-Path $destDir)) { New-Item -ItemType Directory -Path $destDir -Force | Out-Null }

        # if ($has3) {
        [System.IO.Compression.ZipFileExtensions]::ExtractToFile($entry, $destPath, $true)
        # }
        # else {
        #     if (Test-Path $destPath) { Remove-Item $destPath -Force }
        #     [System.IO.Compression.ZipFileExtensions]::ExtractToFile($entry, $destPath)
        # }
    }
}
finally {
    $archive.Dispose()
}
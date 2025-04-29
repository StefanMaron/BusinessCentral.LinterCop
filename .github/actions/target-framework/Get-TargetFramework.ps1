# Get-TargetFramework.ps1
Param (
    [Parameter(Mandatory = $true)]
    [string] $version
)

function ConvertTo-Version {
    [OutputType([System.Version])]
    Param (
        [Parameter(Mandatory = $true)]
        [string] $version
    ) 
    
    $result = $null
    if ([System.Version]::TryParse($version, [ref]$result)) {
        return $result
    }
    else {
        Write-Error "The value '$($version)' is not a valid input."
    }
}

function Get-TargetFramework {
    [OutputType([System.String])]
    Param (
        [Parameter(Mandatory = $true)]
        [System.Version] $version
    ) 
    
    # The minimum version that can target .NET 8.0 SDK
    $MinNet8SdkVersion = [Version]"16.0.1463980"

    if ($Version -lt $MinNet8SdkVersion) {
        return "netstandard2.1"
    }
    else {
        return "net8.0"
    }
}

# Convert input version string to System.Version
$version = ConvertTo-Version -version $version

# Call Get-TargetFramework with the converted version
$result = Get-TargetFramework -version $version

# Output the result
Write-Output $result

# Get-FeatureFlags.ps1
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

function Get-FeatureFlags {
    [OutputType([System.String])]
    Param (
        [Parameter(Mandatory = $true)]
        [System.Version] $version
    ) 
    $featureFlags = ""

    $RuntimeVersion = [Ordered]@{
        'Spring2018'       = '1.0'
        'Fall2018'         = '2.0'
        'Spring2019'       = '3.0'
        'Fall2019'         = '4.0'
        'Spring2020'       = '5.0'
        'Fall2020'         = '6.0'
        'Spring2021'       = '7.0'
        'Fall2021'         = '8.0'
        'Spring2022'       = '9.0'
        'Spring2022RV1'    = '9.1'
        'Spring2022RV2'    = '9.2'
        'Fall2022'         = '10.0'
        'Spring2023'       = '11.0'
        'Fall2023'         = '12.0'
        'Fall2023RV1'      = '12.1'
        'Fall2023RV2'      = '12.2'
        'Fall2023RV3'      = '12.3'
        'Spring2024'       = '13.0'
        'Fall2024'         = '14.0'
        'Spring2025'       = '15.0'
        'Fall2025'         = '16.0'

        'ManifestHelper'   = '13.0.937154'
        'PageSystemAction' = '13.0.878831'
    }

    $supportedRuntimeVersions = $RuntimeVersion.GetEnumerator() | Where-Object { $(ConvertTo-Version($_.Value)) -le $(ConvertTo-Version($version)) } | Foreach-Object { $_.Key } | ForEach-Object { "#$_" }
    if (![string]::IsNullOrEmpty($supportedRuntimeVersions)) {
        $featureFlags = [System.String]::Join("", $supportedRuntimeVersions)
    }

    return $featureFlags
}

# Convert input version string to System.Version
$version = ConvertTo-Version -version $version

# Call Get-FeatureFlags with the converted version
$result = Get-FeatureFlags -version $version

# Output the result
Write-Output $result

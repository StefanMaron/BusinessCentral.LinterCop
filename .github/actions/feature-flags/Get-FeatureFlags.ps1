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
        'Spring2018OrLower'    = '1.0'
        'Fall2018OrLower'      = '2.0'
        'Spring2019OrLower'    = '3.0'
        'Fall2019OrLower'      = '4.0'
        'Spring2020OrLower'    = '5.0'
        'Fall2020OrLower'      = '6.0'
        'Spring2021OrLower'    = '7.0'
        'Fall2021OrLower'      = '8.0'
        'Spring2022OrLower'    = '9.0'
        'Spring2022RV1OrLower' = '9.1'
        'Spring2022RV2OrLower' = '9.2'
        'Fall2022OrLower'      = '10.0'
        'Spring2023OrLower'    = '11.0'
        'Fall2023OrLower'      = '12.0'
        'Fall2023RV1OrLower'   = '12.1'
        'Fall2023RV2OrLower'   = '12.2'
        'Fall2023RV3OrLower'   = '12.3'
        'Spring2024OrLower'    = '13.0'
        'Fall2024OrLower'      = '14.0'
        'Spring2025OrLower'    = '15.0'
        'Fall2025OrLower'      = '16.0'
    }

    $supportedRuntimeVersions = $RuntimeVersion.GetEnumerator() | Where-Object { $(ConvertTo-Version($_.Value)) -gt $(ConvertTo-Version($version)) } | Foreach-Object { $_.Key } | ForEach-Object { "#$_" }
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

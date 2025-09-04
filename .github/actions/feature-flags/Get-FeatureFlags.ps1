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
    
    # Remove everything after the first hyphen (including -beta, -alpha, etc.)
    $version = $version -split '-' | Select-Object -First 1

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
        'LessThenSpring2018'    = '1.0'
        'LessThenFall2018'      = '2.0'
        'LessThenSpring2019'    = '3.0'
        'LessThenFall2019'      = '4.0'
        'LessThenSpring2020'    = '5.0'
        'LessThenFall2020'      = '6.0'
        'LessThenSpring2021'    = '7.0'
        'LessThenFall2021'      = '8.0'
        'LessThenSpring2022'    = '9.0'
        'LessThenSpring2022RV1' = '9.1'
        'LessThenSpring2022RV2' = '9.2'
        'LessThenFall2022'      = '10.0'
        'LessThenSpring2023'    = '11.0'
        'LessThenFall2023'      = '12.0'
        'LessThenFall2023RV1'   = '12.1'
        'LessThenFall2023RV2'   = '12.2'
        'LessThenFall2023RV3'   = '12.3'
        'LessThenSpring2024'    = '13.0'
        'LessThenSpring2024RV1' = '13.1'
        'LessThenFall2024'      = '14.0'
        'LessThenFall2024RV1'   = '14.1'
        'LessThenFall2024RV2'   = '14.2'
        'LessThenFall2024RV3'   = '14.3'
        'LessThenSpring2025'    = '15.0'
        'LessThenSpring2025RV1' = '15.1'
        'LessThenFall2025'      = '16.0'
        'LessThenSpring2026'    = '17.0'
        'LessThenFall2026'      = '18.0'
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

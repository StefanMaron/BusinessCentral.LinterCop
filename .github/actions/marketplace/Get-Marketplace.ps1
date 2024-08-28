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

$listing = Invoke-WebRequest -Method POST -UseBasicParsing `
    -Uri https://marketplace.visualstudio.com/_apis/public/gallery/extensionquery?api-version=3.0-preview.1 `
    -Body '{"filters":[{"criteria":[{"filterType":8,"value":"Microsoft.VisualStudio.Code"},{"filterType":12,"value":"4096"},{"filterType":7,"value":"ms-dynamics-smb.al"}],"pageNumber":1,"pageSize":50,"sortBy":0,"sortOrder":0}],"assetTypes":[],"flags":0x192}' `
    -ContentType application/json | ConvertFrom-Json

$listingFiltered = $listing.results | Select-Object -First 1 -ExpandProperty extensions `
| Select-Object -ExpandProperty versions `
| Where-Object { $(ConvertTo-Version($_.version)) -gt [System.Version]::Parse("12.0.0") }

Write-Output $listingFiltered | ConvertTo-Json -Compress -Depth 3
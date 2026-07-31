# Path to the root output folder
$outputRoot = ".\output"
$outputRootPath = (New-Item -ItemType Directory -Force -Path $outputRoot).FullName

# Mapping: Project => Subfolder
$projects = @{
    "ProxyChecker\ProxyChecker.csproj"                                               = ""
    "ProxyChecker.Cli\ProxyChecker.Cli.csproj"                                       = ""
    "ProxyChecker.Loaders.FlashProxyApi\ProxyChecker.Loaders.FlashProxyApi.csproj"   = "Plugins\Loaders\FlashProxyApi"
    "ProxyChecker.Loaders.GeonodeApi\ProxyChecker.Loaders.GeonodeApi.csproj"         = "Plugins\Loaders\GeonodeApi"
    "ProxyChecker.Loaders.GithubIpLocate\ProxyChecker.Loaders.GithubIpLocate.csproj" = "Plugins\Loaders\GithubIpLocate"
    "ProxyChecker.Loaders.UriTextFile\ProxyChecker.Loaders.UriTextFile.csproj"       = "Plugins\Loaders\UriTextFile"
    "ProxyChecker.Checkers.Anonymity\ProxyChecker.Checkers.Anonymity.csproj"         = "Plugins\Checkers\Anonymity"
    "ProxyChecker.Checkers.OkResponse\ProxyChecker.Checkers.OkResponse.csproj"       = "Plugins\Checkers\OkResponse"
    "ProxyChecker.Exporters.UriTextFile\ProxyChecker.Exporters.UriTextFile.csproj"   = "Plugins\Exporters\UriTextFile"
}

# Clear old publication results
if (Test-Path $outputRoot) { Remove-Item -Recurse -Force $outputRoot }

Write-Host "Publishing projects..." -ForegroundColor Cyan

dotnet publish -v quiet

# Publication cycle
foreach ($proj in $projects.Keys) {
    $subFolder = $projects[$proj]
    $targetPath = Join-Path $outputRootPath $subFolder
    
    $projDir = Split-Path $proj -Parent
    $publishSource = Get-ChildItem -Path "$projDir\bin\Release\**\publish" -Directory | Select-Object -First 1

    if ($null -eq $publishSource) {
        Write-Error "Unable to find compiled filed for $proj"
        continue
    }

    # Copying ready files into folders structure
    Write-Host "Copying files from $($publishSource.FullName) to $targetPath..." -ForegroundColor DarkCyan
    New-Item -ItemType Directory -Force -Path $targetPath | Out-Null
    Copy-Item -Path "$($publishSource.FullName)\*" -Destination $targetPath -Recurse -Force
}

Write-Host "All projects are published." -ForegroundColor Green
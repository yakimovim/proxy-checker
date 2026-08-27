$ErrorActionPreference = 'Stop'
$toolsDir = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url = 'https://github.com/yakimovim/proxy-checker/releases/download/v1.0.2/ProxyChecker.1.0.2.win-x64.zip'

$packageArgs = @{
  packageName   = $env:ChocolateyPackageName
  unzipLocation = $toolsDir
  url           = $url
  
  checksum      = 'A4CCE5E71F8D2553D4A32E8B0A5B5F44C7F2BADA129E739C29D64E9865AEF7BB'
  checksumType  = 'sha256'
}

Install-ChocolateyZipPackage @packageArgs

$targetFile = "$toolsDir\ProxyChecker.exe"
$iconFile = "$toolsDir\ProxyChecker.exe"
$desktopPath = [Environment]::GetFolderPath('CommonDesktopDirectory')
Install-ChocolateyShortcut -ShortcutFilePath "$desktopPath\ProxyChecker.lnk" `
  -TargetPath "$targetFile" `
  -IconLocation "$iconFile" `
  -WorkingDirectory "$toolsDir" `
  -Description "Proxy Checker"

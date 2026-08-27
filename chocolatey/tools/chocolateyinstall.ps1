$ErrorActionPreference = 'Stop'
$toolsDir = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url = 'https://github.com/yakimovim/proxy-checker/releases/download/v1.0.3/ProxyChecker.1.0.3.win-x64.zip'

$packageArgs = @{
  packageName   = $env:ChocolateyPackageName
  unzipLocation = $toolsDir
  url           = $url
  
  checksum      = '1A4255CD64B2DDD212BB9BDE4E8A532B672828FDC026B27F68B685428EA8D828'
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

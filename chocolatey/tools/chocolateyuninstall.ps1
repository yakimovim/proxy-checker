$ErrorActionPreference = 'Stop' # stop on all errors

$desktopPath = [Environment]::GetFolderPath('CommonDesktopDirectory')
$shortcutPath = "$desktopPath\ProxyChecker.lnk"
if (Test-Path $shortcutPath) {
  Remove-Item $shortcutPath -Force
}
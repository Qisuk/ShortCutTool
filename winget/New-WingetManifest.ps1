<#
.SYNOPSIS
    Generates the WinGet manifests for a published ShortCutTool release and validates them.

.DESCRIPTION
    Downloads the setup exe from the GitHub release (the hash must match the published asset,
    not a local rebuild), then writes the version, installer and en-US locale manifests in the
    layout microsoft/winget-pkgs expects:

        artifacts\winget\manifests\q\Qisuk\ShortCutTool\<version>\

    Used for the first submission. Later versions are submitted by the Release workflow with
    wingetcreate, which carries this metadata forward.

.PARAMETER Version
    Released version to describe. Defaults to <Version> in ShortCutTool.csproj.

.PARAMETER InstallerSha256
    Skip the download and use this hash, e.g. to check the manifest format before the release
    exists. The release date then defaults to today.

.EXAMPLE
    .\winget\New-WingetManifest.ps1 -Version 1.1.0
#>
param(
    [string]$Version,
    [string]$InstallerSha256
)

$ErrorActionPreference = 'Stop'

$root = Split-Path $PSScriptRoot -Parent
if (-not $Version) {
    $Version = (dotnet msbuild (Join-Path $root 'ShortCutTool.csproj') -getProperty:Version).Trim()
}

$packageId = 'Qisuk.ShortCutTool'
$manifestVersion = '1.10.0'
$repoUrl = 'https://github.com/Qisuk/ShortCutTool'
$installerName = "ShortCutTool-$Version-win-x64-setup.exe"
$installerUrl = "$repoUrl/releases/download/v$Version/$installerName"
# Must match AppGuid in installer\ShortCutTool.iss
$productCode = '{681BDEBB-6E4C-451C-A509-91C4D9F71A02}_is1'

$outDir = Join-Path $root "artifacts\winget\manifests\q\Qisuk\ShortCutTool\$Version"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

if ($InstallerSha256) {
    $sha256 = $InstallerSha256.ToUpperInvariant()
    $releaseDate = (Get-Date).ToString('yyyy-MM-dd')
} else {
    Write-Host "Downloading $installerUrl"
    $download = Join-Path ([IO.Path]::GetTempPath()) $installerName
    Invoke-WebRequest -Uri $installerUrl -OutFile $download -UseBasicParsing
    $sha256 = (Get-FileHash $download -Algorithm SHA256).Hash
    Remove-Item $download

    $release = Invoke-RestMethod -Uri "https://api.github.com/repos/Qisuk/ShortCutTool/releases/tags/v$Version" -UseBasicParsing
    $releaseDate = ([datetime]$release.published_at).ToString('yyyy-MM-dd')
}

$manifests = @{
    "$packageId.yaml" = @"
# yaml-language-server: `$schema=https://aka.ms/winget-manifest.version.$manifestVersion.schema.json

PackageIdentifier: $packageId
PackageVersion: $Version
DefaultLocale: en-US
ManifestType: version
ManifestVersion: $manifestVersion
"@

    "$packageId.installer.yaml" = @"
# yaml-language-server: `$schema=https://aka.ms/winget-manifest.installer.$manifestVersion.schema.json

PackageIdentifier: $packageId
PackageVersion: $Version
InstallerType: inno
Scope: user
InstallModes:
  - interactive
  - silent
  - silentWithProgress
UpgradeBehavior: install
Dependencies:
  PackageDependencies:
    - PackageIdentifier: Microsoft.DotNet.DesktopRuntime.10
ProductCode: '$productCode'
ReleaseDate: $releaseDate
AppsAndFeaturesEntries:
  - DisplayName: ShortCut Tool
    Publisher: Qisuk
    ProductCode: '$productCode'
Installers:
  - Architecture: x64
    InstallerUrl: $installerUrl
    InstallerSha256: $sha256
ManifestType: installer
ManifestVersion: $manifestVersion
"@

    "$packageId.locale.en-US.yaml" = @"
# yaml-language-server: `$schema=https://aka.ms/winget-manifest.defaultLocale.$manifestVersion.schema.json

PackageIdentifier: $packageId
PackageVersion: $Version
PackageLocale: en-US
Publisher: Qisuk
PublisherUrl: https://github.com/Qisuk
PublisherSupportUrl: $repoUrl/issues
PackageName: ShortCut Tool
PackageUrl: $repoUrl
License: MIT
LicenseUrl: $repoUrl/blob/master/LICENSE
ShortDescription: Launch and cycle through application windows with Meh (Ctrl+Alt+Shift) and Hyper (Ctrl+Alt+Shift+Win) keyboard shortcuts.
Description: |-
  ShortCut Tool is a lightweight system tray app that maps Meh + key (Ctrl+Alt+Shift) to an application:
  the first press launches it or brings it to the front, further presses cycle through its windows,
  and Hyper + key (Ctrl+Alt+Shift+Win) cycles backwards. A popup near the tray shows the window list while cycling.
  Shortcuts are managed from the tray menu and stored per user.
Moniker: shortcuttool
Tags:
  - hotkey
  - hyper-key
  - keyboard-shortcuts
  - launcher
  - meh-key
  - productivity
  - tray
  - window-switcher
ReleaseNotesUrl: $repoUrl/releases/tag/v$Version
ManifestType: defaultLocale
ManifestVersion: $manifestVersion
"@
}

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
foreach ($name in $manifests.Keys) {
    [IO.File]::WriteAllText((Join-Path $outDir $name), ($manifests[$name] -replace "`r`n", "`n") + "`n", $utf8NoBom)
}

Write-Host "SHA256: $sha256"
Write-Host "Manifests: $outDir"

winget validate --manifest $outDir
if ($LASTEXITCODE -ne 0) { throw "winget validate failed with exit code $LASTEXITCODE" }

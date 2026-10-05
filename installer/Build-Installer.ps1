<#
.SYNOPSIS
    Publishes ShortCutTool and builds the release artifacts: the Inno Setup installer,
    a portable ZIP and SHA256SUMS.txt. Used by the CI and Release workflows and locally.

.PARAMETER Version
    Version to build. Defaults to <Version> in ShortCutTool.csproj.

.PARAMETER OutputDir
    Where artifacts are written. Defaults to .\artifacts in the repository root.

.EXAMPLE
    .\installer\Build-Installer.ps1
#>
param(
    [string]$Version,
    [string]$OutputDir
)

$ErrorActionPreference = 'Stop'

$root = Split-Path $PSScriptRoot -Parent
$project = Join-Path $root 'ShortCutTool.csproj'

if (-not $Version) {
    $Version = (dotnet msbuild $project -getProperty:Version).Trim()
}
if (-not $OutputDir) {
    $OutputDir = Join-Path $root 'artifacts'
}

# Windows file versions are numeric only, so 1.2.0-beta.1 becomes 1.2.0
$fileVersion = ($Version -split '[-+]')[0]

function Find-Iscc {
    $command = Get-Command iscc.exe -ErrorAction SilentlyContinue
    if ($command) { return $command.Source }

    $candidates = foreach ($base in @(${env:ProgramFiles(x86)}, $env:ProgramFiles, (Join-Path $env:LOCALAPPDATA 'Programs'))) {
        if ($base) { Get-ChildItem -Path $base -Filter 'Inno Setup*' -Directory -ErrorAction SilentlyContinue }
    }
    $iscc = $candidates |
        Sort-Object Name -Descending |
        ForEach-Object { Join-Path $_.FullName 'ISCC.exe' } |
        Where-Object { Test-Path $_ } |
        Select-Object -First 1

    if (-not $iscc) {
        throw 'Inno Setup (ISCC.exe) not found. Install it with: winget install JRSoftware.InnoSetup'
    }
    return $iscc
}

$iscc = Find-Iscc
$publishDir = Join-Path $OutputDir 'publish'

Write-Host "Building ShortCutTool $Version (file version $fileVersion)"
Write-Host "Inno Setup: $iscc"

if (Test-Path $OutputDir) {
    Remove-Item $OutputDir -Recurse -Force
}

dotnet publish $project -c Release -r win-x64 --self-contained false -o $publishDir "-p:Version=$Version"
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed with exit code $LASTEXITCODE" }

& $iscc /Q "/DAppVersion=$Version" "/DAppFileVersion=$fileVersion" "/DPublishDir=$publishDir" "/DOutputDir=$OutputDir" (Join-Path $PSScriptRoot 'ShortCutTool.iss')
if ($LASTEXITCODE -ne 0) { throw "ISCC failed with exit code $LASTEXITCODE" }

$zip = Join-Path $OutputDir "ShortCutTool-$Version-win-x64.zip"
Compress-Archive -Path (Join-Path $publishDir '*') -DestinationPath $zip

$setup = Join-Path $OutputDir "ShortCutTool-$Version-win-x64-setup.exe"
$sums = foreach ($file in @($setup, $zip)) {
    "$((Get-FileHash $file -Algorithm SHA256).Hash)  $(Split-Path $file -Leaf)"
}
$sums | Set-Content (Join-Path $OutputDir 'SHA256SUMS.txt') -Encoding ascii

Write-Host ''
Write-Host 'Artifacts:'
$sums | ForEach-Object { Write-Host "  $_" }

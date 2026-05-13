# Create-WingetRelease.ps1
# Helper script to create a winget-ready release

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,

    [Parameter(Mandatory=$false)]
    [string]$OutputPath = ".\release"
)

Write-Host "Creating ShortCutTool Release v$Version for Winget" -ForegroundColor Green

# 1. Clean and build
Write-Host "`n1. Building Release..." -ForegroundColor Cyan
dotnet clean -c Release
dotnet publish -c Release -o "$OutputPath\publish" --self-contained false -r win-x64

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# 2. Create zip file
Write-Host "`n2. Creating ZIP package..." -ForegroundColor Cyan
$zipPath = "$OutputPath\ShortCutTool-v$Version.zip"
if (Test-Path $zipPath) {
    Remove-Item $zipPath
}
Compress-Archive -Path "$OutputPath\publish\*" -DestinationPath $zipPath

# 3. Calculate SHA256
Write-Host "`n3. Calculating SHA256 hash..." -ForegroundColor Cyan
$hash = (Get-FileHash -Path $zipPath -Algorithm SHA256).Hash
Write-Host "SHA256: $hash" -ForegroundColor Yellow

# 4. Update manifest with hash
Write-Host "`n4. Updating installer manifest..." -ForegroundColor Cyan
$installerManifest = ".winget\Qisuk.ShortCutTool.installer.yaml"
if (Test-Path $installerManifest) {
    $content = Get-Content $installerManifest -Raw
    $content = $content -replace '<INSERT_SHA256_HASH_HERE>', $hash
    $content = $content -replace 'PackageVersion: .*', "PackageVersion: $Version"
    $content = $content -replace 'v\d+\.\d+\.\d+', "v$Version"
    $content = $content -replace 'ReleaseDate: .*', "ReleaseDate: $((Get-Date).ToString('yyyy-MM-dd'))"
    Set-Content $installerManifest $content
    Write-Host "Updated $installerManifest" -ForegroundColor Green
}

# Update version manifest
$versionManifest = ".winget\Qisuk.ShortCutTool.yaml"
if (Test-Path $versionManifest) {
    $content = Get-Content $versionManifest -Raw
    $content = $content -replace 'PackageVersion: .*', "PackageVersion: $Version"
    Set-Content $versionManifest $content
    Write-Host "Updated $versionManifest" -ForegroundColor Green
}

# Update locale manifest
$localeManifest = ".winget\Qisuk.ShortCutTool.locale.en-US.yaml"
if (Test-Path $localeManifest) {
    $content = Get-Content $localeManifest -Raw
    $content = $content -replace 'PackageVersion: .*', "PackageVersion: $Version"
    $content = $content -replace 'v\d+\.\d+\.\d+', "v$Version"
    Set-Content $localeManifest $content
    Write-Host "Updated $localeManifest" -ForegroundColor Green
}

# 5. Show next steps
Write-Host "`n✅ Release package created successfully!" -ForegroundColor Green
Write-Host "`nRelease package: $zipPath" -ForegroundColor Yellow
Write-Host "SHA256: $hash" -ForegroundColor Yellow
Write-Host "`nNext steps:" -ForegroundColor Cyan
Write-Host "1. Create a GitHub Release at:" -ForegroundColor White
Write-Host "   https://github.com/Qisuk/ShortCutTool/releases/new" -ForegroundColor Gray
Write-Host "2. Tag: v$Version" -ForegroundColor White
Write-Host "3. Upload: $zipPath" -ForegroundColor White
Write-Host "4. Follow WINGET_SUBMISSION.md for winget submission" -ForegroundColor White
Write-Host "`nTo validate manifests:" -ForegroundColor Cyan
Write-Host "   winget validate --manifest .winget\" -ForegroundColor Gray

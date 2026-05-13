# Validate-WingetManifests.ps1
# Validates winget manifest files before submission

Write-Host "Validating ShortCutTool Winget Manifests..." -ForegroundColor Green

$manifestPath = ".winget"

if (-not (Test-Path $manifestPath)) {
    Write-Host "❌ Manifest folder not found: $manifestPath" -ForegroundColor Red
    exit 1
}

# Check if winget is installed
try {
    $wingetVersion = winget --version
    Write-Host "✅ Winget version: $wingetVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Winget is not installed. Install it from Microsoft Store." -ForegroundColor Red
    exit 1
}

# Check required files
$requiredFiles = @(
    "Qisuk.ShortCutTool.yaml",
    "Qisuk.ShortCutTool.installer.yaml",
    "Qisuk.ShortCutTool.locale.en-US.yaml"
)

Write-Host "`nChecking required files..." -ForegroundColor Cyan
foreach ($file in $requiredFiles) {
    $filePath = Join-Path $manifestPath $file
    if (Test-Path $filePath) {
        Write-Host "✅ Found: $file" -ForegroundColor Green
    } else {
        Write-Host "❌ Missing: $file" -ForegroundColor Red
        exit 1
    }
}

# Check for placeholder SHA256
Write-Host "`nChecking for placeholders..." -ForegroundColor Cyan
$installerContent = Get-Content (Join-Path $manifestPath "Qisuk.ShortCutTool.installer.yaml") -Raw
if ($installerContent -match '<INSERT_SHA256_HASH_HERE>') {
    Write-Host "⚠️  SHA256 hash placeholder found. Run Create-WingetRelease.ps1 first!" -ForegroundColor Yellow
} else {
    Write-Host "✅ SHA256 hash is set" -ForegroundColor Green
}

# Validate manifests using winget
Write-Host "`nValidating manifest syntax..." -ForegroundColor Cyan
try {
    winget validate --manifest $manifestPath
    if ($LASTEXITCODE -eq 0) {
        Write-Host "`n✅ All manifests are valid!" -ForegroundColor Green
        Write-Host "`nReady for submission to winget-pkgs repository." -ForegroundColor Cyan
        Write-Host "Follow WINGET_SUBMISSION.md for next steps." -ForegroundColor White
    } else {
        Write-Host "`n❌ Validation failed. Fix errors above." -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "❌ Validation failed: $_" -ForegroundColor Red
    exit 1
}

# Summary
Write-Host "`n📋 Manifest Summary:" -ForegroundColor Cyan
$versionContent = Get-Content (Join-Path $manifestPath "Qisuk.ShortCutTool.yaml") -Raw
if ($versionContent -match 'PackageVersion:\s*(\S+)') {
    Write-Host "Package: Qisuk.ShortCutTool" -ForegroundColor White
    Write-Host "Version: $($matches[1])" -ForegroundColor White
}

$localeContent = Get-Content (Join-Path $manifestPath "Qisuk.ShortCutTool.locale.en-US.yaml") -Raw
if ($localeContent -match 'ShortDescription:\s*(.+)') {
    Write-Host "Description: $($matches[1].Trim())" -ForegroundColor White
}

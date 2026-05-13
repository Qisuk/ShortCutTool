# ConvertTo-Icon.ps1
# Converts PNG images to multi-resolution .ico files

param(
	[Parameter(Mandatory=$true)]
	[string]$InputImage,

	[Parameter(Mandatory=$false)]
	[string]$OutputIcon = "app.ico",

	[Parameter(Mandatory=$false)]
	[int[]]$Sizes = @(16, 32, 48, 256)
)

Write-Host "Converting $InputImage to multi-resolution .ico file..." -ForegroundColor Cyan

# Check if input file exists
if (-not (Test-Path $InputImage)) {
	Write-Error "Input image file not found: $InputImage"
	exit 1
}

# Load System.Drawing assembly
Add-Type -AssemblyName System.Drawing

try {
	# Load the source image
	$sourceImage = [System.Drawing.Image]::FromFile((Resolve-Path $InputImage).Path)
	Write-Host "Loaded source image: $($sourceImage.Width)x$($sourceImage.Height) pixels" -ForegroundColor Green

	# Create a memory stream to hold the .ico data
	$memoryStream = New-Object System.IO.MemoryStream
	$binaryWriter = New-Object System.IO.BinaryWriter($memoryStream)

	# .ico file header
	$binaryWriter.Write([UInt16]0)      # Reserved (must be 0)
	$binaryWriter.Write([UInt16]1)      # Image type (1 = .ico)
	$binaryWriter.Write([UInt16]$Sizes.Count)  # Number of images

	$imageDataStreams = @()
	$offset = 6 + ($Sizes.Count * 16)  # Header + directory entries

	foreach ($size in $Sizes) {
		Write-Host "  Generating ${size}x${size} icon..." -ForegroundColor Gray

		# Create resized bitmap
		$bitmap = New-Object System.Drawing.Bitmap($size, $size)
		$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
		$graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
		$graphics.DrawImage($sourceImage, 0, 0, $size, $size)
		$graphics.Dispose()

		# Save to memory stream as PNG (better quality than BMP)
		$imageStream = New-Object System.IO.MemoryStream
		$bitmap.Save($imageStream, [System.Drawing.Imaging.ImageFormat]::Png)
		$imageData = $imageStream.ToArray()
		$imageStream.Dispose()
		$bitmap.Dispose()

		# Write directory entry
		$binaryWriter.Write([byte]$size)           # Width (0 = 256)
		$binaryWriter.Write([byte]$size)           # Height (0 = 256)
		$binaryWriter.Write([byte]0)               # Color palette (0 = no palette)
		$binaryWriter.Write([byte]0)               # Reserved
		$binaryWriter.Write([UInt16]1)             # Color planes
		$binaryWriter.Write([UInt16]32)            # Bits per pixel
		$binaryWriter.Write([UInt32]$imageData.Length)  # Image data size
		$binaryWriter.Write([UInt32]$offset)       # Offset to image data

		$imageDataStreams += $imageData
		$offset += $imageData.Length
	}

	# Write all image data
	foreach ($imageData in $imageDataStreams) {
		$binaryWriter.Write($imageData)
	}

	# Save to file
	$outputPath = Join-Path $PSScriptRoot $OutputIcon
	[System.IO.File]::WriteAllBytes($outputPath, $memoryStream.ToArray())

	$binaryWriter.Dispose()
	$memoryStream.Dispose()
	$sourceImage.Dispose()

	Write-Host "`n✓ Successfully created $OutputIcon with $($Sizes.Count) sizes: $($Sizes -join ', ') pixels" -ForegroundColor Green
	Write-Host "  Output: $outputPath" -ForegroundColor Cyan

} catch {
	Write-Error "Failed to create icon: $_"
	exit 1
}

# Show next steps
Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "1. Verify the icon looks good (open in File Explorer or icon editor)"
Write-Host "2. Rebuild the ShortCutTool project"
Write-Host "3. Check the executable icon in bin\Release\net10.0-windows\publish\"
Write-Host "`nTo rebuild: dotnet build -c Release" -ForegroundColor Gray

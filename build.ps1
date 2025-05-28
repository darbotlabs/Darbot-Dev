# Build script for AIDevGallery that automatically detects and uses the correct architecture
# This script avoids the "AnyCPU" build issue with WinUI 3 applications

# Detect the system architecture
$arch = if ([System.Environment]::Is64BitOperatingSystem) { 
    if ($env:PROCESSOR_ARCHITECTURE -eq "ARM64") { "ARM64" } else { "x64" }
} else { "x86" }

Write-Host "Building for architecture: $arch" -ForegroundColor Cyan
Write-Host "Starting build process..." -ForegroundColor Yellow

# Build with the specific platform
dotnet build AIDevGallery.sln -c Debug -p:Platform=$arch

# Check if build was successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Build completed successfully!" -ForegroundColor Green
    
    # Optional: Run the application
    $runApp = Read-Host "Do you want to run the application? (y/n)"
    if ($runApp -eq "y") {
        Write-Host "Starting AIDevGallery..." -ForegroundColor Yellow
        dotnet run --project AIDevGallery -c Debug -p:Platform=$arch --no-build
    }
} else {
    Write-Host "Build failed with exit code $LASTEXITCODE" -ForegroundColor Red
}

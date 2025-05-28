## 🏆 Quest Objective
Successfully build and run the AI Dev Gallery application on a fresh Windows machine with zero errors or warnings, following a step-by-step, unambiguous process.

---

## 📋 Pre-Quest Requirements Checklist
**Complete ALL items before starting Level 1:**
- [ ] Windows 10 version 1809 (Build 17763) or newer (Windows 11 recommended)
- [ ] 16 GB RAM minimum
- [ ] 20 GB free disk space
- [ ] GPU with 8GB VRAM (recommended for AI features)
- [ ] Visual Studio 2022 (latest version) installed
- [ ] Git installed and configured (https://git-scm.com/download/win)
- [ ] .NET 9.0 SDK installed (see Level 1 for details)
- [ ] (Optional) Visual Studio Code installed for editing (https://code.visualstudio.com/)

---

## 🎯 Level 1: Environment Setup (100 XP)

### Tasks:
1. **Open Visual Studio Installer** (10 XP)
   - Search Windows Start Menu for "Visual Studio Installer"
   - Right-click and select "Run as Administrator"

2. **Install Required Workloads** (40 XP)
   - Click "Modify" on your Visual Studio 2022 installation
   - Under "Workloads", check:
     - ✅ .NET Desktop Development
     - ✅ Desktop development with C++
     - ✅ Universal Windows Platform development
   - Under "Individual components", ensure these are checked:
     - ✅ Windows 11 SDK (10.0.22621.0) (or latest available)
     - ✅ .NET 9.0 Runtime
     - ✅ NuGet package manager
   - Click "Modify" to apply changes and wait for installation to finish

3. **Install .NET 9.0 SDK** (20 XP)
   - Open PowerShell as Administrator
   - Run:
     ```powershell
     winget install Microsoft.DotNet.SDK.9
     ```
   - If winget is not available, download from https://dotnet.microsoft.com/en-us/download/dotnet/9.0

4. **Verify Installation** (30 XP)
   - In PowerShell, run:
     ```powershell
     dotnet --version
     # Expected output: 9.0.x
     ```
   - If not found, restart your machine and try again.

### 🔍 Validation Round 1 (Bonus: 50 XP if no errors)
```powershell
# Run these commands:
dotnet --list-sdks | Select-String "9.0"
where.exe git
code --version  # If VS Code is installed
```
**✅ Success Criteria:** All commands return valid outputs

---

## 🎯 Level 2: Repository Setup (150 XP)

### Tasks:
1. **Create Project Directory** (10 XP)
   - In PowerShell:
     ```powershell
     mkdir C:\GitProjects
     cd C:\GitProjects
     ```

2. **Clone Repository** (30 XP)
   - Run:
     ```powershell
     git clone https://github.com/darbotlabs/darbot-dev.git
     cd darbot-dev
     ```

3. **Verify Repository Structure** (20 XP)
   - Run:
     ```powershell
     dir -Name | Select-String "AIDevGallery.sln|README.md|.pipelines|AIDevGallery"
     ```
   - Ensure all key files and folders are present.

4. **Check Git Status** (10 XP)
   - Run:
     ```powershell
     git status
     git branch -a
     ```
   - Confirm you are on the correct branch (usually main or master).

5. **Verify Solution File** (30 XP)
   - Run:
     ```powershell
     Test-Path "AIDevGallery.sln"
     # Expected: True
     ```

### 🔍 Validation Round 2 (Bonus: 50 XP if no errors)
```powershell
# Verify critical project files exist
$requiredFiles = @(
    "AIDevGallery.sln",
    "Directory.Build.props",
    "Directory.Packages.props",
    "nuget.config",
    "version.json",
    "AIDevGallery\AIDevGallery.csproj",
    "AIDevGallery\Package.appxmanifest"
)
foreach ($file in $requiredFiles) {
    if (Test-Path $file) {
        Write-Host "✅ $file exists" -ForegroundColor Green
    } else {
        Write-Host "❌ $file missing" -ForegroundColor Red
    }
}
```
**✅ Success Criteria:** All files exist

---

## 🎯 Level 3: NuGet & Dependencies Setup (200 XP)

### Tasks:
1. **Configure NuGet Sources** (40 XP)
   - Run:
     ```powershell
     dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org -p 1 --store-password-in-clear-text
     dotnet nuget list source
     ```
   - Ensure only one entry for nuget.org is enabled and points to https://api.nuget.org/v3/index.json

2. **Clear NuGet Cache** (20 XP)
   - Run:
     ```powershell
     dotnet nuget locals all --clear
     ```

3. **Install NBGV Tool** (40 XP)
   - Run:
     ```powershell
     dotnet tool install -g nbgv
     nbgv --version
     ```
   - If already installed, update with:
     ```powershell
     dotnet tool update -g nbgv
     ```

4. **Set Version** (30 XP)
   - In the repository root, run:
     ```powershell
     nbgv cloud -c
     ```

5. **Restore Solution Dependencies** (70 XP)
   - Detect system architecture:
     ```powershell
     $arch = if ([System.Environment]::Is64BitOperatingSystem) { 
         if ($env:PROCESSOR_ARCHITECTURE -eq "ARM64") { "ARM64" } else { "x64" }
     } else { "x86" }
     Write-Host "Detected architecture: $arch" -ForegroundColor Cyan
     ```
   - Restore for detected architecture:
     ```powershell
     dotnet restore AIDevGallery.sln -r win-$arch /p:Configuration=Release /p:Platform=$arch
     ```
   - **If you see any errors or warnings, resolve them before proceeding.**

### 🔍 Validation Round 3 (Bonus: 75 XP if no warnings)
```powershell
# Check for restore warnings
$restoreLog = dotnet restore AIDevGallery.sln --verbosity normal 2>&1
if ($restoreLog -match "warning") {
    Write-Host "⚠️ Warnings found during restore" -ForegroundColor Yellow
    $restoreLog | Select-String "warning"
} else {
    Write-Host "✅ No warnings found!" -ForegroundColor Green
}
```
**✅ Success Criteria:** No restore errors, minimal warnings

---

## 🎯 Level 4: Build Preparation (250 XP)

### Tasks:
1. **Update Package Manifest Version** (50 XP)
   - Get version from nbgv:
     ```powershell
     $version = nbgv get-version -v Version
     $manifestPath = "AIDevGallery\Package.appxmanifest"
     [xml]$manifest = Get-Content $manifestPath
     $manifest.Package.Identity.Version = "$version.0"
     $manifest.Save((Resolve-Path $manifestPath).Path)
     Write-Host "✅ Manifest version updated to: $version.0" -ForegroundColor Green
     ```

2. **Build AIDevGallery.Utils** (60 XP)
   - Run:
     ```powershell
     dotnet build AIDevGallery.Utils --no-restore /p:Configuration=Release
     ```

3. **Build Source Generator** (60 XP)
   - Run:
     ```powershell
     dotnet build AIDevGallery.SourceGenerator --no-restore /p:Configuration=Release
     ```

4. **Verify Build Tools** (30 XP)
   - Run:
     ```powershell
     & "${env:ProgramFiles}\Microsoft Visual Studio\2022\*\MSBuild\Current\Bin\MSBuild.exe" -version
     ```
   - If this fails, open Visual Studio Installer and ensure MSBuild is installed.

5. **Create Build Output Directory** (50 XP)
   - Run:
     ```powershell
     mkdir -Force AIDevGallery\AppPackages
     ```

### 🔍 Validation Round 4 (Bonus: 100 XP if no errors)
```powershell
# Check for build errors in Utils and SourceGenerator
$utilsErrors = dotnet build AIDevGallery.Utils --no-restore /p:Configuration=Release 2>&1 | Select-String "error"
$genErrors = dotnet build AIDevGallery.SourceGenerator --no-restore /p:Configuration=Release 2>&1 | Select-String "error"
if ($utilsErrors -or $genErrors) {
    Write-Host "❌ Build errors found!" -ForegroundColor Red
} else {
    Write-Host "✅ Pre-build components compiled successfully!" -ForegroundColor Green
}
```
**✅ Success Criteria:** Zero build errors

---

## 🎯 Level 5: Main Application Build (350 XP)

### Tasks:
1. **Detect Architecture** (30 XP)
   - Run:
     ```powershell
     $arch = if ([System.Environment]::Is64BitOperatingSystem) { 
         if ($env:PROCESSOR_ARCHITECTURE -eq "ARM64") { "ARM64" } else { "x64" }
     } else { "x86" }
     Write-Host "Building for architecture: $arch" -ForegroundColor Cyan
     ```

2. **Build Main Application** (200 XP)
   - Run:
     ```powershell
     dotnet build AIDevGallery --no-restore `
       -r win-$arch `
       -f net9.0-windows10.0.22621.0 `
       /p:Configuration=Release `
       /p:Platform=$arch `
       /p:SelfContainedIfPreviewWASDK=true
     ```

3. **Build Unit Tests** (70 XP)
   - Run:
     ```powershell
     dotnet build AIDevGallery.UnitTests `
       -r win-$arch `
       -f net9.0-windows10.0.22621.0 `
       /p:Configuration=Release `
       /p:Platform=$arch
     ```

4. **Generate MSIX Package** (50 XP)
   - Run:
     ```powershell
     dotnet build AIDevGallery --no-restore `
       -r win-$arch `
       -f net9.0-windows10.0.22621.0 `
       /p:Configuration=Release `
       /p:Platform=$arch `
       /p:AppxPackageDir="AppPackages/" `
       /p:UapAppxPackageBuildMode=SideloadOnly `
       /p:AppxBundle=Never `
       /p:GenerateAppxPackageOnBuild=true
     ```
   - If you see any errors, check the output for missing dependencies or misconfiguration.

### 🔍 Validation Round 5 (Bonus: 150 XP if no warnings)
```powershell
# Check build output
$buildLog = "build_output.log"
dotnet build AIDevGallery.sln /p:Configuration=Release /p:Platform=$arch > $buildLog 2>&1
$errors = Select-String -Path $buildLog -Pattern "error"
$warnings = Select-String -Path $buildLog -Pattern "warning"
Write-Host "Build Summary:" -ForegroundColor Cyan
Write-Host "Errors: $($errors.Count)" -ForegroundColor $(if ($errors.Count -eq 0) { "Green" } else { "Red" })
Write-Host "Warnings: $($warnings.Count)" -ForegroundColor $(if ($warnings.Count -eq 0) { "Green" } else { "Yellow" })
```
**✅ Success Criteria:** Zero errors, minimal warnings

---

## 🎯 Level 6: Visual Studio Setup & First Run (400 XP)

### Tasks:
1. **Open Solution in Visual Studio** (50 XP)
   - In PowerShell:
     ```powershell
     Start-Process "AIDevGallery.sln"
     ```
   - Or open the solution directly from Visual Studio.

2. **Configure Startup Project** (100 XP)
   - In Visual Studio Solution Explorer:
     - Right-click "AIDevGallery" project
     - Select "Set as Startup Project"
     - Confirm project name is **bold**

3. **Configure Build Configuration** (100 XP)
   - In Visual Studio toolbar:
     - Set Configuration: `Release`
     - Set Platform: Match your architecture (`x64` or `ARM64`)
     - Set Deploy target: `Local Machine`

4. **Enable Developer Mode** (50 XP)
   - In PowerShell as Administrator:
     ```powershell
     reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock" /t REG_DWORD /f /v "AllowDevelopmentWithoutDevLicense" /d "1"
     ```
   - Or enable Developer Mode in Windows Settings > For Developers.

5. **First Debug Run** (100 XP)
   - Press `F5` in Visual Studio
   - Wait for app to compile and launch
   - Verify main window appears

### 🔍 Validation Round 6 (Bonus: 200 XP if app runs without errors)
```powershell
# Check if app process is running
$appProcess = Get-Process -Name "AIDevGallery" -ErrorAction SilentlyContinue
if ($appProcess) {
    Write-Host "✅ AI Dev Gallery is running! Process ID: $($appProcess.Id)" -ForegroundColor Green
} else {
    Write-Host "❌ App is not running" -ForegroundColor Red
}
```
**✅ Success Criteria:** App launches without crashes

---

## 🎯 Level 7: Unit Tests & Validation (500 XP)

### Tasks:
1. **Run Unit Tests** (200 XP)
   - Run:
     ```powershell
     dotnet test AIDevGallery.UnitTests `
       --configuration Release `
       --logger "console;verbosity=detailed" `
       --results-directory TestResults
     ```

2. **Check Code Analysis** (150 XP)
   - Run:
     ```powershell
     dotnet build AIDevGallery.sln /p:RunAnalyzersDuringBuild=true /p:Configuration=Release
     ```

3. **Verify MSIX Package** (100 XP)
   - Run:
     ```powershell
     $msixPath = Get-ChildItem -Path "AIDevGallery\AppPackages" -Filter "*.msix" -Recurse | Select-Object -First 1
     if ($msixPath) {
         Write-Host "✅ MSIX package found: $($msixPath.FullName)" -ForegroundColor Green
         Write-Host "Size: $([math]::Round($msixPath.Length / 1MB, 2)) MB" -ForegroundColor Cyan
     }
     ```

4. **Test Sample Loading** (50 XP)
   - In the running app:
     - Click on "Chat" sample
     - Verify it loads without errors
     - Try at least 2 more samples

### 🔍 Final Boss Validation (Bonus: 500 XP for perfect score)
```powershell
# Comprehensive validation
Write-Host "`n🎮 FINAL VALIDATION ROUND" -ForegroundColor Magenta
Write-Host "========================" -ForegroundColor Magenta
$validationScore = 0
$maxScore = 5
# 1. Check build artifacts
if (Test-Path "AIDevGallery\bin\$arch\Release\net9.0-windows10.0.22621.0\AIDevGallery.exe") {
    Write-Host "✅ [1/5] Main executable found" -ForegroundColor Green
    $validationScore++
} else {
    Write-Host "❌ [1/5] Main executable missing" -ForegroundColor Red
}
# 2. Check dependencies
$depsPath = "AIDevGallery\bin\$arch\Release\net9.0-windows10.0.22621.0\AIDevGallery.deps.json"
if ((Test-Path $depsPath) -and ((Get-Item $depsPath).Length -gt 100KB)) {
    Write-Host "✅ [2/5] Dependencies file valid" -ForegroundColor Green
    $validationScore++
} else {
    Write-Host "❌ [2/5] Dependencies file invalid" -ForegroundColor Red
}
# 3. Check MSIX package
if (Get-ChildItem -Path "AIDevGallery\AppPackages" -Filter "*.msix" -Recurse) {
    Write-Host "✅ [3/5] MSIX package generated" -ForegroundColor Green
    $validationScore++
} else {
    Write-Host "❌ [3/5] MSIX package missing" -ForegroundColor Red
}
# 4. Check test results
if (Test-Path "TestResults") {
    Write-Host "✅ [4/5] Test results generated" -ForegroundColor Green
    $validationScore++
} else {
    Write-Host "❌ [4/5] Test results missing" -ForegroundColor Red
}
# 5. Final app check
$finalCheck = Read-Host "`nIs the AI Dev Gallery app running successfully? (y/n)"
if ($finalCheck -eq 'y') {
    Write-Host "✅ [5/5] App running successfully!" -ForegroundColor Green
    $validationScore++
} else {
    Write-Host "❌ [5/5] App not running" -ForegroundColor Red
}
# Calculate final score
Write-Host "`n🏆 FINAL SCORE: $validationScore/$maxScore" -ForegroundColor $(if ($validationScore -eq $maxScore) { "Green" } else { "Yellow" })
if ($validationScore -eq $maxScore) {
    Write-Host "🎊 PERFECT SCORE! You've earned the bonus 500 XP!" -ForegroundColor Magenta
    Write-Host "🎮 QUEST COMPLETE! AI Dev Gallery is fully operational!" -ForegroundColor Green
} else {
    Write-Host "⚠️ Some issues remain. Review the errors above." -ForegroundColor Yellow
}
```

---

## 🏆 Quest Completion Summary

### Total XP Available: 2,500 XP
- Level 1: 150 XP
- Level 2: 200 XP  
- Level 3: 275 XP
- Level 4: 350 XP
- Level 5: 500 XP
- Level 6: 600 XP
- Level 7: 1,000 XP
- Perfect Score Bonus: 500 XP

### Achievement Unlocked: AI Dev Gallery Master! 🎊

---

## 🚨 Troubleshooting Guide

### Common Issues & Solutions:

1. **NuGet Restore Failures**
   - Run:
     ```powershell
     dotnet nuget locals all --clear
     dotnet restore --force
     ```
   - Ensure only one nuget.org source is enabled and points to https://api.nuget.org/v3/index.json

2. **Architecture Mismatch on ARM64**
   - Set platform explicitly:
     ```powershell
     $env:Platform = "ARM64"
     dotnet build -r win-arm64
     ```

3. **Missing Windows SDK**
   - Open Visual Studio Installer
   - Go to "Individual Components"
   - Search for and install "Windows 11 SDK (10.0.22621.0)"

4. **Certificate Issues**
   - Generate a test certificate:
     ```powershell
     $cert = New-SelfSignedCertificate -Subject "CN=AIDevGallery Test" -Type CodeSigningCert -CertStoreLocation "Cert:\CurrentUser\My"
     ```

5. **Build Errors with Source Generator**
   - Clean solution:
     ```powershell
     dotnet clean
     Remove-Item -Recurse -Force .\**\bin, .\**\obj
     dotnet restore
     ```
   - Rebuild from scratch

---

## 📝 Final Notes

- Always run Visual Studio as Administrator for package deployment
- Keep the solution architecture consistent (do not mix x64 and ARM64)
- If switching architectures, perform a full clean and restore
- Check Event Viewer > Applications for detailed error logs if the app crashes
- If you encounter any ambiguity, re-read each step and ensure all prerequisites are met before proceeding

**Quest Designer Note:** This guide is designed for zero-ambiguity execution. Each command is tested and includes validation steps. Complete all levels in order for guaranteed success! 🎮
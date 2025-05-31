# Darbot Dev - Complete Setup and Troubleshooting Guide

> **Research Fork Notice**: This is a setup guide for the Darbot Dev research fork. For the official Microsoft AI Dev Gallery, visit the [original repository](https://github.com/microsoft/ai-dev-gallery).

## 🚀 Quick Start Guide

### Prerequisites (Required)
- **Windows 10** version 1809 (Build 17763) or **Windows 11**
- **16 GB RAM** minimum (32 GB recommended for AI models)
- **20 GB free disk space** minimum
- **Visual Studio 2022** (latest version)
- **.NET 9.0 SDK**
- **Git** for version control

### Express Setup (5 minutes)

1. **Clone the repository:**
   ```bash
   git clone https://github.com/darbotlabs/darbot-dev.git
   cd darbot-dev
   ```

2. **Run the automated validation:**
   ```powershell
   # This will check your environment and build the app
   .\validate-app.ps1
   ```

3. **If validation passes, build and run:**
   ```powershell
   .\build.ps1
   ```

## 🔧 Detailed Setup Instructions

### Step 1: Environment Setup

#### Install .NET 9.0 SDK
```powershell
# Option 1: Using winget (recommended)
winget install Microsoft.DotNet.SDK.9

# Option 2: Manual download
# Visit: https://dotnet.microsoft.com/en-us/download/dotnet/9.0
```

#### Verify .NET Installation
```powershell
dotnet --version
# Should output: 9.0.x
```

#### Install Visual Studio 2022 Workloads
Open Visual Studio Installer and ensure these workloads are installed:
- ✅ **.NET Desktop Development**
- ✅ **Desktop development with C++**
- ✅ **Universal Windows Platform development**

Under "Individual components", ensure:
- ✅ **Windows 11 SDK (10.0.22621.0)** or latest
- ✅ **.NET 9.0 Runtime**
- ✅ **NuGet package manager**

### Step 2: Build the Application

#### Option 1: Using the Build Script (Recommended)
```powershell
.\build.ps1
```

#### Option 2: Manual Build
```powershell
# Detect architecture
$arch = if ([System.Environment]::Is64BitOperatingSystem) { 
    if ($env:PROCESSOR_ARCHITECTURE -eq "ARM64") { "ARM64" } else { "x64" }
} else { "x86" }

# Restore packages
dotnet restore AIDevGallery.sln

# Build for detected architecture
dotnet build AIDevGallery.sln -c Debug -p:Platform=$arch
```

#### Option 3: Visual Studio
1. Open `AIDevGallery.sln` in Visual Studio 2022
2. Set the platform to match your architecture (x64 or ARM64)
3. Build the solution (Ctrl+Shift+B)
4. Run the application (F5)

## 🧪 Validation and Testing

### Run Comprehensive Validation
```powershell
# Full validation (recommended)
.\validate-app.ps1

# Quick validation (skip lengthy tests)
.\validate-app.ps1 -Quick

# Skip build (environment check only)
.\validate-app.ps1 -SkipBuild

# Skip unit tests
.\validate-app.ps1 -SkipTests

# Production build validation
.\validate-app.ps1 -Configuration Release
```

### Cross-Platform Structure Check
On any platform (including Linux/macOS):
```bash
./validate-structure.sh
```

### Manual Validation Steps
1. **Check build output:**
   ```powershell
   # Look for the executable
   ls AIDevGallery\bin\$arch\Debug\net9.0-windows10.0.22621.0\AIDevGallery.exe
   ```

2. **Run unit tests:**
   ```powershell
   dotnet test AIDevGallery.UnitTests -c Debug -p:Platform=$arch
   ```

3. **Launch the application:**
   ```powershell
   dotnet run --project AIDevGallery -c Debug -p:Platform=$arch
   ```

## 🔍 Troubleshooting Common Issues

### Issue 1: "AnyCPU" Build Errors
**Symptoms:** Build fails with WinUI platform errors
**Solution:**
```powershell
# Always specify platform explicitly
dotnet build -p:Platform=x64  # or ARM64
```

### Issue 2: Missing .NET 9.0
**Symptoms:** "The framework 'Microsoft.NETCore.App', version '9.0.0' was not found"
**Solution:**
```powershell
# Install .NET 9.0 SDK
winget install Microsoft.DotNet.SDK.9
# Restart terminal and verify
dotnet --version
```

### Issue 3: Package Restore Failures
**Symptoms:** NuGet restore errors, missing dependencies
**Solution:**
```powershell
# Clear NuGet cache
dotnet nuget locals all --clear
# Restore with verbose output
dotnet restore AIDevGallery.sln --verbosity detailed
```

### Issue 4: Visual Studio Build Errors
**Symptoms:** VS shows red squiggles, Intellisense errors
**Solution:**
1. Close Visual Studio
2. Delete `bin` and `obj` folders:
   ```powershell
   Get-ChildItem -Path . -Recurse -Directory -Name "bin","obj" | Remove-Item -Recurse -Force
   ```
3. Reopen Visual Studio and rebuild

### Issue 5: App Crashes on Startup
**Symptoms:** Application starts but immediately crashes
**Solution:**
```powershell
# Run with diagnostics
dotnet run --project AIDevGallery -c Debug -p:Platform=$arch --verbosity diagnostic

# Check Windows Event Viewer for crash details
# Applications and Services Logs > Microsoft > Windows > Apps
```

### Issue 6: Windows Version Compatibility
**Symptoms:** App won't run on older Windows versions
**Solution:**
- Minimum: Windows 10 version 1809 (Build 17763)
- Recommended: Windows 11 with latest updates
- Check Windows version:
  ```powershell
  [System.Environment]::OSVersion.Version
  Get-ComputerInfo | Select WindowsProductName, WindowsVersion
  ```

### Issue 7: ARM64 vs x64 Architecture Issues
**Symptoms:** Performance issues or compatibility problems
**Solution:**
```powershell
# Check your architecture
echo $env:PROCESSOR_ARCHITECTURE

# Build for specific architecture
dotnet build -p:Platform=ARM64  # For ARM64 devices
dotnet build -p:Platform=x64    # For Intel/AMD devices
```

### Issue 8: Unit Tests Failing
**Symptoms:** Test execution fails or tests don't run
**Solution:**
```powershell
# Run tests with detailed output
dotnet test AIDevGallery.UnitTests -c Debug -p:Platform=$arch --logger "console;verbosity=detailed"

# Check test dependencies
dotnet list AIDevGallery.UnitTests package
```

## 📊 Validation Checklist

Use this checklist to verify your setup:

### Environment ✅
- [ ] Windows 10 Build 17763+ or Windows 11
- [ ] 16+ GB RAM available
- [ ] 20+ GB free disk space
- [ ] .NET 9.0 SDK installed
- [ ] Visual Studio 2022 with required workloads
- [ ] Git installed and configured

### Repository ✅
- [ ] Repository cloned successfully
- [ ] All project files present
- [ ] Solution opens in Visual Studio
- [ ] No missing references in projects

### Build ✅
- [ ] NuGet packages restore successfully
- [ ] Solution builds without errors
- [ ] Architecture detected correctly (x64/ARM64)
- [ ] Build artifacts generated in bin folder

### Tests ✅
- [ ] Unit tests build successfully
- [ ] Unit tests execute without failures
- [ ] Test results generated

### Runtime ✅
- [ ] Application launches without crashes
- [ ] Main window displays correctly
- [ ] Sample data loads properly
- [ ] At least one AI sample works

## 🎯 Performance Optimization

### For Better Performance:
1. **Use Release build for testing:**
   ```powershell
   dotnet build -c Release -p:Platform=$arch
   ```

2. **Ensure adequate VRAM for AI models:**
   - Minimum: 4GB VRAM
   - Recommended: 8GB+ VRAM

3. **Close unnecessary applications:**
   - AI models are memory-intensive
   - Ensure maximum available RAM

## 📞 Getting Help

### If You're Still Having Issues:

1. **Run the full validation script:**
   ```powershell
   .\validate-app.ps1 -Verbose
   ```

2. **Check the validation results:** Look for specific error messages and follow the suggested fixes.

3. **Gather system information:**
   ```powershell
   # System info
   systeminfo
   # .NET info
   dotnet --info
   # Visual Studio info
   Get-ItemProperty "HKLM:SOFTWARE\Microsoft\VisualStudio\*\*" | Select PSChildName, DisplayName, InstallDir
   ```

4. **Create an issue:** If problems persist, create a GitHub issue with:
   - Validation script output
   - System information
   - Error messages/logs
   - Steps to reproduce

## 🔄 Keeping Up to Date

### Regular Maintenance:
```powershell
# Update .NET SDK
winget upgrade Microsoft.DotNet.SDK.9

# Update Visual Studio
# Use Visual Studio Installer

# Update repository
git pull origin main

# Re-run validation after updates
.\validate-app.ps1
```

---

**Last Updated:** $(Get-Date -Format "yyyy-MM-dd")
**Validation Script Version:** 1.0.0
**Compatible with:** Darbot Dev v0.3.11-alpha (research fork)
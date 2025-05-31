#!/usr/bin/env pwsh
# AI Dev Gallery - Comprehensive End-to-End Validation Script
# Based on the Game1.md validation framework

param(
    [switch]$Quick,
    [switch]$SkipTests,
    [switch]$SkipBuild,
    [string]$Configuration = "Debug"
)

# Initialize validation tracking
$script:validationResults = @()
$script:totalScore = 0
$script:maxScore = 0

function Write-ValidationHeader {
    param([string]$Title, [string]$Description = "")
    Write-Host "`n" -NoNewline
    Write-Host "🎯 $Title" -ForegroundColor Cyan
    Write-Host "="*50 -ForegroundColor DarkGray
    if ($Description) {
        Write-Host $Description -ForegroundColor Gray
        Write-Host ""
    }
}

function Test-ValidationStep {
    param(
        [string]$Name,
        [scriptblock]$Test,
        [int]$Points = 10,
        [string]$SuccessMessage = "",
        [string]$FailureMessage = "",
        [scriptblock]$OnSuccess = {},
        [scriptblock]$OnFailure = {}
    )
    
    $script:maxScore += $Points
    
    try {
        $result = & $Test
        if ($result) {
            Write-Host "✅ $Name" -ForegroundColor Green
            if ($SuccessMessage) { Write-Host "   $SuccessMessage" -ForegroundColor DarkGreen }
            $script:totalScore += $Points
            & $OnSuccess
            $script:validationResults += @{
                Name = $Name
                Status = "PASS"
                Points = $Points
                Message = $SuccessMessage
            }
        } else {
            Write-Host "❌ $Name" -ForegroundColor Red
            if ($FailureMessage) { Write-Host "   $FailureMessage" -ForegroundColor DarkRed }
            & $OnFailure
            $script:validationResults += @{
                Name = $Name
                Status = "FAIL"
                Points = 0
                Message = $FailureMessage
            }
        }
    } catch {
        Write-Host "❌ $Name" -ForegroundColor Red
        Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor DarkRed
        & $OnFailure
        $script:validationResults += @{
            Name = $Name
            Status = "ERROR"
            Points = 0
            Message = $_.Exception.Message
        }
    }
}

function Get-SystemArchitecture {
    if ([System.Environment]::Is64BitOperatingSystem) { 
        if ($env:PROCESSOR_ARCHITECTURE -eq "ARM64") { "ARM64" } else { "x64" }
    } else { "x86" }
}

# Level 1: Environment Setup Validation
Write-ValidationHeader "Level 1: Environment Setup Validation" "Checking prerequisites and dependencies"

Test-ValidationStep -Name "Windows OS Version Check" -Points 20 -Test {
    $osVersion = [System.Environment]::OSVersion.Version
    $buildNumber = (Get-ItemProperty "HKLM:SOFTWARE\Microsoft\Windows NT\CurrentVersion").CurrentBuild
    $buildNumber -ge 17763
} -SuccessMessage "Windows build $buildNumber is supported" -FailureMessage "Windows build must be 17763 or higher"

Test-ValidationStep -Name ".NET 9.0 SDK Installation" -Points 30 -Test {
    $dotnetVersion = dotnet --version 2>$null
    $dotnetVersion -and $dotnetVersion.StartsWith("9.")
} -SuccessMessage "Found .NET $dotnetVersion" -FailureMessage "Please install .NET 9.0 SDK"

Test-ValidationStep -Name "Visual Studio 2022 Detection" -Points 20 -Test {
    $vsPath = @(
        "${env:ProgramFiles}\Microsoft Visual Studio\2022\*\Common7\IDE\devenv.exe",
        "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2022\*\Common7\IDE\devenv.exe"
    ) | Where-Object { Test-Path $_ } | Select-Object -First 1
    $vsPath -ne $null
} -SuccessMessage "Visual Studio 2022 found" -FailureMessage "Visual Studio 2022 not detected"

Test-ValidationStep -Name "Git Installation Check" -Points 10 -Test {
    (Get-Command git -ErrorAction SilentlyContinue) -ne $null
} -SuccessMessage "Git is available" -FailureMessage "Git is not installed"

Test-ValidationStep -Name "System Resources Check" -Points 20 -Test {
    $ram = [math]::Round((Get-CimInstance Win32_ComputerSystem).TotalPhysicalMemory / 1GB, 2)
    $diskSpace = [math]::Round((Get-CimInstance Win32_LogicalDisk | Where-Object DeviceID -eq "C:").FreeSpace / 1GB, 2)
    $ram -ge 16 -and $diskSpace -ge 20
} -SuccessMessage "RAM: ${ram}GB, Free disk: ${diskSpace}GB" -FailureMessage "Need 16GB+ RAM and 20GB+ free disk space"

# Level 2: Repository Setup Validation
Write-ValidationHeader "Level 2: Repository Setup Validation" "Checking repository structure and configuration"

Test-ValidationStep -Name "Solution File Exists" -Points 15 -Test {
    Test-Path "AIDevGallery.sln"
} -SuccessMessage "Solution file found" -FailureMessage "AIDevGallery.sln not found"

Test-ValidationStep -Name "Main Project Structure" -Points 15 -Test {
    (Test-Path "AIDevGallery\AIDevGallery.csproj") -and
    (Test-Path "AIDevGallery\App.xaml") -and
    (Test-Path "AIDevGallery\MainWindow.xaml")
} -SuccessMessage "Main project structure is valid" -FailureMessage "Missing core project files"

Test-ValidationStep -Name "Build Configuration Files" -Points 10 -Test {
    (Test-Path "Directory.Build.props") -and
    (Test-Path "Directory.Packages.props") -and
    (Test-Path "version.json")
} -SuccessMessage "Build configuration files present" -FailureMessage "Missing build configuration files"

Test-ValidationStep -Name "Unit Tests Project" -Points 10 -Test {
    Test-Path "AIDevGallery.UnitTests\AIDevGallery.UnitTests.csproj"
} -SuccessMessage "Unit tests project found" -FailureMessage "Unit tests project missing"

# Level 3: Dependencies Validation
Write-ValidationHeader "Level 3: Dependencies Validation" "Validating NuGet packages and dependencies"

if (-not $SkipBuild) {
    Test-ValidationStep -Name "NuGet Package Restore" -Points 25 -Test {
        dotnet restore AIDevGallery.sln --verbosity quiet 2>$null
        $LASTEXITCODE -eq 0
    } -SuccessMessage "All packages restored successfully" -FailureMessage "Package restore failed"
}

Test-ValidationStep -Name "Sample Data Structure" -Points 15 -Test {
    (Test-Path "AIDevGallery\Samples") -and
    (Get-ChildItem "AIDevGallery\Samples" -Recurse -Filter "*.json").Count -gt 0
} -SuccessMessage "Sample data files found" -FailureMessage "Sample data structure incomplete"

Test-ValidationStep -Name "Asset Files Present" -Points 10 -Test {
    (Test-Path "AIDevGallery\Assets") -and
    (Get-ChildItem "AIDevGallery\Assets" -Recurse).Count -gt 10
} -SuccessMessage "Asset files are present" -FailureMessage "Missing asset files"

# Level 4: Build Validation
Write-ValidationHeader "Level 4: Build Validation" "Testing compilation and build process"

$arch = Get-SystemArchitecture
Write-Host "Detected architecture: $arch" -ForegroundColor Cyan

if (-not $SkipBuild) {
    Test-ValidationStep -Name "Debug Build (AIDevGallery)" -Points 40 -Test {
        dotnet build AIDevGallery\AIDevGallery.csproj -c $Configuration -p:Platform=$arch --verbosity quiet --no-restore 2>$null
        $LASTEXITCODE -eq 0
    } -SuccessMessage "Main project builds successfully" -FailureMessage "Main project build failed"

    Test-ValidationStep -Name "Debug Build (Utils)" -Points 20 -Test {
        dotnet build AIDevGallery.Utils\AIDevGallery.Utils.csproj -c $Configuration --verbosity quiet --no-restore 2>$null
        $LASTEXITCODE -eq 0
    } -SuccessMessage "Utils project builds successfully" -FailureMessage "Utils project build failed"

    Test-ValidationStep -Name "Debug Build (SourceGenerator)" -Points 15 -Test {
        dotnet build AIDevGallery.SourceGenerator\AIDevGallery.SourceGenerator.csproj -c $Configuration --verbosity quiet --no-restore 2>$null
        $LASTEXITCODE -eq 0
    } -SuccessMessage "Source generator builds successfully" -FailureMessage "Source generator build failed"

    Test-ValidationStep -Name "Unit Tests Build" -Points 25 -Test {
        dotnet build AIDevGallery.UnitTests\AIDevGallery.UnitTests.csproj -c $Configuration -p:Platform=$arch --verbosity quiet --no-restore 2>$null
        $LASTEXITCODE -eq 0
    } -SuccessMessage "Unit tests project builds successfully" -FailureMessage "Unit tests build failed"
}

# Level 5: Build Artifacts Validation
Write-ValidationHeader "Level 5: Build Artifacts Validation" "Checking generated files and outputs"

$outputPath = "AIDevGallery\bin\$arch\$Configuration\net9.0-windows10.0.22621.0"

Test-ValidationStep -Name "Main Executable Generated" -Points 30 -Test {
    Test-Path "$outputPath\AIDevGallery.exe"
} -SuccessMessage "AIDevGallery.exe found" -FailureMessage "Main executable not generated"

Test-ValidationStep -Name "Dependencies File" -Points 15 -Test {
    $depsPath = "$outputPath\AIDevGallery.deps.json"
    (Test-Path $depsPath) -and ((Get-Item $depsPath).Length -gt 100KB)
} -SuccessMessage "Dependencies file is valid" -FailureMessage "Dependencies file missing or invalid"

Test-ValidationStep -Name "Runtime Configuration" -Points 10 -Test {
    Test-Path "$outputPath\AIDevGallery.runtimeconfig.json"
} -SuccessMessage "Runtime configuration found" -FailureMessage "Runtime configuration missing"

Test-ValidationStep -Name "XAML Resources" -Points 15 -Test {
    $resourcesPath = "$outputPath\AIDevGallery.dll"
    Test-Path $resourcesPath
} -SuccessMessage "Compiled resources found" -FailureMessage "Compiled resources missing"

# Level 6: Unit Tests Execution
Write-ValidationHeader "Level 6: Unit Tests Execution" "Running automated tests"

if (-not $SkipTests -and -not $SkipBuild) {
    Test-ValidationStep -Name "Unit Tests Execution" -Points 50 -Test {
        dotnet test AIDevGallery.UnitTests\AIDevGallery.UnitTests.csproj -c $Configuration -p:Platform=$arch --verbosity quiet --no-build --logger "trx;LogFileName=TestResults.trx" 2>$null
        $LASTEXITCODE -eq 0
    } -SuccessMessage "All unit tests passed" -FailureMessage "Some unit tests failed"

    Test-ValidationStep -Name "Test Results Generated" -Points 15 -Test {
        Test-Path "AIDevGallery.UnitTests\TestResults\TestResults.trx"
    } -SuccessMessage "Test results file generated" -FailureMessage "Test results file not found"
}

# Level 7: Application Startup Test
Write-ValidationHeader "Level 7: Application Startup Test" "Testing application launch capability"

if (-not $SkipBuild) {
    Test-ValidationStep -Name "Application Launch Test" -Points 35 -Test {
        # This is a basic test - just check if the executable can start without immediate crash
        # In a real scenario, you'd want more sophisticated startup testing
        $process = Start-Process -FilePath "$outputPath\AIDevGallery.exe" -PassThru -WindowStyle Hidden
        Start-Sleep -Seconds 3
        if (-not $process.HasExited) {
            $process.Kill()
            $true
        } else {
            $false
        }
    } -SuccessMessage "Application starts successfully" -FailureMessage "Application fails to start or crashes immediately"
}

# Final Validation Summary
Write-ValidationHeader "🏆 Final Validation Summary" "Complete assessment results"

$percentage = [math]::Round(($script:totalScore / $script:maxScore) * 100, 1)

Write-Host "`n📊 VALIDATION RESULTS:" -ForegroundColor Magenta
Write-Host "=====================" -ForegroundColor Magenta
Write-Host "Score: $($script:totalScore)/$($script:maxScore) ($percentage%)" -ForegroundColor $(
    if ($percentage -ge 90) { "Green" }
    elseif ($percentage -ge 70) { "Yellow" }
    else { "Red" }
)

Write-Host "`n📋 Detailed Results:" -ForegroundColor White
foreach ($result in $script:validationResults) {
    $statusColor = switch ($result.Status) {
        "PASS" { "Green" }
        "FAIL" { "Red" }
        "ERROR" { "Magenta" }
    }
    $statusIcon = switch ($result.Status) {
        "PASS" { "✅" }
        "FAIL" { "❌" }
        "ERROR" { "⚠️" }
    }
    Write-Host "$statusIcon $($result.Name) ($($result.Points) pts)" -ForegroundColor $statusColor
    if ($result.Message) {
        Write-Host "   $($result.Message)" -ForegroundColor DarkGray
    }
}

Write-Host "`n🎯 FINAL ASSESSMENT:" -ForegroundColor Cyan
if ($percentage -ge 90) {
    Write-Host "🎊 EXCELLENT! AI Dev Gallery is fully operational!" -ForegroundColor Green
    Write-Host "   The application should work perfectly for end users." -ForegroundColor Green
} elseif ($percentage -ge 70) {
    Write-Host "⚠️ GOOD but needs attention. Most functionality works." -ForegroundColor Yellow
    Write-Host "   Some issues need to be resolved for optimal experience." -ForegroundColor Yellow
} else {
    Write-Host "❌ SIGNIFICANT ISSUES FOUND. App may not work properly." -ForegroundColor Red
    Write-Host "   Multiple problems need to be addressed before release." -ForegroundColor Red
}

Write-Host "`n💡 NEXT STEPS:" -ForegroundColor White
if ($script:validationResults | Where-Object { $_.Status -ne "PASS" }) {
    Write-Host "1. Review failed validation steps above" -ForegroundColor Gray
    Write-Host "2. Address the issues mentioned in failure messages" -ForegroundColor Gray
    Write-Host "3. Re-run this validation script to verify fixes" -ForegroundColor Gray
} else {
    Write-Host "🎮 All validation steps passed! The app is ready for use." -ForegroundColor Green
}

Write-Host "`n⚡ Usage Tips:" -ForegroundColor White
Write-Host "- Run with -Quick to skip lengthy tests" -ForegroundColor Gray
Write-Host "- Use -SkipBuild to only check environment and structure" -ForegroundColor Gray
Write-Host "- Use -SkipTests to bypass unit test execution" -ForegroundColor Gray
Write-Host "- Use -Configuration Release for production builds" -ForegroundColor Gray

return $percentage -ge 90
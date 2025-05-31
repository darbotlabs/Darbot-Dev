#!/bin/bash
# AI Dev Gallery - Cross-Platform Project Structure Validation
# This script performs basic validation that can run on any platform

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

# Validation tracking
TOTAL_SCORE=0
MAX_SCORE=0
VALIDATION_RESULTS=()

print_header() {
    echo -e "\n${CYAN}🎯 $1${NC}"
    echo -e "${GRAY}$(printf '=%.0s' {1..50})${NC}"
    if [ -n "$2" ]; then
        echo -e "${GRAY}$2${NC}\n"
    fi
}

validate_step() {
    local name="$1"
    local points="$2"
    local test_command="$3"
    local success_msg="$4"
    local failure_msg="$5"
    
    MAX_SCORE=$((MAX_SCORE + points))
    
    if eval "$test_command" >/dev/null 2>&1; then
        echo -e "✅ ${GREEN}$name${NC}"
        if [ -n "$success_msg" ]; then
            echo -e "   ${GREEN}$success_msg${NC}"
        fi
        TOTAL_SCORE=$((TOTAL_SCORE + points))
        VALIDATION_RESULTS+=("PASS:$name:$points:$success_msg")
    else
        echo -e "❌ ${RED}$name${NC}"
        if [ -n "$failure_msg" ]; then
            echo -e "   ${RED}$failure_msg${NC}"
        fi
        VALIDATION_RESULTS+=("FAIL:$name:0:$failure_msg")
    fi
}

print_header "Cross-Platform Project Structure Validation" "Checking repository structure and configuration files"

# Project Structure Validation
validate_step "Solution File Exists" 15 "test -f AIDevGallery.sln" \
    "Solution file found" "AIDevGallery.sln not found"

validate_step "Main Project Structure" 20 "test -f AIDevGallery/AIDevGallery.csproj && test -f AIDevGallery/App.xaml && test -f AIDevGallery/MainWindow.xaml" \
    "Main project structure is valid" "Missing core project files"

validate_step "Build Configuration Files" 15 "test -f Directory.Build.props && test -f Directory.Packages.props && test -f version.json" \
    "Build configuration files present" "Missing build configuration files"

validate_step "Unit Tests Project" 10 "test -f AIDevGallery.UnitTests/AIDevGallery.UnitTests.csproj" \
    "Unit tests project found" "Unit tests project missing"

validate_step "Source Generator Project" 10 "test -f AIDevGallery.SourceGenerator/AIDevGallery.SourceGenerator.csproj" \
    "Source generator project found" "Source generator project missing"

validate_step "Utils Project" 10 "test -f AIDevGallery.Utils/AIDevGallery.Utils.csproj" \
    "Utils project found" "Utils project missing"

validate_step "Build Script Present" 10 "test -f build.ps1" \
    "Build script found" "build.ps1 script missing"

validate_step "Sample Data Structure" 15 "test -d AIDevGallery/Samples && find AIDevGallery/Samples -name '*.json' | head -1" \
    "Sample data files found" "Sample data structure incomplete"

validate_step "Asset Files Present" 10 "test -d AIDevGallery/Assets && test \$(find AIDevGallery/Assets -type f | wc -l) -gt 10" \
    "Asset files are present" "Missing asset files"

validate_step "Documentation Present" 10 "test -f README.md && test -f Darbot/README.md" \
    "Documentation files found" "Missing documentation"

validate_step "Darbot Design System" 15 "test -d Darbot && test -f Darbot/Status.md && test -f Darbot/StyleGuide.md" \
    "Darbot design system files present" "Darbot design system incomplete"

validate_step "Version Configuration" 10 "grep -q 'version.*0.3.11-alpha' version.json" \
    "Version configuration valid" "Version configuration invalid"

validate_step "Target Framework Check" 15 "grep -q 'net9.0-windows10.0.22621.0' AIDevGallery/AIDevGallery.csproj" \
    "Targeting .NET 9.0 for Windows" "Incorrect target framework"

validate_step "Platform Configuration" 10 "grep -q '<Platforms>x64;ARM64</Platforms>' AIDevGallery/AIDevGallery.csproj" \
    "Platform configuration correct" "Platform configuration missing"

validate_step "WinUI Dependencies" 15 "grep -q 'UseWinUI.*true' AIDevGallery/AIDevGallery.csproj" \
    "WinUI configuration present" "WinUI configuration missing"

validate_step "Project Generator Template" 10 "test -d AIDevGallery/ProjectGenerator/Template && test -f AIDevGallery/ProjectGenerator/Template/ProjectTemplate.csproj" \
    "Project template structure valid" "Project template missing"

validate_step "App Manifest Files" 10 "test -f AIDevGallery/app.manifest && test -f AIDevGallery.UnitTests/app.manifest" \
    "Application manifests present" "Application manifests missing"

# Code Quality Checks
print_header "Code Quality and Structure Checks" "Analyzing code organization and patterns"

validate_step "XAML Files Present" 10 "find AIDevGallery -name '*.xaml' | head -5" \
    "XAML UI files found" "No XAML files found"

validate_step "C# Source Files" 10 "find AIDevGallery -name '*.cs' | head -10" \
    "C# source files found" "No C# source files found"

validate_step "Sample Definitions" 15 "find AIDevGallery/Samples -name '*.json' | wc -l | awk '{if(\$1 > 5) exit 0; else exit 1}'" \
    "Multiple sample definitions found" "Insufficient sample definitions"

validate_step "Resource Files" 10 "test -d AIDevGallery/Assets/AppIcon && find AIDevGallery/Assets/AppIcon -name '*.png' | head -1" \
    "Application icons present" "Application icons missing"

validate_step "Localization Support" 5 "find AIDevGallery -name '*.resx' -o -name '*.resw' | head -1" \
    "Localization files found" "No localization files found"

validate_step "Unit Test Files" 10 "find AIDevGallery.UnitTests -name '*.cs' | head -1" \
    "Unit test files found" "No unit test files found"

# Git Repository Checks
print_header "Git Repository Health" "Checking version control configuration"

validate_step "Git Repository" 5 "test -d .git" \
    "Git repository initialized" "Not a git repository"

validate_step "Git Ignore File" 5 "test -f .gitignore" \
    ".gitignore file present" ".gitignore file missing"

validate_step "Git Blame Ignore" 5 "test -f .git-blame-ignore-revs" \
    "Git blame ignore configured" "Git blame ignore missing"

validate_step "GitHub Workflows" 10 "test -d .github/workflows" \
    "GitHub Actions configured" "GitHub Actions missing"

validate_step "EditorConfig" 5 "test -f .editorconfig" \
    "EditorConfig present" "EditorConfig missing"

# Dependency Analysis
print_header "Dependency and Package Analysis" "Analyzing project dependencies"

validate_step "Package References Valid" 15 "grep -c 'PackageReference Include' AIDevGallery/AIDevGallery.csproj | awk '{if(\$1 > 20) exit 0; else exit 1}'" \
    "Sufficient package references found" "Insufficient package dependencies"

validate_step "Microsoft Dependencies" 10 "grep -q 'Microsoft.WindowsAppSDK' AIDevGallery/AIDevGallery.csproj" \
    "Windows App SDK dependency present" "Windows App SDK dependency missing"

validate_step "Community Toolkit Dependencies" 10 "grep -q 'CommunityToolkit' AIDevGallery/AIDevGallery.csproj" \
    "Community Toolkit dependencies found" "Community Toolkit dependencies missing"

validate_step "ML Dependencies" 10 "grep -q 'Microsoft.ML.OnnxRuntime' AIDevGallery/AIDevGallery.csproj" \
    "ML.NET dependencies present" "ML.NET dependencies missing"

validate_step "Directory Packages" 10 "grep -c 'PackageVersion Include' Directory.Packages.props | awk '{if(\$1 > 10) exit 0; else exit 1}'" \
    "Central package management configured" "Central package management insufficient"

# Final Results
print_header "🏆 Final Validation Summary" "Complete assessment results"

PERCENTAGE=$((TOTAL_SCORE * 100 / MAX_SCORE))

echo -e "\n${CYAN}📊 VALIDATION RESULTS:${NC}"
echo -e "${GRAY}=====================${NC}"

if [ $PERCENTAGE -ge 90 ]; then
    echo -e "Score: ${GREEN}${TOTAL_SCORE}/${MAX_SCORE} (${PERCENTAGE}%)${NC}"
    echo -e "\n${GREEN}🎊 EXCELLENT! Project structure is well-organized!${NC}"
    echo -e "${GREEN}   The repository appears to be properly configured.${NC}"
elif [ $PERCENTAGE -ge 70 ]; then
    echo -e "Score: ${YELLOW}${TOTAL_SCORE}/${MAX_SCORE} (${PERCENTAGE}%)${NC}"
    echo -e "\n${YELLOW}⚠️ GOOD but needs attention. Most structure is correct.${NC}"
    echo -e "${YELLOW}   Some issues need to be resolved for optimal setup.${NC}"
else
    echo -e "Score: ${RED}${TOTAL_SCORE}/${MAX_SCORE} (${PERCENTAGE}%)${NC}"
    echo -e "\n${RED}❌ SIGNIFICANT ISSUES FOUND. Project may have setup problems.${NC}"
    echo -e "${RED}   Multiple problems need to be addressed.${NC}"
fi

echo -e "\n${BLUE}📋 Detailed Results:${NC}"
for result in "${VALIDATION_RESULTS[@]}"; do
    IFS=':' read -r status name points message <<< "$result"
    if [ "$status" = "PASS" ]; then
        echo -e "${GREEN}✅ $name ($points pts)${NC}"
        if [ -n "$message" ]; then
            echo -e "   ${GRAY}$message${NC}"
        fi
    else
        echo -e "${RED}❌ $name (0 pts)${NC}"
        if [ -n "$message" ]; then
            echo -e "   ${RED}$message${NC}"
        fi
    fi
done

echo -e "\n${BLUE}💡 NEXT STEPS:${NC}"
if [ $PERCENTAGE -lt 100 ]; then
    echo -e "${GRAY}1. Review failed validation steps above${NC}"
    echo -e "${GRAY}2. Address the issues mentioned in failure messages${NC}"
    echo -e "${GRAY}3. For Windows-specific validation, run validate-app.ps1${NC}"
    echo -e "${GRAY}4. Re-run this script to verify fixes${NC}"
else
    echo -e "${GREEN}🎮 All structural validation steps passed!${NC}"
    echo -e "${GREEN}   For full functionality testing, run on Windows with validate-app.ps1${NC}"
fi

echo -e "\n${BLUE}⚡ Platform Notes:${NC}"
echo -e "${GRAY}- This script validates project structure and configuration${NC}"
echo -e "${GRAY}- For build and runtime testing, use Windows with PowerShell script${NC}"
echo -e "${GRAY}- The app requires Windows 10+ and Visual Studio 2022 to build${NC}"

exit $([ $PERCENTAGE -ge 70 ] && echo 0 || echo 1)
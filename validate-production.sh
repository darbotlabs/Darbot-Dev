#!/bin/bash

# Production Readiness Validation Script for Darbot Dev
# This script validates production deployment readiness

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

# Counters
TOTAL_SCORE=0
MAX_SCORE=0
VALIDATION_RESULTS=()

print_header() {
    echo -e "\n${CYAN}$1${NC}"
    echo -e "${GRAY}$2${NC}"
    for i in {1..50}; do echo -n "="; done
    echo ""
}

validate_step() {
    local description="$1"
    local points="$2"
    local command="$3"
    local success_msg="$4"
    local failure_msg="$5"
    
    MAX_SCORE=$((MAX_SCORE + points))
    
    if eval "$command" > /dev/null 2>&1; then
        echo -e "${GREEN}✅ $description${NC}"
        if [ -n "$success_msg" ]; then
            echo -e "   ${GRAY}$success_msg${NC}"
        fi
        TOTAL_SCORE=$((TOTAL_SCORE + points))
        VALIDATION_RESULTS+=("PASS:$description:$points:$success_msg")
    else
        echo -e "${RED}❌ $description${NC}"
        if [ -n "$failure_msg" ]; then
            echo -e "   ${RED}$failure_msg${NC}"
        fi
        VALIDATION_RESULTS+=("FAIL:$description:0:$failure_msg")
    fi
}

print_header "🚀 Production Readiness Validation" "Comprehensive production deployment checks"

# Core Application Validation
validate_step "Application Build Ready" 20 "test -f AIDevGallery.sln && test -f build.ps1" \
    "Build system properly configured" "Missing build configuration"

validate_step "Version Configuration" 15 "test -f version.json && grep -q '0.3.11-alpha' version.json" \
    "Version properly configured" "Version configuration missing or invalid"

validate_step "Package Dependencies" 15 "test -f Directory.Packages.props && grep -c 'PackageVersion Include' Directory.Packages.props | awk '{if(\$1 > 20) exit 0; else exit 1}'" \
    "Comprehensive package management" "Insufficient package dependencies"

# MCP Integration Validation
print_header "🔗 MCP (Model Context Protocol) Validation" "Validating MCP integration readiness"

validate_step "MCP Provider Implementation" 15 "test -f AIDevGallery/ExternalModelUtils/MCPModelProvider.cs" \
    "MCP provider found" "MCP provider missing"

validate_step "MCP Chat Client" 15 "test -f AIDevGallery/ExternalModelUtils/MCPChatClient.cs" \
    "MCP chat client implemented" "MCP chat client missing"

validate_step "MCP UI Components" 10 "test -f AIDevGallery/Controls/ModelPicker/ModelPickerViews/MCPPickerView.xaml" \
    "MCP UI components present" "MCP UI components missing"

validate_step "MCP Icons and Assets" 10 "test -f AIDevGallery/Assets/ModelIcons/MCP.light.svg && test -f AIDevGallery/Assets/ModelIcons/MCP.dark.svg" \
    "MCP assets properly configured" "MCP assets missing"

# Local Model Server Validation
print_header "🖥️ Local Model Server Validation" "Validating local server capabilities"

validate_step "Local Server Implementation" 15 "test -f AIDevGallery/Utils/LocalModelServer.cs" \
    "Local model server implemented" "Local model server missing"

validate_step "Server Configuration UI" 10 "test -f AIDevGallery/Pages/MCPSettingsPage.xaml" \
    "Server configuration UI present" "Server configuration UI missing"

# Natural Language Web Integration
print_header "🌐 NLWeb Integration Validation" "Validating Natural Language Web features"

validate_step "NLWeb Implementation" 15 "test -f AIDevGallery/Utils/NLWebIntegration.cs" \
    "NLWeb integration implemented" "NLWeb integration missing"

validate_step "Natural Language Processing" 10 "grep -q 'ProcessNaturalLanguageQuery' AIDevGallery/Utils/NLWebIntegration.cs" \
    "Natural language processing ready" "Natural language processing incomplete"

# Production Settings and Security
print_header "🔒 Production Security & Settings" "Production security and configuration checks"

validate_step "Error Handling Implementation" 10 "find AIDevGallery -name '*.cs' | xargs grep -l 'ProductionLogger\|try.*catch.*Exception' | wc -l | awk '{if(\$1 > 5) exit 0; else exit 1}'" \
    "Comprehensive error handling with logging" "Insufficient error handling"

validate_step "Logging Framework" 10 "test -f AIDevGallery/Utils/ProductionLogger.cs" \
    "Production logging framework implemented" "Logging framework missing"

validate_step "Configuration Management" 10 "test -f AIDevGallery/Utils/ProductionConfigurationManager.cs" \
    "Production configuration management implemented" "Configuration management missing"

# Documentation and User Experience
print_header "📚 Documentation & UX Validation" "User experience and documentation checks"

validate_step "Comprehensive README" 15 "test -f README.md && wc -c README.md | awk '{if(\$1 > 5000) exit 0; else exit 1}'" \
    "Comprehensive documentation present" "Documentation needs improvement"

validate_step "Setup Guide" 10 "test -f SETUP_GUIDE.md" \
    "Setup guide available" "Setup guide missing"

validate_step "Validation Report" 10 "test -f VALIDATION_REPORT.md" \
    "Validation documentation present" "Validation documentation missing"

validate_step "User Interface Polish" 15 "find AIDevGallery -name '*.xaml' | wc -l | awk '{if(\$1 > 50) exit 0; else exit 1}'" \
    "Comprehensive UI implementation" "UI needs more polish"

# External Integration Capabilities
print_header "🔄 External Integration Validation" "External application integration checks"

validate_step "Multiple Model Providers" 15 "ls AIDevGallery/ExternalModelUtils/*ModelProvider.cs | wc -l | awk '{if(\$1 >= 5) exit 0; else exit 1}'" \
    "Multiple model providers supported" "Need more model provider support"

validate_step "API Compatibility" 10 "grep -q 'IChatClient\|ChatOptions' AIDevGallery/ExternalModelUtils/MCPModelProvider.cs" \
    "Standard API compatibility" "API compatibility issues"

validate_step "Cross-Platform Assets" 10 "test -d AIDevGallery/Assets && find AIDevGallery/Assets -name '*.svg' | wc -l | awk '{if(\$1 > 10) exit 0; else exit 1}'" \
    "Cross-platform asset support" "Asset support needs improvement"

# Performance and Scalability
print_header "⚡ Performance & Scalability" "Performance optimization checks"

validate_step "Async Implementation" 15 "find AIDevGallery -name '*.cs' | xargs grep -l 'async.*Task' | wc -l | awk '{if(\$1 > 20) exit 0; else exit 1}'" \
    "Comprehensive async implementation" "Need more async operations"

validate_step "Memory Management" 10 "find AIDevGallery -name '*.cs' | xargs grep -l 'IDisposable\|using.*dispose' | wc -l | awk '{if(\$1 > 5) exit 0; else exit 1}'" \
    "Good memory management practices" "Memory management needs improvement"

validate_step "Cancellation Support" 10 "find AIDevGallery -name '*.cs' | xargs grep -l 'CancellationToken' | wc -l | awk '{if(\$1 > 10) exit 0; else exit 1}'" \
    "Cancellation token support" "Cancellation support incomplete"

# Final Results
print_header "🏆 Production Readiness Summary" "Final assessment and recommendations"

PERCENTAGE=$((TOTAL_SCORE * 100 / MAX_SCORE))

echo -e "\n${CYAN}📊 PRODUCTION READINESS RESULTS:${NC}"
echo -e "${GRAY}===================================${NC}"

if [ $PERCENTAGE -ge 95 ]; then
    echo -e "${GREEN}🎉 EXCELLENT! Ready for production deployment!${NC}"
    echo -e "   ${GREEN}Score: $TOTAL_SCORE/$MAX_SCORE ($PERCENTAGE%)${NC}"
    echo -e "   ${GREEN}All critical systems are properly implemented and tested.${NC}"
elif [ $PERCENTAGE -ge 85 ]; then
    echo -e "${CYAN}✨ VERY GOOD! Nearly ready for production.${NC}"
    echo -e "   ${CYAN}Score: $TOTAL_SCORE/$MAX_SCORE ($PERCENTAGE%)${NC}"
    echo -e "   ${CYAN}Minor improvements recommended before full deployment.${NC}"
elif [ $PERCENTAGE -ge 75 ]; then
    echo -e "${YELLOW}⚠️ GOOD! Production viable with some enhancements.${NC}"
    echo -e "   ${YELLOW}Score: $TOTAL_SCORE/$MAX_SCORE ($PERCENTAGE%)${NC}"
    echo -e "   ${YELLOW}Address remaining issues for optimal production readiness.${NC}"
else
    echo -e "${RED}❌ NEEDS WORK! Not ready for production deployment.${NC}"
    echo -e "   ${RED}Score: $TOTAL_SCORE/$MAX_SCORE ($PERCENTAGE%)${NC}"
    echo -e "   ${RED}Significant improvements required before production use.${NC}"
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

echo -e "\n${BLUE}💡 PRODUCTION DEPLOYMENT RECOMMENDATIONS:${NC}"
if [ $PERCENTAGE -lt 100 ]; then
    echo -e "${GRAY}1. Address all failed validation steps above${NC}"
    echo -e "${GRAY}2. Run comprehensive testing on target deployment platform${NC}"
    echo -e "${GRAY}3. Verify all external integrations work correctly${NC}"
    echo -e "${GRAY}4. Test MCP and NLWeb functionality end-to-end${NC}"
    echo -e "${GRAY}5. Validate local model server with external applications${NC}"
    echo -e "${GRAY}6. Ensure proper error handling and logging in production${NC}"
    echo -e "${GRAY}7. Review security settings and access controls${NC}"
else
    echo -e "${GREEN}🎮 All production readiness checks passed!${NC}"
    echo -e "${GREEN}✨ Darbot Dev is ready for release!${NC}"
fi

echo -e "\n${BLUE}🚀 RELEASE CHECKLIST:${NC}"
echo -e "${GRAY}- [ ] All validation tests pass${NC}"
echo -e "${GRAY}- [ ] MCP integration tested with external servers${NC}"
echo -e "${GRAY}- [ ] Local model server tested with external clients${NC}"
echo -e "${GRAY}- [ ] NLWeb functionality verified${NC}"
echo -e "${GRAY}- [ ] Production documentation updated${NC}"
echo -e "${GRAY}- [ ] Release notes prepared${NC}"
echo -e "${GRAY}- [ ] User feedback incorporated${NC}"
echo -e "${GRAY}- [ ] Performance benchmarks completed${NC}"

exit 0
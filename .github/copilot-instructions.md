# GitHub Copilot Instructions for Darbot-Dev

> **Repository Context**: Darbot-Dev is a research fork of Microsoft's AI Dev Gallery, focused on developing and showcasing the Darbot Labs Cyber-Retro Design System while maintaining core AI functionality.

## Project Overview

This is a Windows developer tool built with:
- **Technology**: C#, WinUI 3, Windows App SDK, .NET 9.0
- **Architecture**: Desktop application for Windows 10 (Build 17763+) and Windows 11
- **Primary Focus**: AI sample gallery with Darbot Cyber-Retro design system integration
- **Build Requirements**: Visual Studio 2022, x64 or ARM64 platform targets (NOT AnyCPU)

## Key Design Principles

### 1. Darbot Cyber-Retro Design System
This repository showcases the Darbot Labs Cyber-Retro Design System. When working on UI components:

- **Colors**: Use Darbot gradient palette (Purple #B54CFF, Blue #0078FF, Green #00E0B4)
- **Dark Theme**: Maintain dark cyber aesthetics with rich backgrounds (#0A0A0B, #1A1A1D, #2A2A2F)
- **Typography**: Use Segoe UI Variable, follow established text styles (Display, Heading, Body, Code)
- **Spacing**: 8-point grid system - use multiples of 8px for margins and padding
- **Corner Radius**: 12px for cards/dialogs, 8px for buttons, 4px for small elements
- **Components**: Reference existing Darbot styles in `/Styles/Darbot/` directory

### 2. Code Style and Conventions

- Follow existing C# coding conventions and naming patterns
- Use file-scoped namespaces where appropriate
- Include copyright headers matching existing files: `// Copyright (c) Microsoft Corporation. All rights reserved. // Licensed under the MIT License.`
- Maintain consistency with the codebase's existing patterns

### 3. Sample Architecture

When adding or modifying samples:

- Samples go in the `/AIDevGallery/Samples/` directory
- Each sample requires a `.xaml` and `.xaml.cs` file
- Annotate sample classes with `[GallerySample]` attribute
- Model definitions go in `/AIDevGallery/Samples/ModelsDefinitions/` as `.model.json` files
- Follow the pattern in `/docs/AddingSamples.md`
- Ensure proper disposal of resources in the `Unloaded` event handler

### 4. Build and Platform Requirements

**CRITICAL**: This project requires explicit platform targets:

```powershell
# Always specify platform - NEVER use AnyCPU
dotnet build -p:Platform=x64     # For x64 systems
dotnet build -p:Platform=ARM64   # For ARM64 systems
```

- Use the provided `build.ps1` script which auto-detects architecture
- The solution uses .NET 9.0 SDK
- WinUI 3 components require platform-specific builds

### 5. Testing and Validation

Before committing changes:

```powershell
# Run comprehensive validation
.\validate-app.ps1

# Or quick validation
.\validate-app.ps1 -Quick
```

- The validation script checks environment setup, builds the project, and runs tests
- Ensure builds succeed on both x64 and ARM64 if possible
- Test UI changes manually in the running application

## Directory Structure

```
AIDevGallery/               # Main application project
├── Samples/                # AI sample implementations
│   ├── ModelsDefinitions/  # Model metadata (.model.json files)
│   └── [Various samples]   
├── Styles/                 # XAML style resources
│   └── Darbot/            # Darbot design system styles
├── Controls/              # Custom WinUI controls
├── ViewModels/            # MVVM view models
├── Pages/                 # Application pages
└── Utils/                 # Utility classes

Darbot/                    # Darbot design system documentation
├── README.md              # Design system overview
├── StyleGuide.md          # Complete style specifications
└── Game1.md               # Gamified setup guide

docs/                      # Additional documentation
├── AddingSamples.md       # Guide for adding new samples
└── PublishingNewVersion.md
```

## Common Tasks

### Adding a New Sample

1. Create `.xaml` and `.xaml.cs` files in `/AIDevGallery/Samples/`
2. Annotate the class with `[GallerySample]` attribute
3. If using a new model, add `.model.json` to `ModelsDefinitions/`
4. Override `OnNavigatedTo` to handle model loading if needed
5. Implement proper cleanup in `Unloaded` event
6. Follow existing sample patterns for UI consistency

### Modifying UI Components

1. Check `/Styles/Darbot/` for existing style resources
2. Use established Darbot color brushes and gradients
3. Follow the 8-point grid system for spacing
4. Reference `StyleGuide.md` for design specifications
5. Test in both light and high-contrast modes if applicable

### Working with AI Models

- Models are downloaded locally via Hugging Face integration
- Model paths are provided through `SampleNavigationParameters`
- Use `params.RequestWaitForCompletion()` and `params.NotifyCompletion()` for async model loading
- Always dispose of model resources properly

## Important Constraints

### What to Avoid

- ❌ **Never use AnyCPU as build target** - causes WinUI build failures
- ❌ **Don't break the Darbot design system** - maintain visual consistency
- ❌ **Avoid hardcoded paths** - use `Package.Current.InstalledLocation.Path`
- ❌ **Don't add production code to this research fork** - this is experimental
- ❌ **No unnecessary external dependencies** - keep the dependency footprint small

### Research Fork Notice

This is a research fork for experimental purposes:
- No functionality is guaranteed
- May contain bugs or incomplete features
- Breaking changes can occur without notice
- Not suitable for production use

When making contributions, prioritize:
1. Design system development and refinement
2. AI sample functionality
3. Code quality and maintainability
4. Documentation updates

## Special Considerations

### Windows Copilot Runtime APIs

Some samples use Windows Copilot Runtime APIs:
- `Microsoft.Windows.AI.Generative` for language models (Phi Silica)
- `Microsoft.Windows.Vision` for image processing (OCR, image description)
- These APIs are platform-specific and require appropriate feature detection

### ARM64 Support

- ARM64 builds are required for Copilot+ PC features
- Always test ARM64-specific functionality on appropriate hardware
- Use architecture detection in build scripts

### Telemetry

- The app includes basic telemetry logging
- Follow privacy guidelines when adding telemetry points
- Reference Microsoft privacy statement

## Getting Help

- **Setup Issues**: See `SETUP_GUIDE.md` for comprehensive setup instructions
- **Sample Development**: Reference `docs/AddingSamples.md`
- **Design System**: See `Darbot/README.md` and `Darbot/StyleGuide.md`
- **Issue Reporting**: Open issues at https://github.com/darbotlabs/darbot-dev/issues

## Contributing Guidelines

This project welcomes contributions related to:
- Darbot design system development
- AI sample improvements
- Documentation enhancements
- Bug fixes and stability improvements

Contributions require agreeing to a Contributor License Agreement (CLA).

The project follows the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).

---

*© Darbot Labs 2025 - Research Fork*

> For the official Microsoft AI Dev Gallery, visit: https://github.com/microsoft/ai-dev-gallery

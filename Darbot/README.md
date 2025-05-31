# Darbot Labs Cyber-Retro Design System

> **Part of Darbot Dev** - A research fork for design system development and testing

## Overview

The Darbot Labs Cyber-Retro Design System is a modern design language that blends cyberpunk aesthetics with a retro-futuristic feel, specifically designed for use in AI-powered developer tools like Darbot Dev (formerly AI Dev Gallery). This design system provides components, styles, and guidelines to create consistent, visually striking, and functional user interfaces.

This design system is currently in active development as part of the Darbot Dev research project. **Components and APIs may change without notice.**

## Key Characteristics

- **Gradient Accents**: Vibrant gradient effects using purple (#B54CFF), blue (#0078FF), and green (#00E0B4)
- **Dark Backgrounds**: Rich, dark backdrops for optimal contrast and reduced eye strain
- **Cyber-Retro Typography**: Clean, technical-feeling typography with modern proportions
- **Geometric Shapes**: Consistent corner radii and defined borders for UI elements
- **8-Point Grid**: Consistent spacing based on multiples of 8 pixels

## Getting Started

### Using Darbot Styles in Your XAML

1. Import the styles in your XAML file:

```xaml
<Page.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="/Styles/Darbot/DarbotTheme.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Page.Resources>
```

2. Apply Darbot styles to your elements:

```xaml
<Button Content="Primary Action" Style="{StaticResource DarbotAccentButtonStyle}" />
<TextBlock Text="Heading" Style="{StaticResource DarbotHeadingTextBlockStyle}" Foreground="{StaticResource DarbotTextPrimaryBrush}" />
<Border Style="{StaticResource DarbotCardBorderStyle}" />
```

### Component Library

#### Colors and Brushes

- **Primary Colors**:
  - `DarbotPurpleBrush` - #B54CFF
  - `DarbotBlueBrush` - #0078FF
  - `DarbotGreenBrush` - #00E0B4

- **Gradients**:
  - `DarbotPurpleToBlueGradient`
  - `DarbotBlueToGreenGradient`
  - `DarbotPurpleToGreenGradient`
  - `DarbotBrandGradientBrush` (3-color brand gradient)

- **Background Colors**:
  - `DarbotDarkBackgroundBrush` - #0A0A0B
  - `DarbotMediumBackgroundBrush` - #1A1A1D
  - `DarbotLightBackgroundBrush` - #2A2A2F

- **Text Colors**:
  - `DarbotTextPrimaryBrush` - #FFFFFF
  - `DarbotTextSecondaryBrush` - #CCCCCC
  - `DarbotTextMutedBrush` - #888888

- **Semantic Colors**:
  - `DarbotSuccessBrush` - #00E0B4
  - `DarbotWarningBrush` - #FFAD33
  - `DarbotErrorBrush` - #FF4444
  - `DarbotInfoBrush` - #0078FF

#### Typography

Apply these styles to TextBlock elements:

- `DarbotDisplayTextBlockStyle` - 36px, SemiBold (Large titles)
- `DarbotHeadingTextBlockStyle` - 28px, SemiBold (Section headers)
- `DarbotBodyTextBlockStyle` - 16px, Normal (Body text)
- `DarbotBodyStrongTextBlockStyle` - 16px, SemiBold (Emphasized body text)
- `DarbotCodeTextBlockStyle` - 14px, Consolas (Code display)

#### Buttons

- `DarbotAccentButtonStyle` - Primary action with gradient background
- `DarbotSecondaryButtonStyle` - Secondary action with border
- `DarbotTertiaryButtonStyle` - Text-only button
- `DarbotFadedButtonStyle` - Semi-transparent gradient button

#### Cards

- `DarbotCardStyle` - Standard card
- `DarbotElevatedCardStyle` - Card with acrylic backdrop effect
- `DarbotFeatureCardStyle` - Card with gradient border
- `DarbotCardBorderStyle` - Standard card border
- `DarbotCardAccentBorderStyle` - Card with accent border

### Building with Architecture Support

To avoid AnyCPU build issues with WinUI components, use the provided build script:

```powershell
.\build.ps1
```

This will automatically detect your system architecture and build using the correct platform target (x64, x86, or ARM64).

## Design System Files

- `DarbotTheme.xaml` - Master import file for all Darbot styles
- `DarbotColors.xaml` - Color definitions and brushes
- `DarbotCompatibility.xaml` - Compatibility layer for WinUI integration
- `DarbotButtons.xaml` - Button component styles
- `DarbotTypography.xaml` - Text styles and fonts
- `DarbotCards.xaml` - Card component styles

## Implementation Status

For detailed information on the implementation status, remaining tasks, and roadmap, see the [Status.md](./Status.md) document.

## Design Guidelines

The complete design specifications can be found in [StyleGuide.md](./StyleGuide.md).

---

*© Darbot Labs 2025 - Research & Development*

> **Note**: This design system is part of the Darbot Dev research project. Use in production applications is not recommended. For stable design systems, consider Microsoft's Fluent UI or other established frameworks.

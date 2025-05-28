# Darbot Labs — Cyber‑Retro × Fluent 2 Style Guide (Windows AI Dev App Edition)

> **Version:** 1.1  | **Updated:** 2025‑05‑27
>
> Applies to: **Darbot Vision Management**, **Darbot Studio / Winflow**, **Windows AI Dev App**
>
> Use this document as the *single source of truth* for UI/UX decisions across XAML, WinUI 3, Web, and Desktop shells.

---

## 1 • Core Design Principles

|  Principle             |  Practical meaning                                                 |
| ---------------------- | ------------------------------------------------------------------ |
| **Cyber‑Retro**        | Neon accents, CRT‑dark surfaces, subtle glow & pixel‑perfect grid. |
| **Fluent 2**           | Large radii, acrylic «glass», depth via elevation & subtle motion. |
| **WinUI Consistency**  | Respect system theme, input modalities, high‑contrast.             |
| **Modern Minimalism**  | Clear hierarchy, zero ornamental noise, 8‑pt spacing grid.         |

---

## 2 • Color System

### 2.1 Token Palette

Place these in top‑level **`Colors.xaml`** *and* in Web `:root`.

```xaml
<!-- Brand Gradient -->
<SolidColorBrush x:Key="DarbotPurpleBrush" Color="#B54CFF"/>
<SolidColorBrush x:Key="DarbotNitrousBlueBrush" Color="#0078FF"/>
<SolidColorBrush x:Key="DarbotTealBrush" Color="#00E0B4"/>
<LinearGradientBrush x:Key="BrandGradientBrush" StartPoint="0,0" EndPoint="1,1">
  <GradientStop Color="#B54CFF" Offset="0"/>
  <GradientStop Color="#0078FF" Offset="0.4"/>
  <GradientStop Color="#00E0B4" Offset="1"/>
</LinearGradientBrush>

<!-- Surfaces & Text -->
<SolidColorBrush x:Key="SurfaceBrush" Color="#0E0F13"/>
<SolidColorBrush x:Key="CardAcrylicBrush" Color="#66000000" /> <!-- 04 alpha -->
<SolidColorBrush x:Key="BorderBrush" Color="#14FFFFFF" />
<SolidColorBrush x:Key="TextMainBrush" Color="#F4F4F6" />
<SolidColorBrush x:Key="TextSubBrush" Color="#A0A4B8" />

<!-- Semantic -->
<SolidColorBrush x:Key="SuccessBrush" Color="#1DB954"/>
<SolidColorBrush x:Key="WarningBrush" Color="#FFB020"/>
<SolidColorBrush x:Key="ErrorBrush"   Color="#FF4D4F"/>
```

```css
:root {
  --g-start:#b54cff;  --g-mid:#0078ff;  --g-end:#00e0b4;
  --bg-surface:#0e0f13;
  --card-bg:rgba(255,255,255,.04);
  --card-br:rgba(255,255,255,.08);
  --txt-main:#f4f4f6; --txt-sub:#a0a4b8;
  --success:#1db954;  --warning:#ffb020; --error:#ff4d4f;
}
```

### 2.2 Theme Variants

* **Light Theme** – raise surface luminance by +8 L ⋆, keep gradient stops.
* **High‑Contrast** – replace gradient with `SystemAccentColor` for stops; switch text/on‑surface tokens to `HighContrast*`.

---

## 3 • Typography

|  Role       |  WinUI Style                |  Web size (rem)  |  Weight  |
| ----------- | --------------------------- | ---------------- | -------- |
| Display‑1   | `TitleLargeTextBlockStyle`  | 2.25             |  600     |
| Heading‑2   | `SubtitleTextBlockStyle`    | 1.75             |  600     |
| Body        | `BodyTextBlockStyle`        | 1.00             |  400     |
| Body‑Strong | `BodyStrongTextBlockStyle`  | 1.00             |  600     |
| Code/Small  | custom `CodeTextBlockStyle` |  0.875           |  400     |

> **Tip:** Always bind font to `ContentControl.FontFamily` with *Segoe UI Variable* where available for optical size.

---

## 4 • Spacing & Grid

* **8‑pt grid** — multiples of 4 px allowed only for iconography.
* XAML: use `Thickness="8"`, `Margin="0,12,0,0"` etc.
* Web: tokenised spacing scale

```css
:root{ --s-1:4px; --s-2:8px; --s-3:12px; --s-4:16px; --s-5:24px; --s-6:32px; }
```

---

## 5 • Corner Radius

|  Token                |  Value  |  Usage                  |
| --------------------- | ------- | ----------------------- |
| `OverlayCornerRadius` | 12 px   | Flyouts, cards, dialogs |
| `ControlCornerRadius` | 8 px    | Buttons, inputs         |
| `AvatarCornerRadius`  | 50%     | Circular images         |

---

## 6 • Component Library

### 6.1 Buttons (`Button.xaml`)

| Variant        |  Style Key                     |  Notes                                       |
| -------------- | ------------------------------ | -------------------------------------------- |
| **Primary**    | `AccentButtonStyle`            | Gradient background (`BrandGradientBrush`)   |
| **Secondary**  | `SecondaryButtonStyle`         | Transparent bg, border `BorderBrush`         |
| **Tertiary**   | `TertiaryButtonStyle`          | Text only, hover underline                   |
| **Faded**      | `FadedButtonStyle`             | 20 % brand gradient overlay for low emphasis |

> **Padding:** `Padding="12,6"` · **Icon gap:** 8 px

### 6.2 ComboBox (`ComboBox.xaml`)

* Corner radius = `ControlCornerRadius`
* Dropdown uses `SurfaceBrush` with acrylic blur
* Focus visual: 2 px brand gradient outline

### 6.3 NavigationView (`NavigationView.xaml`)

* Top rail background `SurfaceBrush` (90 % opacity)
* Selected item pill uses gradient → `BrandGradientBrush`
* Compact mode icons 24 px grid

### 6.4 SelectorBar / Tabs

* Template uses `Border.Bottom` 2 px
* Active brush: `DarbotTealBrush`

### 6.5 Cards (`Card.xaml`)

```xaml
<Border Style="{StaticResource CardStyle}" > … </Border>
```

* Acrylic backdrop (#04 alpha) + blur radius 30
* Shadow: Elevation 4 (`ShadowDepth="4"`)

### 6.6 Header with Gradient Overlay

* Grid row height = 96 px
* Background: `BrandGradientBrush` + diagonal clip path

### 6.7 Shimmer Placeholder (`Shimmer.xaml`)

* Uses `CardGradient1Brush` ➔ `CardGradient2Brush` keyframe animation 2 s linear infinite.

---

## 7 • Effects & Motion

| Effect               | Implementation                                                                 |
| -------------------- | ------------------------------------------------------------------------------ |
| **Glass acrylic**    | `AcrylicBrush` tint `#66000000`, `FallbackColor` `#1AFFFFFF` for no‑blur envs. |
| **Shadow**           | `Elevation 4–8` for flyouts/dialogs.                                           |
| **Focus**            | 2 px gradient outline; High‑contrast uses `SystemFocusVisualSolidColorBrush`.  |
| **Hover brightness** | Web: `filter:brightness(1.12)`; XAML: `ThemeAnimation` opacity 0.08.           |

---

## 8 • Theme & High Contrast Support

* Bind all brushes to `ThemeResource` wrappers for automatic theme swap.
* Provide HC ResourceDictionary with solid colors meeting **WCAG 2.1 ≥ 7:1**.

---

## 9 • Code Style (C# / XAML)

| Aspect         | Guideline                                                        |
| -------------- | ---------------------------------------------------------------- |
| **C# naming**  | Public `PascalCase`, private `_camelCase`, async suffix `Async`. |
| **XAML names** | `x:Name="PascalCaseThing"` for code‑behind refs.                 |
| **Folder org** | Feature‑based root › `Views/`, `ViewModels/`, `Styles/`.         |
| **XML Docs**   | Mandatory on public APIs.                                        |

---

## 10 • Accessibility

* Keyboard: ensure `TabIndex` order matches visual order.
* Screen reader: supply `AutomationProperties.Name`.
* Motion: respect `prefers-reduced-motion` / `UISettings.AnimationsEnabled`.

---

## 11 • Assets

| File              | Use                                   |
| ----------------- | ------------------------------------- |
| `darbot-logo.svg` | App & dock icon (".>" D mark)         |
| `bg-grid.svg`     | Optional dark CRT overlay, 8% opacity |

---

## 12 • Import / Reference Cheat‑Sheet

```xaml
<ResourceDictionary.MergedDictionaries>
  <XamlControlsResources xmlns="using:Microsoft.UI.Xaml.Controls"/>
  <ResourceDictionary Source="/Styles/Colors.xaml"/>
  <ResourceDictionary Source="/Styles/Button.xaml"/>
  <ResourceDictionary Source="/Styles/Card.xaml"/>
  …
</ResourceDictionary.MergedDictionaries>
```

```scss
@use "tokens" as *;
@use "components/buttons";
@use "components/cards";
```

---

*Crafted with ❤︎ by Darbot Labs · Contact `@DesignOps` for questions or PR reviews.*

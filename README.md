![WindowsAI hero image](docs/images/header.png)

<h1 align="center">
    Darbot Dev
</h1>

> [!IMPORTANT]  
> **This is a research fork** of the original Microsoft AI Dev Gallery for experimental and testing purposes only. **No functionality is guaranteed.** This project is maintained by Darbot Labs for research and development of the Darbot Cyber-Retro Design System. For the official Microsoft AI Dev Gallery, visit the [original repository](https://github.com/microsoft/ai-dev-gallery).

Darbot Dev is a Windows developer tool that integrates AI capabilities with the Darbot Labs Cyber-Retro Design System. It includes:

- **Explore interactive samples powered by local AI models with Darbot's cyber-retro design**
- **Browse, download, and run models from Hugging Face and GitHub**
- **View C# source code and export standalone Visual Studio projects with a single click**
- **Experience the Darbot Labs Cyber-Retro Design System in action**

<p align="center">
<img src="docs/images/HeroImage1.png" alt="Darbot Dev" width="600"/>
</p>

> [!WARNING]
> **Research & Testing Only**: This fork is provided as-is for research and experimentation with the Darbot design system. It may contain bugs, incomplete features, or breaking changes. Use at your own discretion.

## 🚀 Getting started
This is a research fork. For a stable experience, please use the [official Microsoft AI Dev Gallery from the Microsoft Store](http://aka.ms/ai-dev-gallery-store). To build this research version:

### 1. Set up the environment

>**⚠️ Note**: Darbot Dev requires [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or later for building and Windows 10 or newer to run.
If you're new to building apps with WinUI and the Windows App SDK, follow the [installation instructions](https://learn.microsoft.com/windows/apps/get-started/start-here).

**Required [Visual Studio components](https://learn.microsoft.com/windows/apps/get-started/start-here?tabs=vs-2022-17-10#required-workloads-and-components):**
- Windows application development

### 2. Clone the repository

```shell
git clone https://github.com/darbotlabs/darbot-dev.git
```

### 3. Open AIDevGallery.sln with Visual Studio!

Ensure that the `AIDevGallery` project is set as the startup project in Visual Studio.

Press <kbd>F5</kbd> to run Darbot Dev!

>**⚠️ Note**: On ARM64-based Copilot+ PCs, make sure to build and run the solution as `ARM64` (and not as `x64`). This is required especially when running the samples that invoke the Windows Copilot Runtime to communicate with models such as Phi Silica.

>**⚠️ Note**: Having issues installing the app on your machine? Let us know by <a href="https://github.com/darbotlabs/darbot-dev/issues">opening an issue </a> and our team will do our best to help!

<br/>

## 🎨 Darbot Cyber-Retro Design System

This fork showcases the Darbot Labs Cyber-Retro Design System, featuring:

- **Gradient Accents**: Vibrant purple, blue, and green gradients
- **Dark Cyber Aesthetics**: Rich, dark backgrounds with neon highlights
- **Retro-Futuristic Typography**: Clean, technical fonts with modern proportions
- **Geometric Components**: Consistent corner radii and defined borders
- **8-Point Grid System**: Precise spacing for pixel-perfect layouts

For detailed design documentation, see the [Darbot Design System README](./Darbot/README.md) and [Style Guide](./Darbot/StyleGuide.md).

## 💻 Device requirements
- **Minimum OS version**: Windows 10, version 1809 (10.0; Build 17763)
- **Architecture**: x64, ARM64
- **Memory**: At least 16 GB is recommended
- **Disk space**: At least 20GB free space is recommended
- **GPU**: 8GB of VRAM is recommended for running samples on the GPU

## 👏 Contribute to Darbot Dev

This is a research fork focused on design system development. For general AI Dev Gallery improvements, please contribute to the [official Microsoft repository](https://github.com/microsoft/ai-dev-gallery).

For Darbot-specific improvements and design system feedback, feel free to <a href="https://github.com/darbotlabs/darbot-dev/issues">open an issue</a> or submit a PR.

<br/>

## ❓ FAQs
- **Q: Is this an official Microsoft product?**
  - **A:** No, this is an independent research fork by Darbot Labs. For the official app, visit the [Microsoft AI Dev Gallery](https://github.com/microsoft/ai-dev-gallery).
- **Q: Can I use this in production?**
  - **A:** No, this is for research and testing only. No functionality is guaranteed.
- **Q: Can I use the app without an internet connection?**
  - **A:** Yes, the app works offline since the AI models are downloaded locally. However, you will need to be online to download additional AI models from Hugging Face or GitHub.
- **Q: What AI models are available in the app?**
  - **A:** The app features popular open source models and will eventually include APIs from the <a href="https://learn.microsoft.com/windows/ai/overview">Windows Copilot Runtime</a>. When executing a sample, you can select which model you want to use.
- **Q: How does this differ from the original AI Dev Gallery?**
  - **A:** This fork focuses on showcasing and developing the Darbot Cyber-Retro Design System while maintaining core AI functionality.
- **Q: Where can I provide feedback?**
  - **A:** Feel free to give us feedback or [open an issue](https://github.com/darbotlabs/darbot-dev/issues/new) on our GitHub repository.

<br/>

## ✨ Contributing

This project welcomes contributions related to the Darbot design system and research goals. Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

<br/>

## 📚 Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft trademarks or logos is subject to and must follow [Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.

Darbot Labs and the Darbot logo are trademarks of Darbot Labs.

<br/>

## 🔔 Code of Conduct
This project has adopted the <a href="https://github.com/microsoft/AI-Dev-Gallery/blob/main/CODE_OF_CONDUCT.md"> Microsoft Open Source Code of Conduct</a>.

<br/>

## 📢 Privacy Statement
The application logs basic telemetry. Please read the <a href="http://go.microsoft.com/fwlink/?LinkId=521839"> Microsoft privacy statement</a> for more information. 

---

*© Darbot Labs 2025 - Research Fork*
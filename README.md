# Unity Package Assistant

**Made by [Bohdan Yavhusishyn](https://github.com/Scrag0)**

[![Unity Version](https://img.shields.io/badge/Unity-6000.0%2B-blue)](https://unity.com)
[![LICENSE](https://img.shields.io/badge/License-Apache--2.0-blue?logo=apache)](https://github.com/Scrag0/UnityPackageAssistant/blob/main/LICENSE)
[![GitHub Stars](https://img.shields.io/github/stars/Scrag0/UnityPackageAssistant)](https://github.com/Scrag0/UnityPackageAssistant/stargazers)
[![Issues](https://img.shields.io/github/issues/Scrag0/UnityPackageAssistant)](https://github.com/Scrag0/UnityPackageAssistant/issues)
[![Changelog](https://img.shields.io/badge/Changelog-grey)](https://github.com/Scrag0/UnityPackageAssistant/blob/main/CHANGELOG.md)

A Unity Editor tool for package publishing and deletion on personal Verdaccio server.

## 🚀 Features

- **Package publishing** - Updated version of package and publish them to Verdaccio 
- **Package deleting** - Deletes package by specific version. If it is the last version removes package completely from Verdaccio
- **Package comparison** - In order to identify unpublished changes to Verdaccio
- **Authentication** - Login and sign up to Verdaccio package manager by using Unity Editor Window
- **Session data save** - Saves manifest from all scoped registries in order to prevent multiple requests after Unity Editor script reloading
- **Editor window** - Dedicated window with access to features

## 📦 Third Party Dependencies

| Name                                                                                                           | Version | Source                | License                                                                                                              |
| -------------------------------------------------------------------------------------------------------------- | ------- | --------------------- | -------------------------------------------------------------------------------------------------------------------- |
| **[Tomlyn](https://www.nuget.org/packages/Tomlyn)**                                                            | 0.19.0  | NuGet                 | [BSD-2-Clause license](https://licenses.nuget.org/BSD-2-Clause)                                                      |
| **[NuGet.Versioning](https://www.nuget.org/packages/NuGet.Versioning)**                                        | 6.14.0  | NuGet                 | [Apache-2.0 license](https://licenses.nuget.org/Apache-2.0)                                                          |
| **[Newtonsoft Json](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@2.0/manual/index.html)** | 3.2.1   | Unity Package Manager | [Unity Technologies ApS](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@2.0/license/LICENSE.html) |
| **[SharpZipLib](https://docs.unity3d.com/Packages/com.unity.sharp-zip-lib@1.3/manual/index.html)**             | 1.3.9   | Unity Package Manager | [Unity Technologies ApS](https://docs.unity3d.com/Packages/com.unity.sharp-zip-lib@1.3/license/LICENSE.html)         |
| **[UniTask](https://github.com/Cysharp/UniTask)**                                                              | 2.5.10  | GitHub repository     | [The MIT License (MIT)](https://github.com/Cysharp/UniTask?tab=MIT-1-ov-file)                                        |

## 📋 Requirements

- Unity 6000.0 LTS or higher
- Manually added UniTask package

## 🛠️ Installation

### Option 1: Unity Package Manager (Recommended)
1. Open Unity Editor
2. Go to `Window > Package Manager`
3. Click `+` and select `Add package from git URL`
4. Enter: `https://github.com/Scrag0/UnityPackageAssistant.git#1.0.0`

### Option 2: Manual package adding
1. Open `Packages` folder
2. Find file manifest.json
3. Manually add line to dependencies:

```
  "dependencies": {
    "com.scrag0.unity-package-assistant": "https://github.com/Scrag0/UnityPackageAssistant.git#1.0.0",
    ...
```

### Option 3: Manual Installation
1. Download the latest release from [Releases](https://github.com/Scrag0/UnityPackageAssistant/releases)
2. Extract to your Unity project's `Packages` folder
3. Unity will automatically detect and import the package

## 🎯 Quick Start

1. In order to access remote server with Verdaccio you need to add scoped registries by two ways:
	1. First
		1. Open `Project Settings`
		2. Find tab `Package Manager`
		3. Click `+` in `Scoped Registries`
	2. Second
		1. Open `Window > Package Manager`
		2. Click on three dots In upper right corner
		3. Choose option `Project Settings
		4. Click `+` in `Scoped Registries`
2. Open Editor Window in  `Tools > Unity Package Assistant`
3. If scoped registry gives access to publishing and unpublishing features only for authorized users, click on button `Account`. Remark: `Remember me` toggle sets property `AlwaysAuth`.
4. Button `Packages` gives access to window with publish and unpublish features.
5. Click on button `Request` executes method, which retrieves data about packages from all scoped registries. Analyzed data is shown per scoped registries and in 4 categories:
	1. **Available** - Packages are only present in Verdaccio;
	2. **Common** - Packages are present in project and in Verdaccio;
	3. **Changed** - Packages are present in project and in Verdaccio, but there are some differences;
	4. **Installable** - Packages are only present in project.
6. Click on refresh button reinitializes loaded data and scripts. Button is present at both tabs `Account` and `Packages`.

> [!Warning]
> Package in project must be in embedded in order to be recognizable by tool
> P.s. They must be in folder `Packages`

## 🐛 Issues & Support

- **Bug Reports**: [Create an issue](https://github.com/Scrag0/UnityPackageAssistant/issues/new?template=bug_report.md)
- **Feature Requests**: [Create an issue](https://github.com/Scrag0/UnityPackageAssistant/issues/new?template=feature_request.md)

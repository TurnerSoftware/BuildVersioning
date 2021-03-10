# Build Versioning

Simple build versioning for .NET, powered by Git tags.

[![AppVeyor](https://img.shields.io/appveyor/ci/Turnerj/buildversioning/main.svg)](https://ci.appveyor.com/project/Turnerj/BuildVersioning)
[![Codecov](https://img.shields.io/codecov/c/github/turnersoftware/BuildVersioning/main.svg)](https://codecov.io/gh/TurnerSoftware/BuildVersioning)
[![NuGet](https://img.shields.io/nuget/v/TurnerSoftware.BuildVersioning.svg)](https://www.nuget.org/packages/TurnerSoftware.BuildVersioning/)


## Overview

Inspired by [MinVer](https://github.com/adamralph/minver), Build Versioning is a different attempt at the same problem - to make versioning simple.
The simplicity comes from how the version strings are generated and the built-in integrations.

## 📖 Table of Contents
- [Getting Started](#Getting-Started)
  - [Example Versions](#Example-Versions)
- [CI Versioning Integrations](#CI-Versioning-Integrations)
- [Customizing Version Strings](#Customizing-Version-Strings)
  - [Formatting Tags](#Formatting-Tags)
  - [Version Strings](#Version-Strings)
- [Additional Settings](#Additional-Settings)

## ⭐ Getting Started

1. [Install Build Versioning](https://www.nuget.org/packages/TurnerSoftware.BuildVersioning/)<br>
```powershell
PM> Install-Package TurnerSoftware.BuildVersioning
```
2. There is no second step - *you're done!*

The version information is extracted from the current state of the Git repository.
From a tag that is [SemVer v2.0](https://semver.org/spec/v2.0.0.html) compliant, it can extract the major, minor, patch, pre-release and build metadata information.
This information is then fed through a formatting system to generate specific [version strings](#Version-Strings).

Additional information is provided from Git directly including the commit height (number of commits since the last tag) and the commit hash itself.

### Example Versions

These examples use the default configuration after installing Build Versioning.

|Example|Git Tag|Commit Height|Full Version|File Version|Assembly Version|
|-|:-:|:-:|:-:|:-:|:-:|
|New Release                        |1.2.4      |0|1.2.4+a4f31ea                    |1.2.4.0|1.0.0.0|
|New Pre-Release                    |1.2.4-alpha|0|1.2.4-alpha+a4f31ea              |1.2.4.0|1.0.0.0|
|Main Branch / Active Development   |1.2.4      |4|1.2.4-dev.4+a4f31ea              |1.2.4.0|1.0.0.0|
|Non-PR Commit via GitHub Actions   |1.2.4      |4|1.2.4-dev.4+a4f31ea-github.432515|1.2.4.0|1.0.0.0|
|PR Commit via GitHub Actions       |1.2.4      |4|1.2.4-pr.17+a4f31ea-github.432515|1.2.4.0|1.0.0.0|

## 🛠 CI Versioning Integrations

By default, Build Versioning provides rich pre-release and build metadata from the current CI environment.
For pull requests, this will automatically have a pre-release defined which will include the PR number (eg. `1.2.4-pr.17`).
For all commits, the build metadata will include the CI environment and a relevant build identifier (eg. `1.2.4+a4f31ea-github.432515`).

### Default Integrations

|Integration|Configuration Tag|Notes|
|-|-|-|
|[GitHub Actions](https://github.com/features/actions)|`<BuildVersioningWithGitHub>`|GitHub Actions don't fetch tags by default. This integration will perform a `git fetch` for tags automatically when building your application.|
|[Azure DevOps](https://azure.microsoft.com/en-us/services/devops/pipelines/)|`<BuildVersioningWithAzureDevOps>`||
|[AppVeyor](https://www.appveyor.com/)|`<BuildVersioningWithAppVeyor>`||

### Disabling an Integration

Each integration can be individually disabled through configuration. For example, include the following in your project file to disable the GitHub Actions integration:

```xml
<BuildVersioningWithGitHub>false</BuildVersioningWithGitHub>
```

## ✏ Customizing Version Strings

### Formatting Tags

These are formatting tags available for you to use for customizing your version strings. 

|Tag|Notes|
|-|-|
|`{Major}`|The major version retrieved from the Git tag. If there are no tags available, defaults to `0`.|
|`{Major++}`|The major version retrieved from the Git tag incremented by 1. If this is a tagged release, the value will return the major version without increment.|
|`{Minor}`|The minor version retrieved from the Git tag. If there are no tags available, defaults to `0`.|
|`{Minor++}`|The minor version retrieved from the Git tag incremented by 1. If this is a tagged release, the value will return the minor version without increment.|
|`{Patch}`|The patch version retrieved from the Git tag. If there are no tags available, defaults to `0`.|
|`{Patch++}`|The patch version retrieved from the Git tag incremented by 1. If this is a tagged release, the value will return the patch version without increment.|
|`{CommitHeight}`|The number of commits since the last tag. If there are no tags available, defaults to `0`.|
|`{CommitHash}`|The first 7 characters of the most recent commit hash.|

Additionally, the full version string supports two additional formatting tags.

|Tag|Default Value|Configuration Tag|Description|
|-|-|-|-|
|`{PreRelease}`|`dev.{CommitHeight}`|`<BuildPreReleaseFormat>`|The pre-release portion of the version. This will include the leading dash (`-`) if a pre-release is defined, otherwise blank. The value is overridden by the Git tag if this is a tagged release.|
|`{BuildMetadata}`|`{CommitHash}`|`<BuildMetadataFormat>`|The build metadata portion of the version. This will include the leading plus (`+`) if build metadata is defined, otherwise blank. The value is overridden by the Git tag if this is a tagged release and is defined in the tag.|


### Version Strings

|Name|Configuration Tag|Default Value|
|-|-|-|
|📦 **Full Version**<br>aka. the "package" or "product" version, it is used for versioning the package itself and displayed in NuGet.|`<BuildFullVersionFormat>`|`{Major}.{Minor}.{Patch}{PreRelease}{BuildMetadata}`|
|📄 **File Version**<br>A superficial version number, displayed by the OS. This is not used by the .NET runtime.|`<BuildFileVersionFormat>`|`{Major}.{Minor}.{Patch}.0`|
|⚙ **Assembly Version**<br>Used by .NET for referencing the assembly when strong-named signing is enabled. Updating this by major version is advised.|`<BuildAssemblyVersionFormat>`|`{Major}.0.0.0`|

For more information on file version vs assembly version, [see the MSDN docs](https://docs.microsoft.com/en-us/troubleshoot/visualstudio/general/assembly-version-assembly-file-version).

## 🎛 Additonal Settings

### Disabling Build Versioning

You can disable build versioning by setting `<SkipBuildVersioning>` in your project file to `true`.

### Enable Output Logging

You can enable output logging for Build Versioning by specifying `<BuildVersioningLogLevel>` as `high`.
# Build Versioning
Simple build versioning, powered by Git tags, for .NET

## Overview

Inspired by [MinVer](https://github.com/adamralph/minver), BuildVersioning is a different attempt at the same problem - to make versioning simple.

To control the build version, you specify "formats" in your `*.csproj` files using the follow tags:

- `<BuildFullVersionFormat>`
- `<BuildFileVersionFormat>`
- `<BuildAssemblyVersionFormat>`
- `<BuildPreReleaseFormat>`
- `<BuildMetadataFormat>`

Within these tags, you can use special tokens that are replaced with their appropriate values, derived from the latest Git tag.

- `{Major}`
- `{Minor}`
- `{Patch}`
- `{CommitHeight}`
- `{CommitHash}`

For `<BuildFullVersionFormat>`, there are additional tokens:

- `{PreRelease}`: Derived from the format of `<BuildPreReleaseFormat>`. Includes the leading `-`. Will be blank if there is no pre-release.
- `{BuildMetadata}`: Derived from the format of `<BuildMetadataFormat>`. Includes the leading `+`. Will be blank if there is no build metadata.

More information coming soon...
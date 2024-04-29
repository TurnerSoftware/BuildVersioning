using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using TurnerSoftware.BuildVersioning.Tool;

var rootCommand = new RootCommand
{
	new Option<string>("--full-version-format")
	{
		IsRequired = true,
		Description = "The string to format for the full version."
	},
	new Option<string>("--file-version-format")
	{
		IsRequired = true,
		Description = "The string to format for the file version."
	},
	new Option<string>("--assembly-version-format")
	{
		IsRequired = true,
		Description = "The string to format for the assembly version."
	},
	new Option<string>("--prerelease-format", () => string.Empty)
	{
		Description = "The string to format for the pre-release."
	},
	new Option<string>("--build-metadata-format", () => string.Empty)
	{
		Description = "The string to format for the build metadata."
	}
};

rootCommand.Description = "Build Versioning Tool";

rootCommand.Handler = CommandHandler.Create<string, string, string, string, string>((fullVersionFormat, fileVersionFormat, assemblyVersionFormat, preReleaseFormat, buildMetadataFormat) =>
{
	var buildVersioner = new BuildVersioner(new VersionDetailsProvider(new GitCommandRunner()));
	var buildVersion = buildVersioner.GetBuildVersion(new BuildVersioningOptions
	{
		FullVersionFormat = fullVersionFormat,
		FileVersionFormat = fileVersionFormat,
		AssemblyVersionFormat = assemblyVersionFormat,
		PreReleaseFormat = preReleaseFormat,
		BuildMetadataFormat = buildMetadataFormat
	});

	Console.WriteLine($"{buildVersion.FullVersion};{buildVersion.FileVersion};{buildVersion.AssemblyVersion}");
	return 0;
});

return rootCommand.InvokeAsync(args).Result;

using System;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace TurnerSoftware.BuildVersioning.Tool
{
	class Program
	{
		static int Main(string[] args)
		{
			var rootCommand = new RootCommand
			{
				new Option<string>("--full-version-format")
				{
					Required = true,
					Description = "The string to format for the full version."
				},
				new Option<string>("--file-version-format")
				{
					Required = true,
					Description = "The string to format for the file version."
				},
				new Option<string>("--assembly-version-format")
				{
					Required = true,
					Description = "The string to format for the assembly version."
				},
				new Option<string>("--prerelease-format", defaultValue: string.Empty)
				{
					Description = "The string to format for the pre-release."
				},
				new Option<string>("--build-metadata-format", defaultValue: string.Empty)
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
		}
	}
}

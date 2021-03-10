using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TurnerSoftware.BuildVersioning.Tool
{
	class Program
	{
		/// <summary>
		/// Parses the value from `git describe --tags --abbrev=7 --always` into specific version and commit information.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Format with tag: {tag}-{commitHeight}-{commitHash}<br/>
		/// Format without tag: {commitHash}
		/// </para>
		/// <para>
		/// Tag format: {major}.{minor}.{patch}{-preRelease}{+buildMetadata}<br/>
		/// Tag can have a prefix which will be ignored.
		/// </para>
		/// </remarks>
		private static readonly Regex GitDescribeParser = new(@"(?:[a-z. ]+)?(?<major>\d+).(?<minor>\d+).(?<patch>\d+)(?:-(?<preRelease>[a-z0-9][a-z0-9-.]+))?(?:\+(?<buildMetadata>[a-z0-9][a-z0-9-.]+))?-(?<commitHeight>\d+)-(?<commitHash>\w+)|(?<commitHash>\w+)", RegexOptions.IgnoreCase);

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
				var versionDetails = GetVersionDetails();
				if (versionDetails is null)
				{
					return -1;
				}

				if (!versionDetails.IsTaggedRelease && versionDetails.PreRelease is null && preReleaseFormat.Length > 0)
				{
					versionDetails = versionDetails with
					{
						PreRelease = preReleaseFormat
							.Replace("{CommitHeight}", versionDetails.CommitHeight.ToString())
					};
				}

				if (versionDetails.BuildMetadata is null && buildMetadataFormat.Length > 0)
				{
					versionDetails = versionDetails with
					{
						BuildMetadata = buildMetadataFormat
							.Replace("{CommitHash}", versionDetails.CommitHash)
							.Replace("{CommitHeight}", versionDetails.CommitHeight.ToString())
					};
				}

				var fullVersion = FormatVersion(fullVersionFormat, versionDetails)
					.Replace("{PreRelease}", versionDetails.PreRelease is null ? default : $"-{versionDetails.PreRelease}")
					.Replace("{BuildMetadata}", versionDetails.BuildMetadata is null ? default : $"+{versionDetails.BuildMetadata}");
				var fileVersion = FormatVersion(fileVersionFormat, versionDetails);
				var assemblyVersion = FormatVersion(assemblyVersionFormat, versionDetails);

				Console.WriteLine($"{fullVersion};{fileVersion};{assemblyVersion}");
				return 0;
			});

			return rootCommand.InvokeAsync(args).Result;
		}

		private static string FormatVersion(string format, VersionDetails versionDetails)
		{
			var autoIncrement = versionDetails.CommitHeight > 0 ? 1 : 0;
			return format
				.Replace("{Major}", versionDetails.MajorVersion.ToString())
				.Replace("{Major++}", (versionDetails.MajorVersion + autoIncrement).ToString())
				.Replace("{Minor}", versionDetails.MinorVersion.ToString())
				.Replace("{Minor++}", (versionDetails.MinorVersion + autoIncrement).ToString())
				.Replace("{Patch}", versionDetails.PatchVersion.ToString())
				.Replace("{Patch++}", (versionDetails.PatchVersion + autoIncrement).ToString())
				.Replace("{CommitHeight}", versionDetails.CommitHeight.ToString())
				.Replace("{CommitHash}", versionDetails.CommitHash ?? "NOCANDO");
		}

		private static VersionDetails GetVersionDetails()
		{
			var gitDetails = RunGitDescribe();
			if (gitDetails is null)
			{
				return null;
			}

			var matchedGroups = GitDescribeParser.Match(gitDetails).Groups;

			if (matchedGroups["major"].Success)
			{
				return new VersionDetails
				{
					MajorVersion = int.Parse(matchedGroups["major"].Value),
					MinorVersion = int.Parse(matchedGroups["minor"].Value),
					PatchVersion = int.Parse(matchedGroups["patch"].Value),
					PreRelease = matchedGroups.TryGetValue("preRelease", out var preReleaseGroup) ? preReleaseGroup.Value : default,
					BuildMetadata = matchedGroups.TryGetValue("buildMetadata", out var buildMetadataGroup) ? buildMetadataGroup.Value : default,
					CommitHeight = int.Parse(matchedGroups["commitHeight"].Value),
					IsTaggedRelease = int.Parse(matchedGroups["commitHeight"].Value) == 0,
					CommitHash = matchedGroups["commitHash"].Value
				};
			}
			else
			{
				return new VersionDetails
				{
					CommitHash = matchedGroups["commitHash"].Value
				};
			}
		}

		/// <summary>
		/// Returns a result from `git describe` containing the tag name, number of commits from the tag (commit height) and a 7-character commit hash.
		/// </summary>
		/// <remarks>
		/// Format with tag: {tag}-{commitHeight}-{commitHash}<br />
		/// Format without tag: {commitHash}
		/// </remarks>
		private static string RunGitDescribe()
		{
			using (var process = new System.Diagnostics.Process())
			{
				process.StartInfo = new ProcessStartInfo("git", "describe --tags --abbrev=7 --always")
				{
					RedirectStandardOutput = true
				};

				var waitOnExit = new TaskCompletionSource<object>();
				process.Exited += (s, e) => waitOnExit.SetResult(default);
				process.EnableRaisingEvents = true;

				try
				{
					process.Start();
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine(ex.Message);
					return null;
				}

				var standardOutputTask = process.StandardOutput.ReadToEndAsync();

				Task.WaitAll(waitOnExit.Task, standardOutputTask);

				if (process.ExitCode != 0)
				{
					return null;
				}

				return standardOutputTask.Result;
			}
		}
	}
}

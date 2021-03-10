namespace TurnerSoftware.BuildVersioning.Tool
{
	internal class BuildVersioner
	{
		private IVersionDetailsProvider VersionDetailsProvider { get; }

		public BuildVersioner(IVersionDetailsProvider versionDetailsProvider)
		{
			VersionDetailsProvider = versionDetailsProvider;
		}

		public BuildVersion GetBuildVersion(BuildVersioningOptions options)
		{
			var versionDetails = VersionDetailsProvider.GetVersionDetails();
			if (versionDetails is null)
			{
				return null;
			}

			if (!versionDetails.IsTaggedRelease && versionDetails.PreRelease is null && options.PreReleaseFormat.Length > 0)
			{
				versionDetails = versionDetails with
				{
					PreRelease = options.PreReleaseFormat
						.Replace("{CommitHeight}", versionDetails.CommitHeight.ToString())
				};
			}

			if (versionDetails.BuildMetadata is null && options.BuildMetadataFormat.Length > 0)
			{
				versionDetails = versionDetails with
				{
					BuildMetadata = options.BuildMetadataFormat
						.Replace("{CommitHash}", versionDetails.CommitHash)
						.Replace("{CommitHeight}", versionDetails.CommitHeight.ToString())
				};
			}

			var fullVersion = FormatVersion(options.FullVersionFormat, versionDetails)
				.Replace("{PreRelease}", versionDetails.PreRelease is null ? default : $"-{versionDetails.PreRelease}")
				.Replace("{BuildMetadata}", versionDetails.BuildMetadata is null ? default : $"+{versionDetails.BuildMetadata}");
			var fileVersion = FormatVersion(options.FileVersionFormat, versionDetails);
			var assemblyVersion = FormatVersion(options.AssemblyVersionFormat, versionDetails);

			return new BuildVersion
			{
				FullVersion = fullVersion,
				FileVersion = fileVersion,
				AssemblyVersion = assemblyVersion
			};
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
	}
}

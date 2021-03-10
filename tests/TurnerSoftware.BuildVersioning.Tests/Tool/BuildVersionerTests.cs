using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TurnerSoftware.BuildVersioning.Tool;

namespace TurnerSoftware.BuildVersioning.Tests.Tool
{
	[TestClass]
	public class BuildVersionerTests
	{
		private static readonly BuildVersioningOptions DefaultBuildVersioningOptions = new()
		{
			FullVersionFormat = "{Major}.{Minor}.{Patch}{PreRelease}{BuildMetadata}",
			FileVersionFormat = "{Major}.{Minor}.{Patch}.0",
			AssemblyVersionFormat = "{Major}.0.0.0",
			PreReleaseFormat = "dev.{CommitHeight}",
			BuildMetadataFormat = "{CommitHash}"
		};

		private static IEnumerable<object[]> GetBuildVersionTestData()
		{
			yield return new object[]
			{
				"Null values",
				null,
				null,
				null
			};
			yield return new object[]
			{
				"No formats specified",
				new VersionDetails { },
				new BuildVersioningOptions { },
				new BuildVersion { }
			};
			yield return new object[]
			{
				"No Git tag",
				new VersionDetails { CommitHash = "abcdef" },
				DefaultBuildVersioningOptions,
				new BuildVersion { FullVersion = "0.0.0-dev.0+abcdef", FileVersion = "0.0.0.0", AssemblyVersion = "0.0.0.0" }
			};
			yield return new object[]
			{
				"Tagged release",
				new VersionDetails { MajorVersion = 1, MinorVersion = 2, PatchVersion = 4, CommitHash = "abcdef", IsTaggedRelease = true },
				DefaultBuildVersioningOptions,
				new BuildVersion { FullVersion = "1.2.4+abcdef", FileVersion = "1.2.4.0", AssemblyVersion = "1.0.0.0" }
			};
			yield return new object[]
			{
				"Has commit height",
				new VersionDetails { MajorVersion = 1, MinorVersion = 2, PatchVersion = 4, CommitHeight = 1, CommitHash = "abcdef" },
				DefaultBuildVersioningOptions,
				new BuildVersion { FullVersion = "1.2.4-dev.1+abcdef", FileVersion = "1.2.4.0", AssemblyVersion = "1.0.0.0" }
			};
			yield return new object[]
			{
				"Pre-release",
				new VersionDetails { MajorVersion = 1, MinorVersion = 2, PatchVersion = 4, PreRelease = "alpha", CommitHeight = 4, CommitHash = "abcdef" },
				DefaultBuildVersioningOptions,
				new BuildVersion { FullVersion = "1.2.4-alpha+abcdef", FileVersion = "1.2.4.0", AssemblyVersion = "1.0.0.0" }
			};
			yield return new object[]
			{
				"Build metadata is overridden when format is defined",
				new VersionDetails { BuildMetadata = "custom.{CommitHash}", CommitHash = "abcdef" },
				DefaultBuildVersioningOptions,
				new BuildVersion { FullVersion = "0.0.0-dev.0+abcdef", FileVersion = "0.0.0.0", AssemblyVersion = "0.0.0.0" }
			};
			yield return new object[]
			{
				"Auto-increment tags",
				new VersionDetails { MajorVersion = 1, MinorVersion = 2, PatchVersion = 3 },
				new BuildVersioningOptions { FullVersionFormat = "{Major++}.{Minor++}.{Patch++}", FileVersionFormat = "{Major++}.{Minor++}.{Patch++}", AssemblyVersionFormat = "{Major++}.{Minor++}.{Patch++}" },
				new BuildVersion { FullVersion = "2.3.4", FileVersion = "2.3.4", AssemblyVersion = "2.3.4" }
			};
			yield return new object[]
			{
				"Don't auto-increment tags on tagged release",
				new VersionDetails { MajorVersion = 1, MinorVersion = 2, PatchVersion = 3, IsTaggedRelease = true },
				new BuildVersioningOptions { FullVersionFormat = "{Major++}.{Minor++}.{Patch++}", FileVersionFormat = "{Major++}.{Minor++}.{Patch++}", AssemblyVersionFormat = "{Major++}.{Minor++}.{Patch++}" },
				new BuildVersion { FullVersion = "1.2.3", FileVersion = "1.2.3", AssemblyVersion = "1.2.3" }
			};
			yield return new object[]
			{
				"Commit height available for pre-release",
				new VersionDetails { CommitHeight = 1, CommitHash = "abcdef" },
				new BuildVersioningOptions { FullVersionFormat = "{Major}.{Minor}.{Patch}{PreRelease}", PreReleaseFormat = "commitHeight.{CommitHeight}" },
				new BuildVersion { FullVersion = "0.0.0-commitHeight.1" }
			};
			yield return new object[]
			{
				"Commit height and commit hash available for build metadata",
				new VersionDetails { CommitHeight = 1, CommitHash = "abcdef" },
				new BuildVersioningOptions { FullVersionFormat = "{Major}.{Minor}.{Patch}{BuildMetadata}", BuildMetadataFormat = "commitHeight.{CommitHeight}-commitHash.{CommitHash}" },
				new BuildVersion { FullVersion = "0.0.0+commitHeight.1-commitHash.abcdef" }
			};
		}

		public static string GetBuildVersionTestName(MethodInfo methodInfo, object[] data) => data[0] as string;

		[DataTestMethod]
		[DynamicData(nameof(GetBuildVersionTestData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetBuildVersionTestName))]
		public void GetBuildVersion(string testName, VersionDetails inputVersion, BuildVersioningOptions options, BuildVersion expected)
		{
			var versionDetailsProviderMock = new Mock<IVersionDetailsProvider>();
			versionDetailsProviderMock.Setup(c => c.GetVersionDetails()).Returns(inputVersion);
			var buildVersioner = new BuildVersioner(versionDetailsProviderMock.Object);

			var result = buildVersioner.GetBuildVersion(options);
			Assert.AreEqual(expected, result);
		}
	}
}

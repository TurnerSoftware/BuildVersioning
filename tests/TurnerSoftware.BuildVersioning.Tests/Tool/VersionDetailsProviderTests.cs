using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TurnerSoftware.BuildVersioning.Tool;

namespace TurnerSoftware.BuildVersioning.Tests.Tool;

[TestClass]
public class VersionDetailsProviderTests
{
	private static IEnumerable<object[]> GetVersionDetailsTestData()
	{
		yield return new object[]
		{
			null,
			null
		};
		yield return new object[]
		{
			"abcdef",
			new VersionDetails { CommitHash = "abcdef" }
		};
		yield return new object[]
		{
			"1.2.4-0-abcdef",
			new VersionDetails { MajorVersion = 1, MinorVersion = 2, PatchVersion = 4, CommitHash = "abcdef", IsTaggedRelease = true }
		};
		yield return new object[]
		{
			"1.2.4-1-abcdef",
			new VersionDetails { MajorVersion = 1, MinorVersion = 2, PatchVersion = 4, CommitHeight = 1, CommitHash = "abcdef" }
		};
		yield return new object[]
		{
			"1.2.4-alpha-4-abcdef",
			new VersionDetails { MajorVersion = 1, MinorVersion = 2, PatchVersion = 4, PreRelease = "alpha", CommitHeight = 4, CommitHash = "abcdef" }
		};
		yield return new object[]
		{
			"1.2.4-alpha+build.123-4-abcdef",
			new VersionDetails { MajorVersion = 1, MinorVersion = 2, PatchVersion = 4, PreRelease = "alpha", BuildMetadata = "build.123", CommitHeight = 4, CommitHash = "abcdef" }
		};
		yield return new object[]
		{
			"v1.2.4-alpha+build.123-4-abcdef",
			new VersionDetails { MajorVersion = 1, MinorVersion = 2, PatchVersion = 4, PreRelease = "alpha", BuildMetadata = "build.123", CommitHeight = 4, CommitHash = "abcdef" }
		};
	}

	public static string GetVersionDetailsTestName(MethodInfo methodInfo, object[] data) => data[0] as string ?? "Null";

	[DataTestMethod]
	[DynamicData(nameof(GetVersionDetailsTestData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetVersionDetailsTestName))]
	public void GetVersionDetails(string gitDescribeString, VersionDetails expected)
	{
		var commandRunnerMock = new Mock<IGitCommandRunner>();
		commandRunnerMock.Setup(c => c.GitDescribe()).Returns(gitDescribeString);
		var versionDetailsProvider = new VersionDetailsProvider(commandRunnerMock.Object);

		var result = versionDetailsProvider.GetVersionDetails();
		Assert.AreEqual(expected, result);
	}
}

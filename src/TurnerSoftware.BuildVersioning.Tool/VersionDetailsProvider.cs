using System.Text.RegularExpressions;

namespace TurnerSoftware.BuildVersioning.Tool;

internal class VersionDetailsProvider(IGitCommandRunner gitDataProvider) : IVersionDetailsProvider
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

	public VersionDetails GetVersionDetails()
	{
		var gitDetails = gitDataProvider.GitDescribe();
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
				PreRelease = matchedGroups["preRelease"].Success ? matchedGroups["preRelease"].Value : default,
				BuildMetadata = matchedGroups["buildMetadata"].Success ? matchedGroups["buildMetadata"].Value : default,
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
}

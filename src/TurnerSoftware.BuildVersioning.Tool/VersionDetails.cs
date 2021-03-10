namespace TurnerSoftware.BuildVersioning.Tool
{
	public record VersionDetails
	{
		public int MajorVersion { get; init; }
		public int MinorVersion { get; init; }
		public int PatchVersion { get; init; }
		public string PreRelease { get; init; }
		public string BuildMetadata { get; init; }
		public string CommitHash { get; init; }
		public int CommitHeight { get; init; }
		public bool IsTaggedRelease { get; init; }
	}
}

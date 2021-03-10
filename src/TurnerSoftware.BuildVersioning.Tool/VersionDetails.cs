namespace TurnerSoftware.BuildVersioning.Tool
{
	class VersionDetails
	{
		public int MajorVersion { get; set; }
		public int MinorVersion { get; set; }
		public int PatchVersion { get; set; }
		public string PreRelease { get; set; }
		public string BuildMetadata { get; set; }
		public string CommitHash { get; set; }
		public int CommitHeight { get; set; }
		public bool IsTaggedRelease { get; set; }
	}
}

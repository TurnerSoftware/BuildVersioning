namespace TurnerSoftware.BuildVersioning.Tool
{
	public record BuildVersioningOptions
	{
		public string FullVersionFormat { get; init; }
		public string FileVersionFormat { get; init; }
		public string AssemblyVersionFormat { get; init; }
		public string PreReleaseFormat { get; init; }
		public string BuildMetadataFormat { get; init; }
	}
}

namespace TurnerSoftware.BuildVersioning.Tool
{
	public record BuildVersion
	{
		public string FullVersion { get; init; }
		public string FileVersion { get; init; }
		public string AssemblyVersion { get; init; }
	}
}

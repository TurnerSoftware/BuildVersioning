namespace TurnerSoftware.BuildVersioning.Tool
{
	public interface IGitCommandRunner
	{
		/// <summary>
		/// Returns a result from `git describe` containing the tag name, number of commits from the tag (commit height) and a 7-character commit hash.
		/// </summary>
		/// <remarks>
		/// Format with tag: {tag}-{commitHeight}-{commitHash}<br />
		/// Format without tag: {commitHash}
		/// </remarks>
		string GitDescribe();
	}
}

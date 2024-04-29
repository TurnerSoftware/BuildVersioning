using Microsoft.VisualStudio.TestTools.UnitTesting;
using TurnerSoftware.BuildVersioning.Tool;

namespace TurnerSoftware.BuildVersioning.Tests.Tool;

[TestClass]
public class GitCommandRunnerTests
{
	[TestMethod]
	public void GitDescribe()
	{
		var gitCommandRunner = new GitCommandRunner();

		var result = gitCommandRunner.GitDescribe();

		Assert.IsFalse(result.StartsWith("fatal"));
	}
}

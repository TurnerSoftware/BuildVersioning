using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TurnerSoftware.BuildVersioning.Tool
{
	internal class GitCommandRunner : IGitCommandRunner
	{
		private string RunCommand(string command)
		{
			using (var process = new Process())
			{
				process.StartInfo = new ProcessStartInfo("git", command)
				{
					RedirectStandardOutput = true
				};

				var waitOnExit = new TaskCompletionSource<object>();
				process.Exited += (s, e) => waitOnExit.SetResult(default);
				process.EnableRaisingEvents = true;

				try
				{
					process.Start();
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine(ex.Message);
					return null;
				}

				var standardOutputTask = process.StandardOutput.ReadToEndAsync();

				Task.WaitAll(waitOnExit.Task, standardOutputTask);

				if (process.ExitCode != 0)
				{
					return null;
				}

				return standardOutputTask.Result;
			}
		}

		public string GitDescribe() => RunCommand("describe --tags --abbrev=7 --always --long");
	}
}

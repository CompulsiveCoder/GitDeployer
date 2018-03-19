using System;
using System.Threading;
using System.Text;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime;
using gitter;

namespace GitDeployer
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var sleepTime = Convert.ToInt32(args[0]);

			var repositoryPath = args[1];

			var path = Path.GetFullPath (args[2]);
			if (!Directory.Exists(path))
				Directory.CreateDirectory (path);

			Environment.CurrentDirectory = path;

			var updateScriptName = args [3];

			var gitter = new Gitter ();

			var isRunning = true;
			var needsUpdate = false;

			while (isRunning) {
				Console.WriteLine ("Checking remote repository");

				if (gitter.IsInitialized (path)) {
					var repository = gitter.Open (path);
					var changesFound = repository.Pull ("origin");
					needsUpdate = changesFound;
				}
				else {
					gitter.Clone (repositoryPath, path);
					needsUpdate = true;
				}

				if (needsUpdate)
					Update (updateScriptName);
				else
					Console.WriteLine ("Already up to date. Update skipped.");

				Thread.Sleep (sleepTime * 1000);
			}
		}

		public static void Update(string updateScriptName)
		{
			Console.WriteLine ("Triggering update");
			var starter = new ProcessStarter ();
			starter.Start ("sh", updateScriptName);

			Console.WriteLine (starter.Output);
		}

	}
}

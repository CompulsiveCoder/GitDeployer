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
			Console.WriteLine ("Starting GitDeployer");
			Console.WriteLine ("");

			var sleepTime = Convert.ToInt32(args[0]);

			Console.WriteLine ("Sleep time: " + sleepTime + " seconds");

			var repositoryPath = args[1];

			Console.WriteLine ("Repository path: " + repositoryPath);

			var path = Path.GetFullPath (args[2]);
			if (!Directory.Exists(path))
				Directory.CreateDirectory (path);

			Environment.CurrentDirectory = path;

			var updateScriptName = args [3];

			Console.WriteLine ("Update script: " + updateScriptName);
			Console.WriteLine ("");

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
			Console.WriteLine ("Dir: " + Environment.CurrentDirectory);
			var starter = new ProcessStarter ();
			starter.Start ("sh " + updateScriptName);

			Console.WriteLine (starter.Output);

			var notInSync = starter.Output.Contains ("not in sync");
			var couldNotOpenPort = starter.Output.Contains ("could not open port");

			if (notInSync) {
				Console.WriteLine ("'not in sync' error means the arduino has likely crashed. Disconnect and reconnect USB or reboot.");
			}

			if (couldNotOpenPort) {
				Console.WriteLine ("'could not open port' error means the arduino isnt connected or isnt detected. Ensure it's connected.");
			}

			if (notInSync || couldNotOpenPort) {
				Console.WriteLine ("");
				Console.WriteLine ("Upload failed. Retrying in 10 seconds.");
				Console.WriteLine ("");

				Thread.Sleep (10 * 1000);

				Update (updateScriptName);
			}
		}

	}
}

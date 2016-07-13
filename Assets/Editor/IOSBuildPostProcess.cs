using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

public class IOSBuildPostProcess : MonoBehaviour
{
	[PostProcessBuild]
	public static void OnPostprocessBuild (BuildTarget target, string pathToBuildProject)
	{
		if (target == BuildTarget.iOS) {
			UnityEngine.Debug.Log ("[SPIL] Starting custom post process build script." + pathToBuildProject);

			UnityEngine.Debug.Log ("[SPIL] Moving Spil.framework to the root of the project");
			MoveDirectory (pathToBuildProject + "/Frameworks/Plugins/iOS/Spil.framework", pathToBuildProject + "/Spil.framework");

			UnityEngine.Debug.Log ("[SPIL] Executing: python " + pathToBuildProject + "/Spil.framework/setup.py Unity-iPhone");
			Process setupProcess = new Process ();
			setupProcess.StartInfo.WorkingDirectory = pathToBuildProject;
			setupProcess.StartInfo.FileName = "python";
			setupProcess.StartInfo.Arguments = "Spil.framework/setup.py Unity-iPhone";
			setupProcess.StartInfo.UseShellExecute = false;
			setupProcess.StartInfo.RedirectStandardOutput = true;
			setupProcess.Start (); 
			setupProcess.WaitForExit ();

			UnityEngine.Debug.Log ("[SPIL] --> Setup.py output: " + setupProcess.StandardOutput.ReadToEnd ());

			UnityEngine.Debug.Log ("[SPIL] Custom post process build script finished executing!");
		}
	}

	public static void MoveDirectory (string source, string target)
	{
		var stack = new Stack<Folders> ();
		stack.Push (new Folders (source, target));

		while (stack.Count > 0) {
			var folders = stack.Pop ();
			Directory.CreateDirectory (folders.Target);
			foreach (var file in Directory.GetFiles(folders.Source, "*.*")) {
				string targetFile = Path.Combine (folders.Target, Path.GetFileName (file));
				if (File.Exists (targetFile))
					File.Delete (targetFile);
				File.Move (file, targetFile);
			}

			foreach (var folder in Directory.GetDirectories(folders.Source)) {
				stack.Push (new Folders (folder, Path.Combine (folders.Target, Path.GetFileName (folder))));
			}
		}
		Directory.Delete (source, true);
	}

	public class Folders
	{
		public string Source { get; private set; }

		public string Target { get; private set; }

		public Folders (string source, string target)
		{
			Source = source;
			Target = target;
		}
	}
}
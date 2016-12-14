using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using SpilGames.Unity.Utils;
using SpilGames.Unity.Implementations;

public class SpilIOSBuildPostProcess : MonoBehaviour
{
	[PostProcessBuild]
	public static void OnPostprocessBuild (BuildTarget target, string pathToBuildProject)
	{
		if (target == BuildTarget.iOS) {
			UnityEngine.Debug.Log ("[SPIL] Starting custom post process build script." + pathToBuildProject);

			try {
				CheckLatestPluginVersion();
				CheckSlotGameConifg();
			} catch (Exception) {}

			UnityEngine.Debug.Log ("[SPIL] Moving Spil.framework to the root of the project");
			MoveDirectory (pathToBuildProject + "/Frameworks/Plugins/iOS/Spil.framework", pathToBuildProject + "/Spil.framework");

			bool exportDefaultEntitlements = EditorPrefs.GetBool ("exportDefaultEntitlements");
			bool useICloudContainer = EditorPrefs.GetBool ("useICloudContainer");
			bool useICloudKV = EditorPrefs.GetBool ("useICloudKV");
			bool usePushNotifications = EditorPrefs.GetBool ("usePushNotifications");

			String arguments = "Unity-iPhone " +
								   exportDefaultEntitlements + " " +
								   useICloudContainer + " " +
				                   useICloudKV + " " +
				                   usePushNotifications;

			UnityEngine.Debug.Log ("[SPIL] Executing: python " + pathToBuildProject + "/Spil.framework/setup.py " + arguments);
			Process setupProcess = new Process ();
			setupProcess.StartInfo.WorkingDirectory = pathToBuildProject;
			setupProcess.StartInfo.FileName = "python";
			setupProcess.StartInfo.Arguments = "Spil.framework/setup.py " + arguments;
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

	public static void CheckLatestPluginVersion(){
		WWW request = new WWW("https://api.github.com/repos/spilgames/spil_event_unity_plugin/releases/latest");

		while (!request.isDone);

		if(request.error == null){
			JSONObject response = new JSONObject(request.text);

			string GitHubReleaseTag = response.GetField("tag_name").Print(false);

			if(!GitHubReleaseTag.Contains(SpilUnityImplementationBase.PluginVersion)){
				UnityEngine.Debug.Log("A new version of the Spil SDK is available! You can download the new version here: https://github.com/spilgames/spil_event_unity_plugin/releases ");
			} 
		}
	}

	public static void CheckSlotGameConifg(){
		JSONObject slotConfigJSON = new JSONObject(GetData("requestConfig"));
		JSONObject localConfigJSON = new JSONObject(System.IO.File.ReadAllText (Application.streamingAssetsPath + "/defaultGameConfig.json"));
		if(slotConfigJSON.HasField("iosSdkConfig") && localConfigJSON.HasField("iosSdkConfig")){
			string slotConfig = slotConfigJSON.GetField("iosSdkConfig").Print(false);
			string localConfig = localConfigJSON.GetField("iosSdkConfig").Print(false);

			if(!localConfig.Equals(slotConfig)){
				UnityEngine.Debug.Log("The local \"defaultGameConfig.json\" file is out-of-sync with the SLOT configuration. Please make sure to update your file with the latest changes from SLOT before building!");
			}
		} 
	}

	public static string GetData (string type) {
		string data = "";
		WWWForm form = GetFormData ();
		form.AddField ("name", type);
		WWW request = new WWW ("https://apptracker.spilgames.com/android_event", form);
		while (!request.isDone)
			;
		if (request.error != null) {
			UnityEngine.Debug.LogError ("Error getting game data: " + request.error);  
		} else { 
			UnityEngine.Debug.Log (type + " Data returned: " + request.text);
			JSONObject serverResponce = new JSONObject (request.text);
			data = serverResponce.GetField ("data").ToString ();
		}
		return data;
	}

	static WWWForm GetFormData () {
		JSONObject dummyData = new JSONObject ();
		dummyData.AddField ("uid", "deadbeef");
		dummyData.AddField ("locale", "en");
		dummyData.AddField ("appVersion", "1");
		dummyData.AddField ("apiVersion", "1");
		dummyData.AddField ("os", "Android");
		dummyData.AddField ("osVersion", "1");
		dummyData.AddField ("deviceModel", "Backend");
		dummyData.AddField ("packageName", PlayerSettings.bundleIdentifier);
		dummyData.AddField ("tto", "0");
		dummyData.AddField ("sessionId", "deadbeef");
		dummyData.AddField ("timezoneOffset", "0");
		JSONObject dummyCustomData = new JSONObject ();
		JSONObject dummyWallet = new JSONObject ();
		dummyWallet.AddField ("offset", 0);
		JSONObject dummyInventory = new JSONObject ();
		dummyInventory.AddField ("offset", 0);
		dummyCustomData.AddField ("wallet", dummyWallet);
		dummyCustomData.AddField ("inventory", dummyInventory);
		WWWForm form = new WWWForm ();
		form.AddField ("data", dummyData.ToString ());
		form.AddField ("customData", dummyCustomData.ToString ());
		form.AddField ("ts", "1470057439857");
		form.AddField ("queued", "0");
		return form;
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
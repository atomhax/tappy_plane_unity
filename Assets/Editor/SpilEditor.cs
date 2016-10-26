using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using SpilGames.Unity.Utils;
using SpilGames.Unity;

public class SpilEditor : EditorWindow {
	private int tabSelected = 0;

	[MenuItem ("Spilgames/Config", false, 1)]
	static void Init () {
		SpilEditor window = (SpilEditor)EditorWindow.GetWindow (typeof(SpilEditor));
		window.autoRepaintOnSceneChange = true;
		window.titleContent.text = "Spilgames";
		window.Show ();
	}

	void OnGUI () {
		GUILayout.BeginVertical(); {
			GUILayout.BeginHorizontal ();
			{
				if (GUILayout.Toggle (tabSelected == 0, "General", EditorStyles.toolbarButton)) {
					tabSelected = 0;
				}
				if (GUILayout.Toggle (tabSelected == 1, "iOS", EditorStyles.toolbarButton)) {
					tabSelected = 1;
				}
				if (GUILayout.Toggle (tabSelected == 2, "Android", EditorStyles.toolbarButton)) {
					tabSelected = 2;
				}
			}
			GUILayout.EndHorizontal ();

			switch (tabSelected) {
			case 0:
				DrawGeneral ();
				break;
			case 1:
				DrawIOS ();
				break;
			case 2:
				DrawAndroid ();
				break;
			}
		}
		GUILayout.EndVertical ();
	}

	private void DrawGeneral () {
		GUILayout.Label ("SpilSDK Config Import", EditorStyles.boldLabel);
		GUILayout.Label ("WARNING: Please make sure to set your bundle ID in your player settings before getting your game data.\nPlease also note that this tool will only work if your build target is set to iOS or Android");
		if (GUILayout.Button ("Create Default Configs")) {
			CreateDefaultConfigFiles ();
		}
	}

	private void DrawIOS () {
		GUILayout.Label ("SpilSDK iOS Config", EditorStyles.boldLabel);

		string customUserId = "<< Spil component not found in scene! >>";
		bool exportDefaultEntitlements = EditorPrefs.GetBool ("exportDefaultEntitlements");
		bool useICloudContainer = EditorPrefs.GetBool ("useICloudContainer");
		bool useICloudKV = EditorPrefs.GetBool ("useICloudKV");
		bool usePushNotifications = EditorPrefs.GetBool ("usePushNotifications");

		Spil spil = GameObject.FindObjectOfType<Spil> ();
		if (spil != null) {
			customUserId = spil.CustomBundleId;
		}
		GUILayout.Label ("Custom bundle id: (Useful when the bundle id used to build is\n" +
			"not the same as the one connecting to the Spil Games backend)");
		customUserId = GUILayout.TextField (customUserId);

		GUILayout.Space (4);
		GUILayout.Label ("Entitlements:", EditorStyles.boldLabel);
		exportDefaultEntitlements = GUILayout.Toggle (exportDefaultEntitlements, "Export Spil Games entitlements");
		useICloudContainer = GUILayout.Toggle (useICloudContainer, "Use iCloud project container");
		useICloudKV = GUILayout.Toggle (useICloudKV, "Use iCloud key-value store");
		usePushNotifications = GUILayout.Toggle (usePushNotifications, "Use push notifications");

		EditorPrefs.SetBool ("exportDefaultEntitlements", exportDefaultEntitlements);
		EditorPrefs.SetBool ("useICloudContainer", useICloudContainer);
		EditorPrefs.SetBool ("useICloudKV", useICloudKV);
		EditorPrefs.SetBool ("usePushNotifications", usePushNotifications);

		if (spil != null) {
			spil.CustomBundleId = customUserId;
		}
	}

	private void DrawAndroid () {
		GUILayout.Label ("SpilSDK Android Config", EditorStyles.boldLabel);
	}

	void CreateDefaultConfigFiles () {
		if (PlayerSettings.bundleIdentifier == "") {
			throw new UnityException ("Bundle ID is Blank");
		}
		Debug.Log ("Getting Default Game Data for: " + PlayerSettings.bundleIdentifier);
		string streamingAssetsPath = Application.dataPath + "/StreamingAssets";
		if (!File.Exists (streamingAssetsPath)) {
			Directory.CreateDirectory (streamingAssetsPath);
		}
		File.WriteAllText (streamingAssetsPath + "/defaultGamedata.json", GetData ("requestGameData"));
		File.WriteAllText (streamingAssetsPath + "/defaultGameConfig.json", GetData ("requestConfig"));
		File.WriteAllText (streamingAssetsPath + "/defaultPlayerData.json", GetData ("requestPlayerData"));
	}

	string GetData (string type) {
		string gameData = "";
		WWWForm form = GetFormData ();
		form.AddField ("name", type);
		WWW request = new WWW ("https://apptracker.spilgames.com/android_event", form);
		while (!request.isDone)
			;
		if (request.error != null) {
			Debug.LogError ("Error getting game data: " + request.error);  
		} else { 
			Debug.Log (type + " Data returned: " + request.text);
			JSONObject serverResponce = new JSONObject (request.text);
			gameData = serverResponce.GetField ("data").ToString ();
		}
		return gameData;
	}

	WWWForm GetFormData () {
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
}
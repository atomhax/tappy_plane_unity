using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using SpilGames.Unity.Utils;
public class SpilEditor : EditorWindow 
{

	[MenuItem ("Window/Spilgames", false, 1)]
	static void Init () {
		// Get existing open window or if none, make a new one:
		SpilEditor window = (SpilEditor)EditorWindow.GetWindow( typeof( SpilEditor ) );
		window.autoRepaintOnSceneChange = true;
		window.titleContent.text = "Spilgames";
		window.Show();
	}


	void OnGUI()
	{
		GUILayout.Label( "SpilSDK Config Import", EditorStyles.boldLabel );
		GUILayout.Label ("WARNING: Please make sure to set your bundle ID in your player settings before getting your game data.\nPlease also note that this tool will only work if your build target is set to iOS or Android");
		if (GUILayout.Button ("Create Default Configs")) {
			CreateDefaultConfigFiles ();
		}
	}


	void CreateDefaultConfigFiles(){
		if(PlayerSettings.bundleIdentifier == ""){
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

	string GetData(string type){
		string gameData = "";
		WWWForm form = GetFormData ();
		form.AddField ("name", type);
		WWW request = new WWW ("https://apptracker.spilgames.com/android_event", form);
		while(!request.isDone);
		if (request.error != null) {
			Debug.LogError ("Error getting game data: " + request.error);  
		} else { 
			Debug.Log (type + " Data returned: " + request.text);
			JSONObject serverResponce = new JSONObject (request.text);
			gameData = serverResponce.GetField ("data").ToString ();
		}
		return gameData;
	}

	WWWForm GetFormData(){
		JSONObject dummyData = new JSONObject ();
		dummyData.AddField ("uid","deadbeef");
		dummyData.AddField ("locale","en");
		dummyData.AddField ("appVersion","1");
		dummyData.AddField ("apiVersion","1");
		dummyData.AddField ("os","Android");
		dummyData.AddField ("osVersion","1");
		dummyData.AddField ("deviceModel","Backend");
		dummyData.AddField ("packageName",PlayerSettings.bundleIdentifier);
		dummyData.AddField ("tto","0");
		dummyData.AddField ("sessionId","deadbeef");
		dummyData.AddField ("timezoneOffset","0");
		JSONObject dummyCustomData = new JSONObject ();
		JSONObject dummyWallet = new JSONObject ();
		dummyWallet.AddField ("offset", 0);
		JSONObject dummyInventory = new JSONObject ();
		dummyInventory.AddField ("offset", 0);
		dummyCustomData.AddField ("wallet",dummyWallet);
		dummyCustomData.AddField ("inventory",dummyInventory);
		WWWForm form = new WWWForm ();
		form.AddField ("data",dummyData.ToString());
		form.AddField ("customData",dummyCustomData.ToString());
		form.AddField ("ts", "1470057439857");
		form.AddField ("queued", "0");
		return form;
	}

}
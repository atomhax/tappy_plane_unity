using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Json;

public class SpilEditorReleases : EditorWindow {

	private static string releaseNotes;
	Vector2 scrollPos;
	private Texture2D logo = null;
	private static GUIStyle centeredStyle;

	[MenuItem ("Spil SDK/Release notes", false, 3)]
	static void Init () {
		SpilEditorReleases window = (SpilEditorReleases)EditorWindow.GetWindow (typeof(SpilEditorReleases));
		window.autoRepaintOnSceneChange = true;
		window.titleContent.text = "Release notes";
		window.minSize = new Vector2(1000, 600);
		window.Show ();
	}

	void OnEnable() {
		Vector2 size = new Vector2(position.width, 256);
        
		logo = new Texture2D((int) size.x, (int) size.y, TextureFormat.RGB24,false);
		logo.LoadImage(File.ReadAllBytes(Application.dataPath + "/Resources/Spilgames/PrivacyPolicy/Images/spillogo.png"));
	}
	
	void OnGUI(){
		EditorGUILayout.BeginVertical();

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width (position.width), GUILayout.Height (position.height));

		if (centeredStyle == null) {
			centeredStyle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, wordWrap = true, fontStyle = FontStyle.Bold};
		}
        
		GUILayout.Label("");
		GUILayout.Label(logo, centeredStyle);
		
		GUILayout.Label("");

		if (GUILayout.Button("Check latest version", GUILayout.Width(position.width - 20))){
			CheckLatestPluginVersion();
		}

		GUILayout.Label("Current Spil SDK version: " + SpilUnityImplementationBase.PluginVersion,  centeredStyle);

		GUILayout.Label("");

		if (releaseNotes == null) {
			releaseNotes = GetReleaseNotes();
		}
		
		GUILayout.Label(releaseNotes, EditorStyles.wordWrappedLabel);

		EditorGUILayout.EndScrollView();

		EditorGUILayout.EndVertical();

	}

	void CheckLatestPluginVersion(){
		WWW request = new WWW("https://api.github.com/repos/spilgames/spil_event_unity_plugin/releases/latest");

		while (!request.isDone);

		if(request.error == null || request.error.Equals("")){
			JSONObject response = new JSONObject(request.text);

			string GitHubReleaseTag = response.GetField("tag_name").Print(false);

			if(!GitHubReleaseTag.Contains(SpilUnityImplementationBase.PluginVersion)){
				Debug.Log("A new version of the Spil SDK is available! You can download the new version here: https://github.com/spilgames/spil_event_unity_plugin/releases ");
			} else {
				Debug.Log("The Spil SDK is up-to-date!");
			}
		}
	}

	string GetReleaseNotes(){
		string releaseNotes = "";

		WWW request = new WWW("https://raw.githubusercontent.com/spilgames/spil_event_unity_plugin/master/CHANGELOG.md");

		while (!request.isDone);

		if(request.error == null || request.error.Equals("")){
			releaseNotes = request.text.Substring(0, 15000);
		}

		return releaseNotes;
	}

}


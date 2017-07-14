using UnityEngine;
using System.Collections;
using UnityEditor;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Json;

public class SpilEditorReleases : EditorWindow
{

	Vector2 scrollPos;

	[MenuItem ("Spil SDK/Release notes", false, 2)]
	static void Init () {
		SpilEditorReleases window = (SpilEditorReleases)EditorWindow.GetWindow (typeof(SpilEditorReleases));
		window.autoRepaintOnSceneChange = true;
		window.titleContent.text = "Release notes";
		window.Show ();
	}

	void OnGUI(){
		EditorGUILayout.BeginVertical();

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width (position.width), GUILayout.Height (position.height));

		GUILayout.Label("");

		if (GUILayout.Button("Check latest version", GUILayout.Width (400))){
			CheckLatestPluginVersion();
		}

		GUILayout.Label("Current Spil SDK version: " + SpilUnityImplementationBase.PluginVersion,  EditorStyles.boldLabel);

		GUILayout.Label("");

		string releaseNotes = GetReleaseNotes();

		GUILayout.Label(releaseNotes, EditorStyles.wordWrappedLabel);

		EditorGUILayout.EndScrollView();

		EditorGUILayout.EndVertical();

	}

	void CheckLatestPluginVersion(){
		WWW request = new WWW("https://api.github.com/repos/spilgames/spil_event_unity_plugin/releases/latest");

		while (!request.isDone);

		if(request.error != null && !request.error.Equals("")){
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

		if(request.error == null){
			releaseNotes = request.text;
		}

		return releaseNotes;
	}

}


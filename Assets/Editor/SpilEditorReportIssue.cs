using UnityEngine;
using System.Collections;
using UnityEditor;
using SpilGames.Unity;

public class SpilEditorReportIssue : EditorWindow {

	private static Spil spil;

	[MenuItem ("Spil SDK/Report a SDK Issue", false, 3)]
	static void Init () {
		Application.OpenURL("https://github.com/spilgames/spil_event_unity_plugin/issues");
	}
}


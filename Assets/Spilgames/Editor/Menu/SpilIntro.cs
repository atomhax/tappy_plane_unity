using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SpilIntro : EditorWindow {
    private Vector2 scrollPos;
    
    static SpilIntro() {
        int spilIntroductionShown = PlayerPrefs.GetInt("spilIntroduction", 0);
        if (spilIntroductionShown == 0) {
            PlayerPrefs.SetInt("spilIntroduction", 1);
            Init();
        }
    }

    [MenuItem("Spil SDK/Introduction", false, 0)]
    static void Init() {
        SpilIntro window = (SpilIntro) EditorWindow.GetWindow(typeof(SpilIntro));
        window.autoRepaintOnSceneChange = true;
        window.titleContent.text = "Introduction";
        window.Show();
    }

    void OnGUI() {
        GUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));
        
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }
}
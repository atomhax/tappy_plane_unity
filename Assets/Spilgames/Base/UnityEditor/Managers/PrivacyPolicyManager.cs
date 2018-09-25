#if UNITY_EDITOR || UNITY_WEBGL
using SpilGames.Unity.Base.Implementations;
using UnityEditor;
using UnityEngine;

namespace SpilGames.Unity.Base.UnityEditor.Managers {
    public class PrivacyPolicyManager : MonoBehaviour{
        public static GameObject PrivacyPolicy;
        private static bool settingsScreen;
        
        public static void ShowPrivacyPolicy(bool settings) {
            SpilLogging.Log("Opening Privacy Policy Screen");

            settingsScreen = settings;
            
#if UNITY_WEBGL
            PrivacyPolicy = (GameObject)Instantiate(Spil.MonoInstance.privacyPolicyPopupPrefab);
#else 
            PrivacyPolicy = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Spilgames/Editor/Prefabs/PrivacyPolicy.prefab"));
#endif
            PrivacyPolicy.SetActive(true);
        }

        public void AcceptPrivacyPolicy() {
            if (!settingsScreen) {
                if (Spil.Instance.GetPrivValue() < 0) {
                    Spil.Instance.SavePrivValue(3);
					Spil.Instance.firePrivacyPolicyStatus("true");
                } else {
					Spil.Instance.firePrivacyPolicyStatus("true");
                }
            }
            
            Destroy(PrivacyPolicy);
        }
    }
}
#endif
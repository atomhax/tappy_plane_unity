#if UNITY_EDITOR
using SpilGames.Unity.Base.Implementations;
using UnityEngine;

namespace SpilGames.Unity.Base.UnityEditor.Managers {
    public class PrivacyPolicyManager : MonoBehaviour{
        public static GameObject PrivacyPolicy;
        
        public static void ShowPrivacyPolicy() {
            SpilLogging.Log("Opening Privacy Policy Screen");

            PrivacyPolicy = (GameObject) Instantiate(Resources.Load("Spilgames/Editor/PrivacyPolicy"));
            PrivacyPolicy.SetActive(true);
        }

        public void AcceptPrivacyPolicy() {
            SpilUnityImplementationBase.firePrivacyPolicyStatus(true);

            Destroy(PrivacyPolicy);
        }
    }
}
#endif
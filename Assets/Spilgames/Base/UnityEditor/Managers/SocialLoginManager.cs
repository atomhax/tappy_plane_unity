using System;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Base.SDK;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR || UNITY_WEBGL
namespace SpilGames.Unity.Base.UnityEditor.Managers {
    public class SocialLoginManager : MonoBehaviour {

        public static GameObject SocialOverlay;
        public Text Title;
        public Text Message;
        public Text LoginText;
        public Text PlayAsGuestText;
        
        public static string title;
        public static string message;
        public static string loginText;
        public static string playAsGuestText;
        
        void Update() {
            Title.text = title;
            Message.text = message;
            LoginText.text = loginText;
            PlayAsGuestText.text = playAsGuestText;
        }
        
        public static void ProcessUserRegister(JSONObject socialLoginJSON) {
            if (socialLoginJSON == null) {
                SpilErrorMessage error = new SpilErrorMessage();
                error.id = 37;
                error.name = "SocialLoginServerError";
                error.message = "Error communicating with the server!";
				Spil.Instance.fireLoginFailed(JsonUtility.ToJson(error));

                return;
            }

            string uid = socialLoginJSON.GetField("uid").str;
            string spilToken = socialLoginJSON.GetField("spilToken").str;
            string socialProvider = socialLoginJSON.GetField("socialProvider").str;
            string socialId = socialLoginJSON.GetField("socialId").str;

            if (uid != null && spilToken != null && socialProvider != null && socialId != null) {
                SpilUnityEditorImplementation.spilToken = spilToken;

                Spil.Instance.SetUserId(socialProvider, socialId);

                SpilUnityEditorImplementation.unauthorized = false;

                JSONObject loginMessage = new JSONObject();
                loginMessage.AddField("resetData", false);
                loginMessage.AddField("socialProvider", socialProvider);
                loginMessage.AddField("socialId", socialId);
                loginMessage.AddField("isGuest", false);

				Spil.Instance.fireLoginSuccessful(loginMessage.Print());
            }
            else {
                SpilErrorMessage error = new SpilErrorMessage();
                error.id = 37;
                error.name = "SocialLoginServerError";
                error.message = "Error communicating with the server!";
				Spil.Instance.fireLoginFailed(JsonUtility.ToJson(error));
            }
        }

        public static void ProcessUserLogin(JSONObject socialLoginJSON) {
            if (socialLoginJSON == null) {
                SpilErrorMessage error = new SpilErrorMessage();
                error.id = 37;
                error.name = "SocialLoginServerError";
                error.message = "Error communicating with the server!";
				Spil.Instance.fireLoginFailed(JsonUtility.ToJson(error));

                return;
            }

            string uid = socialLoginJSON.GetField("uid").str;
            string spilToken = socialLoginJSON.GetField("spilToken").str;
            string socialProvider = socialLoginJSON.GetField("socialProvider").str;
            string socialId = socialLoginJSON.GetField("socialId").str;

            if (uid != null && spilToken != null && socialProvider != null && socialId != null) {
                bool resetData = false;

                if (!Spil.SpilUserIdEditor.Equals(uid)) {
                    resetData = true;
                    Spil.SpilUserIdEditor = uid;
                }

                SpilUnityEditorImplementation.spilToken = spilToken;

                Spil.Instance.SetUserId(socialProvider, socialId);

                SpilUnityEditorImplementation.unauthorized = false;

                JSONObject loginMessage = new JSONObject();
                loginMessage.AddField("resetData", resetData);
                loginMessage.AddField("socialProvider", socialProvider);
                loginMessage.AddField("socialId", socialId);
                loginMessage.AddField("isGuest", false);

				Spil.Instance.fireLoginSuccessful(loginMessage.Print());
            }
            else {
                SpilErrorMessage error = new SpilErrorMessage();
                error.id = 37;
                error.name = "SocialLoginServerError";
                error.message = "Error communicating with the server!";
				Spil.Instance.fireLoginFailed(JsonUtility.ToJson(error));
            }
        }

        public static void ProcessUserLoginError(JSONObject socialLoginJSON) {
            if (socialLoginJSON.HasField("authError")) {
                long authError = socialLoginJSON.GetField("authError").i;

                SpilErrorMessage error = new SpilErrorMessage();

                switch (authError) {
                    case 1:
                        error.id = 32;
                        error.name = "InvalidSpilTokenError";
                        error.message = "Spil Token is invalid! Please login again!";
                        break;
                    case 3:
                        error.id = 34;
                        error.name = "InvalidSocialTokenError";
                        error.message = "The provided social token could not be verified with the backend";
                        break;
                    case 4:
                        error.id = 35;
                        error.name = "UserAlreadyLinkedError";
                        error.message = "User already has a social provider account";
                        break;
                    case 5:
                        error.id = 36;
                        error.name = "SocialIdAlreadyLinkedError";
                        error.message = "The social id is already linked to another user!";
                        break;
                    default:
                        error.id = 37;
                        error.name = "SocialLoginServerError";
                        error.message = "Error communicating with the server!";
                        break;
                }

				Spil.Instance.fireLoginFailed(JsonUtility.ToJson(error));
            }
            else {
                SpilErrorMessage error = new SpilErrorMessage();
                error.id = 37;
                error.name = "SocialLoginServerError";
                error.message = "Error communicating with the server!";

				Spil.Instance.fireLoginFailed(JsonUtility.ToJson(error));
            }
        }

        public static void ProcessUserLogout() {
            Spil.Instance.SetUserId(null, null);
            Spil.SpilUserIdEditor = Guid.NewGuid().ToString();
            SpilUnityEditorImplementation.spilToken = null;
			Spil.Instance.fireLogoutSuccessful();
        }

        public static void ProcessUnauthorizedResponse(string errorJSONString) {
            if (SpilUnityEditorImplementation.unauthorized) {
                return;
            }

            SpilUnityEditorImplementation.unauthorized = true;

            JSONObject errorJSON = new JSONObject(errorJSONString);

            if (errorJSON.HasField("authError")) {
                long authError = errorJSON.GetField("authError").i;

                SpilErrorMessage error = new SpilErrorMessage();

                switch (authError) {
                    case 1:
                        error.id = 32;
                        error.name = "InvalidSpilTokenError";
                        error.message = "Spil Token is invalid! Please login again!";
                        break;
                    case 2:
                        error.id = 33;
                        error.name = "RequriesLoginError";
                        error.message = "Event requires user login!";
                        break;
                    default:
                        error.id = 37;
                        error.name = "SocialLoginServerError";
                        error.message = "Error communicating with the server!";
                        break;
                }

				Spil.Instance.fireAuthenticationError(JsonUtility.ToJson(error));
            }
            else {
                SpilErrorMessage error = new SpilErrorMessage();
                error.id = 37;
                error.name = "SocialLoginServerError";
                error.message = "Error communicating with the server!";

				Spil.Instance.fireAuthenticationError(JsonUtility.ToJson(error));
            }
        }

        public static void ShowUnauthorizedDialog(string selectedTitle, string selectedMessage, string selectedLoginText,
            string selectedPlayAsGuestText) {
#if UNITY_WEBGL
            SocialOverlay = (GameObject) Instantiate(Resources.Load("Assets/Spilgames/Editor/Prefabs/SocialLogin.prefab"));
#else 
            SocialOverlay = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Spilgames/Editor/Prefabs/SocialLogin.prefab"));
#endif
            SocialOverlay.SetActive(true);

            title = selectedTitle;
            message = selectedMessage;
            loginText = selectedLoginText;
            playAsGuestText = selectedPlayAsGuestText;
        }

        public void PlayAsGuest() {
            Spil.Instance.UserPlayAsGuest();
            title = null;
            message = null;
            loginText = null;
            playAsGuestText = null;
            Destroy(SocialOverlay);
        }

        public void RequestLogin() {
			Spil.Instance.fireRequestLogin();
            title = null;
            message = null;
            loginText = null;
            playAsGuestText = null;
            Destroy(SocialOverlay);
        }
    }
    
    public class SocialLoginResponse : ResponseEvent {
        public static void ProcessSocialLoginResponse(ResponseEvent responseEvent) {
            JSONObject socialLoginJSON = responseEvent.data;

            switch (responseEvent.action.Trim().ToLower()) {
                case "register":
                    SocialLoginManager.ProcessUserRegister(socialLoginJSON);
                    break;
                case "login":
                    SocialLoginManager.ProcessUserLogin(socialLoginJSON);
                    break;
                case "logout":
                    SocialLoginManager.ProcessUserLogout();
                    break;
                case "error":
                    SocialLoginManager.ProcessUserLoginError(socialLoginJSON);
                    break;
            }
        }
    }
}
#endif
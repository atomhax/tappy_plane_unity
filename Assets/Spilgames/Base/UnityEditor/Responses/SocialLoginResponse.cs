using System;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Base.SDK;
using UnityEngine;

#if UNITY_EDITOR
namespace SpilGames.Unity.Base.UnityEditor.Responses {
    public class SocialLoginResponse : ResponseEvent {
        

        public static void ProcessSocialLoginResponse(ResponseEvent responseEvent) {
            JSONObject socialLoginJSON = responseEvent.data;

            switch (responseEvent.action.Trim().ToLower()) {
                case "register":
                    ProcessUserRegister(socialLoginJSON);
                    break;
                case "login":
                    ProcessUserLogin(socialLoginJSON);
                    break;
                case "logout":
                    ProcessUserLogout();
                    break;
                case "error":
                    ProcessUserLoginError(socialLoginJSON);
                    break;
            }
        }

        private static void ProcessUserRegister(JSONObject socialLoginJSON) {
            if (socialLoginJSON == null) {
                SpilErrorMessage error = new SpilErrorMessage();
                error.id = 37;
                error.name = "SocialLoginServerError";
                error.message = "Error communicating with the server!";
                SpilUnityImplementationBase.fireLoginFailed(JsonUtility.ToJson(error));

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

                SpilUnityImplementationBase.fireLoginSuccessful(loginMessage.Print());
            } else {
                SpilErrorMessage error = new SpilErrorMessage();
                error.id = 37;
                error.name = "SocialLoginServerError";
                error.message = "Error communicating with the server!";
                SpilUnityImplementationBase.fireLoginFailed(JsonUtility.ToJson(error));
            }
        }

        private static void ProcessUserLogin(JSONObject socialLoginJSON) {
            if (socialLoginJSON == null) {
                SpilErrorMessage error = new SpilErrorMessage();
                error.id = 37;
                error.name = "SocialLoginServerError";
                error.message = "Error communicating with the server!";
                SpilUnityImplementationBase.fireLoginFailed(JsonUtility.ToJson(error));

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

                SpilUnityImplementationBase.fireLoginSuccessful(loginMessage.Print());
            } else {
                SpilErrorMessage error = new SpilErrorMessage();
                error.id = 37;
                error.name = "SocialLoginServerError";
                error.message = "Error communicating with the server!";
                SpilUnityImplementationBase.fireLoginFailed(JsonUtility.ToJson(error));
            }
        }

        private static void ProcessUserLoginError(JSONObject socialLoginJSON) {
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

                SpilUnityImplementationBase.fireLoginFailed(JsonUtility.ToJson(error));
            } else {
                SpilErrorMessage error = new SpilErrorMessage();
                error.id = 37;
                error.name = "SocialLoginServerError";
                error.message = "Error communicating with the server!";

                SpilUnityImplementationBase.fireLoginFailed(JsonUtility.ToJson(error));
            }
        }

        private static void ProcessUserLogout() {
            Spil.Instance.SetUserId(null, null);
            Spil.SpilUserIdEditor = Guid.NewGuid().ToString();
            SpilUnityEditorImplementation.spilToken = null;
            SpilUnityImplementationBase.fireLogoutSuccessful();
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

                SpilUnityImplementationBase.fireAuthenticationError(JsonUtility.ToJson(error));
            } else {
                SpilErrorMessage error = new SpilErrorMessage();
                error.id = 37;
                error.name = "SocialLoginServerError";
                error.message = "Error communicating with the server!";

                SpilUnityImplementationBase.fireAuthenticationError(JsonUtility.ToJson(error));
            }
        }

        public static void ShowUnauthorizedDialog(string title, string message, string loginText,
            string playAsGuestText) {
            GameObject overlayObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            overlayObject.GetComponent<MeshRenderer>().enabled = false;
            SocialOverlay overlay = overlayObject.AddComponent<SocialOverlay>();

            overlay.title = title;
            overlay.message = message;
            overlay.loginText = loginText;
            overlay.playAsGuestText = playAsGuestText;
        }

        public class SocialOverlay : MonoBehaviour {
            public string title;
            public string message;
            public string loginText;
            public string playAsGuestText;

            private GUIStyle textFieldGuiStyle;

            private void Start() {
                textFieldGuiStyle = CreateGuiStyleTextField();
            }

            void OnGUI() {
                GUI.Label(new Rect(10, 10, (Screen.width - 20), (Screen.height - 20) / 3), title, textFieldGuiStyle);

                GUI.Label(new Rect(10, 10 + Screen.height / 3, (Screen.width - 20), (Screen.height - 20) / 3), message, textFieldGuiStyle);

                if (GUI.Button(new Rect(10, 10 + 2 * Screen.height / 3, (Screen.width - 20) / 2, (Screen.height - 20) / 3), playAsGuestText)) {
                    Spil.Instance.UserPlayAsGuest();
                    GameObject.Destroy(this.gameObject);
                }

                if (GUI.Button(new Rect(10 + (Screen.width - 20) / 2, 10 + 2 * Screen.height / 3, (Screen.width - 20) / 2, (Screen.height - 20) / 3), loginText)) {
                    SpilUnityImplementationBase.fireRequestLogin();
                    GameObject.Destroy(this.gameObject);
                }
            }

            private GUIStyle CreateGuiStyleTextField() {
                GUIStyle textFieldGuiStyle = new GUIStyle();
                GUIStyleState guiStyleState = new GUIStyleState();
                guiStyleState.textColor = Color.white;
                Color color = new Color(0f, 0f, 0f, 0.39f);
                Texture2D blackBackground = new Texture2D((Screen.width - 20), (Screen.height - 20) / 2);

                var fillColorArray = blackBackground.GetPixels();

                for (var i = 0; i < fillColorArray.Length; ++i) {
                    fillColorArray[i] = color;
                }

                blackBackground.SetPixels(fillColorArray);

                blackBackground.Apply();

                guiStyleState.background = blackBackground;
                textFieldGuiStyle.normal = guiStyleState;
                textFieldGuiStyle.fontSize = 24;
                textFieldGuiStyle.padding = new RectOffset(10, 10, 10, 10);
                textFieldGuiStyle.alignment = TextAnchor.MiddleCenter;

                return textFieldGuiStyle;
            }
        }
    }
}
#endif
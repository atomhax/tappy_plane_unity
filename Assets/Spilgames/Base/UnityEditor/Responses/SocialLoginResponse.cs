
using System;
using UnityEngine;

#if UNITY_EDITOR
namespace SpilGames.Unity.Base.UnityEditor.Responses {
    public class SocialLoginResponse : ResponseEvent {
        public static void ProcessSocialLoginResponse(ResponseEvent responseEvent) {
            
        }

        public static void ProcessUnauthorizedResponse(String errorJSON) {
            Debug.Log(errorJSON);
        }
        
        public static void ShowUnauthorizedDialog(string title, string message, string loginText,
            string playAsGuestText) {
            
        }
    }
}
#endif
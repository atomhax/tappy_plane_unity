using System;
using System.Collections.Generic;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.GameData;
using SpilGames.Unity.Json;
using UnityEngine;

namespace SpilGames.Unity.Base.Implementations
{
#if UNITY_WEBGL
    public class SpilWebGLUnityImplementation : SpilUnityEditorImplementation
    {
        public override string GetDeviceId()
        {
            #if UNITY_EDITOR
            return @"0123-4567-89012-3456";
            #else
            return SpilWebGLJavaScriptInterface.GetDeviceIdJS();
            #endif
        }

        public override void ShowHelpCenterWebview(string url)
        {
            SpilWebGLJavaScriptInterface.OpenUrlInNewWindowWebGL(url);
        }
    }
#endif
}
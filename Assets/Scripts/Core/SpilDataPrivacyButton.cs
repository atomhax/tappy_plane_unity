#if !UNITY_2018_3_OR_NEWER
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using SpilGames.Unity.Base.Implementations;

public class SpilDataPrivacyButton : Button
{
    bool urlOpened = false;

    SpilDataPrivacyButton()
    {
        onClick.AddListener(OpenDataPrivacyUrl);
    }

    void OnFailure(string reason)
    {
        interactable = true;
        Debug.LogWarning(String.Format("Failed to get data privacy url: {0}", reason));
    }

    void OpenUrl(string url)
    {
        Debug.Log(String.Format("Received data privacy URL: {0}", url));
        interactable = true;
        urlOpened = true;

        #if UNITY_WEBGL && !UNITY_EDITOR
        SpilWebGLJavaScriptInterface.OpenUrlInNewWindowWebGL(url);
        #else
        Application.OpenURL(url);
        #endif
    }

    void OpenDataPrivacyUrl()
    {
        interactable = false;
        DataPrivacy.FetchPrivacyUrl(OpenUrl, OnFailure);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && urlOpened)
        {
            urlOpened = false;
            DataPrivacy.FetchOptOutStatus();
        }
    }
}
#endif //!UNITY_2018_3_OR_NEWER
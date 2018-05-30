using System;
using UnityEngine;
using UnityEngine.UI;

public class RestoreIAPButtonController : Button
{
    RestoreIAPButtonController()
    {
        #if !UNITY_IOS
        this.gameObject.SetActive(false);
        #endif
        onClick.AddListener(RestoreIAPs);
    }

    void RestoreIAPs()
    {
        MyIAPManager iapManager = GameObject.FindObjectOfType<MyIAPManager>();
        iapManager.RestoreIAPs();
    }
}

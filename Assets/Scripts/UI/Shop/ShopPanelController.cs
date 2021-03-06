﻿using System;
using System.Collections.Generic;
using SpilGames.Unity;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.GameData;
using SpilGames.Unity.Helpers.PlayerData;
using SpilGames.Unity.Helpers.PlayerData.Perk;
using SpilGames.Unity.Json;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanelController : MonoBehaviour
{
    public GameObject getFreeCoinsButton, rewardSucessPanel;

    public GameController gameController;

    public Text starsAmountText, diamonsAmountText, starsRewardedText, tapperAmountText, tappyChestAmountText, spilIds;
    public Image starsPerkImage, diamondsPerkImage, tappyChestPerkImage;

    public List<GameObject> shopTabs = new List<GameObject>();
    public List<GameObject> tabButtons = new List<GameObject>();

    public Transform tabButtonPanel, tabsPanel;

    public GameObject tabButtonPrefab, tabPrefab;
    private bool closeShopAfterReward;

    public GameObject privacyPolicySettingsButton;

    public MyIAPManager iapManager;

    void OnEnable()
    {

        Spil.Instance.OnAdAvailable -= OnAdAvailable;
        Spil.Instance.OnAdAvailable += OnAdAvailable;

        Spil.Instance.OnPlayerDataUpdated -= OnPlayerDataUpdated;
        Spil.Instance.OnPlayerDataUpdated += OnPlayerDataUpdated;

        Spil.Instance.OnSplashScreenOpen -= OnSplashScreenOpen;
        Spil.Instance.OnSplashScreenOpen += OnSplashScreenOpen;

        Spil.Instance.OnSplashScreenClosed -= OnSplashScreenClosed;
        Spil.Instance.OnSplashScreenClosed += OnSplashScreenClosed;

        Spil.Instance.OnSplashScreenNotAvailable -= OnSplashScreenNotAvailable;
        Spil.Instance.OnSplashScreenNotAvailable += OnSplashScreenNotAvailable;

        Spil.Instance.OnSplashScreenError -= OnSplashScreenError;
        Spil.Instance.OnSplashScreenError += OnSplashScreenError;

        Spil.Instance.OnSplashScreenOpenShop -= OnSplashScreenOpenShop;
        Spil.Instance.OnSplashScreenOpenShop += OnSplashScreenOpenShop;

        Spil.Instance.OnPromotionsAvailable -= OnPromotionsAvailable;
        Spil.Instance.OnPromotionsAvailable += OnPromotionsAvailable;

        Invoke("RequestRewardVideo", 2);

        OnPlayerDataUpdated("Opened", null);

        spilIds.text = "DeviceId: " + Spil.Instance.GetDeviceId() + "\nUserId: " + Spil.Instance.GetSpilUserId();

        if (!Spil.CheckPrivacyPolicy)
        {
            privacyPolicySettingsButton.SetActive(false);
        }

        ResetShop();
        CreateShop();
    }

    private void OnPromotionsAvailable()
    {
        ResetShop();
        CreateShop();
    }

    //this method will take the Spil game data and create the shop from it
    public void CreateShop()
    {
        //First create the buttons for each shop tab/window
        for (int i = 0; i < Spil.GameData.Shop.Tabs.Count; i++)
        {
            CreateTabButton(Spil.GameData.Shop.Tabs[i], i);
            CreateTab(Spil.GameData.Shop.Tabs[i]);
        }

        if (Spil.PlayerData.InventoryHasItem(100848))
        {
            starsPerkImage.gameObject.SetActive(true);
        }

        if (Spil.PlayerData.InventoryHasItem(100847))
        {
            diamondsPerkImage.gameObject.SetActive(true);
        }

        if (Spil.PlayerData.InventoryHasItem(100849))
        {
            tappyChestPerkImage.gameObject.SetActive(true);
        }
    }

    public void ResetShop()
    {
        getFreeCoinsButton.SetActive(false);
        for (int i = 0; i < tabButtons.Count; i++)
        {
            Destroy(tabButtons[i]);
        }
        tabButtons.Clear();
        for (int i = 0; i < shopTabs.Count; i++)
        {
            Destroy(shopTabs[i]);
        }
        shopTabs.Clear();
    }

    void CreateTabButton(Tab tab, int position)
    {
        GameObject newTabButton = (GameObject)Instantiate(tabButtonPrefab);
        newTabButton.transform.SetParent(tabButtonPanel);
        bool hasActivePromotion = Spil.Instance.GetPromotions().HasActiveTabPromotion(tab);
        newTabButton.GetComponent<TabButtonController>().SetupButton(tab.Name, position, hasActivePromotion, this);
        newTabButton.SetActive(true);
        tabButtons.Add(newTabButton);
    }

    void CreateTab(Tab tab)
    {
        GameObject newTab = (GameObject)Instantiate(tabPrefab);
        newTab.transform.SetParent(tabsPanel);
        newTab.GetComponent<TabController>().SetupTab(this, tab);
        newTab.SetActive(false);
        shopTabs.Add(newTab);
    }

    void OnDisable()
    {
        gameController.UpdateSkins();
    }

    public void updatePlayerValues()
    {
        starsAmountText.text = Spil.PlayerData.GetCurrencyBalance(25).ToString();
        diamonsAmountText.text = Spil.PlayerData.GetCurrencyBalance(28).ToString();
        if (Spil.PlayerData.GetItemAmount(100077) < 0)
        {
            tapperAmountText.text = "0";
        }
        else
        {
            tapperAmountText.text = Spil.PlayerData.GetItemAmount(100077).ToString();
        }
        if (Spil.PlayerData.GetItemAmount(100088) < 0)
        {
            tappyChestAmountText.text = "0";
        }
        else
        {
            tappyChestAmountText.text = Spil.PlayerData.GetItemAmount(100088).ToString();
        }
    }

    void OnPlayerDataUpdated(string reason, PlayerDataUpdatedData updatedData)
    {

        starsAmountText.text = Spil.PlayerData.GetCurrencyBalance(25).ToString();
        diamonsAmountText.text = Spil.PlayerData.GetCurrencyBalance(28).ToString();

        if (Spil.PlayerData.GetItemAmount(100077) < 0)
        {
            tapperAmountText.text = "0";
        }
        else
        {
            tapperAmountText.text = Spil.PlayerData.GetItemAmount(100077).ToString();
        }
        if (Spil.PlayerData.GetItemAmount(100088) < 0)
        {
            tappyChestAmountText.text = "0";
        }
        else
        {
            tappyChestAmountText.text = Spil.PlayerData.GetItemAmount(100088).ToString();
        }
    }

    void OnAdAvailable(enumAdType adType)
    {
        if (adType == enumAdType.RewardVideo)
        {
            getFreeCoinsButton.SetActive(true);
        }
    }

    public void RequestRewardVideo()
    {
        Spil.Instance.RequestRewardVideo("Shop");
    }

    public void StartRewardedVideo()
    {
        Spil.Instance.OnAdFinished -= Spil_Instance_OnAdFinished;
        Spil.Instance.OnAdFinished += Spil_Instance_OnAdFinished;
        Spil.Instance.PlayVideo("Shop Button");
    }

    void Spil_Instance_OnAdFinished(SpilAdFinishedResponse response)
    {
        getFreeCoinsButton.SetActive(false);
        if (response.reward != null)
        {
            gameController.latestRewardAmount = 0;
            gameController.latestRewardAmount = response.reward.reward;
            gameController.latestRewardType = "stars";

            closeShopAfterReward = false;
            starsRewardedText.text = "Thanks!\nStars Rewarded : " + response.reward.reward.ToString();
            Spil.PlayerData.Wallet.Add(25, response.reward.reward, PlayerDataUpdateReasons.RewardAds, "Shop");
            rewardSucessPanel.SetActive(true);
        }
        Spil.Instance.RequestRewardVideo("Shop");
    }

    public void ShowReward(PlayerCurrencyData currency)
    {
        gameController.latestRewardAmount = currency.delta;
        gameController.latestRewardType = currency.name;

        closeShopAfterReward = true;
        starsRewardedText.text = "Rewarded: " + currency.delta + " " + currency.name + "!";
        rewardSucessPanel.SetActive(true);
    }

    public void RewardScreenClosed()
    {
        if (closeShopAfterReward)
        {
            rewardSucessPanel.SetActive(false);
            this.gameObject.SetActive(false);
            closeShopAfterReward = false;
        }
        else
        {
            rewardSucessPanel.SetActive(false);
        }
    }

    public void ShowHelpCenter()
    {
        Spil.Instance.ShowHelpCenterWebview("https://support.spilgames.com/hc/en-us");
    }

    public void ShowPrivacyPolicySettings()
    {
        Spil.Instance.ShowPrivacyPolicySettings();
    }

    public void ShowTermsAndConditions()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            SpilWebGLJavaScriptInterface.OpenUrlInNewWindowWebGL("http://www.spilgames.com/terms-of-use/");
        #else
            Application.OpenURL("http://www.spilgames.com/terms-of-use/");
        #endif       
    }
    
    public void OpenTappyChest() {
        List<PerkItem> perks = new List<PerkItem>();
        if (Spil.PlayerData.InventoryHasItem(100849)) {
            PerkItem perkItem = new PerkItem("TappyChestWeightPerk");

            Item tappyChestWeightPerk = Spil.GameData.GetItem(100849);
            long weightValue = (long) tappyChestWeightPerk.Properties["value"];
            
            PerkGachaWeight gachaWeight = new PerkGachaWeight(28, PerkGachaWeight.PerkGachaWeightType.CURRENCY, (int)weightValue);
            perkItem.gachaWeights.Add(gachaWeight);
            
            perks.Add(perkItem);
        }
        
        if (perks.Count > 0) {
            Spil.Instance.OpenGacha(100088, PlayerDataUpdateReasons.OpenGacha, "Shop", "Opening Tappy Chest", perks);
        } else {
            Spil.Instance.OpenGacha(100088, PlayerDataUpdateReasons.OpenGacha, "Shop", "Opening Tappy Chest");

        }
    }

    public void ShowTappyWheel() {
        Spil.Instance.RequestSplashScreen("tappyWheel");
    }
    
    void OnSplashScreenOpen() {
        Debug.Log("SplashScreenOpen");
    }

    void OnSplashScreenClosed() {
        Debug.Log("SplashScreenClosed");
    }

    void OnSplashScreenNotAvailable() {
        Debug.Log("SplashScreenNotAvailable");
    }

    void OnSplashScreenError(SpilErrorMessage error) {
        Debug.Log("SplashScreenError with reason: " + error.message);
    }

    void OnSplashScreenOpenShop() {
        Debug.Log("SplashScreenOpenShop");
        gameController.OpenShop();
    }
}
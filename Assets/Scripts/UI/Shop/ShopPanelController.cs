﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;
using SpilGames.Unity.Utils;


public class ShopPanelController : MonoBehaviour {

	public GameObject getFreeCoinsButton, rewardSucessPanel;

	public GameController gameController;

	public Text starsAmountText, diamonsAmountText, starsRewardedText;

	public List<GameObject> shopTabs = new List<GameObject> ();
	public List<GameObject> tabButtons = new List<GameObject> ();

	public Transform tabButtonPanel, tabsPanel;

	public GameObject tabButtonPrefab, tabPrefab;

	void OnEnable(){
		Spil.Instance.OnAdAvailable += Spil_Instance_OnAdAvailable;	
		Spil.Instance.OnPlayerDataUpdated += Spil_Instance_OnPlayerDataUpdated;
		Spil.Instance.SendRequestRewardVideoEvent ();
		Spil_Instance_OnPlayerDataUpdated("Opened", null);
		ResetShop();
		CreateShop ();
		Spil.Instance.RequestSplashScreen();
	}

	//this method will take the Spil game data and create the shop from it
	void CreateShop(){
		//First create the buttons for each shop tab/window
		for(int i = 0; i < Spil.GameData.Shop.Tabs.Count; i++ ) {
			CreateTabButton (Spil.GameData.Shop.Tabs[i], i);
			CreateTab (Spil.GameData.Shop.Tabs [i]);
		}
	}

	void ResetShop(){
		getFreeCoinsButton.SetActive (false);
		for (int i = 0; i < tabButtons.Count; i++) {
			Destroy (tabButtons [i]);
		}
		tabButtons.Clear ();
		for (int i = 0; i < shopTabs.Count; i++) {
			Destroy (shopTabs [i]);
		}
		shopTabs.Clear ();
	}

	void CreateTabButton(Tab tab,int position){
		GameObject newTabButton = (GameObject)Instantiate (tabButtonPrefab);
		newTabButton.transform.SetParent (tabButtonPanel);
		newTabButton.GetComponent<TabButtonController> ().SetupButton (tab.Name, position, this);
		tabButtons.Add (newTabButton);
	}

	void CreateTab(Tab tab){
		GameObject newTab = (GameObject)Instantiate (tabPrefab);
		newTab.transform.SetParent (tabsPanel);
		newTab.GetComponent<TabController> ().SetupTab (tab);
		newTab.SetActive (false);
		shopTabs.Add (newTab);
	}
		
	void OnDisable(){
		gameController.UpdateSkins ();
	}
		
	void Spil_Instance_OnPlayerDataUpdated (string reason, PlayerDataUpdatedData updatedData)
	{
		starsAmountText.text = Spil.PlayerData.GetCurrencyBalance (25).ToString ();
		diamonsAmountText.text = Spil.PlayerData.GetCurrencyBalance (28).ToString ();
	}
		
	void Spil_Instance_OnAdAvailable (SpilGames.Unity.Utils.enumAdType adType)
	{
		if (adType == SpilGames.Unity.Utils.enumAdType.RewardVideo) {
			getFreeCoinsButton.SetActive (true);
		}
	}

	public void StartRewardedVideo(){
		Spil.Instance.OnAdFinished += Spil_Instance_OnAdFinished;
		Spil.Instance.PlayVideo ();
	}

	void Spil_Instance_OnAdFinished (SpilGames.Unity.Utils.SpilAdFinishedResponse response)
	{
		getFreeCoinsButton.SetActive (false);
		if(response.reward != null){
			starsRewardedText.text = "Thanks!\nStars Rewarded : " + response.reward.reward.ToString ();
			Spil.PlayerData.Wallet.Add (25, response.reward.reward,PlayerDataUpdateReasons.RewardAds);
			rewardSucessPanel.SetActive (true);
		}
		Spil.Instance.SendRequestRewardVideoEvent ();
	}

	public void ShowHelpCenter(){
		Spil.Instance.ShowHelpCenter();
	}

}

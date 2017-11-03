using System.Collections.Generic;
using SpilGames.Unity;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.GameData;
using SpilGames.Unity.Helpers.PlayerData;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanelController : MonoBehaviour {

	public GameObject getFreeCoinsButton, rewardSucessPanel;

	public GameController gameController;

	public Text starsAmountText, diamonsAmountText, starsRewardedText, tapperAmountText, tappyChestAmountText, spilIds;

	public List<GameObject> shopTabs = new List<GameObject> ();
	public List<GameObject> tabButtons = new List<GameObject> ();

	public Transform tabButtonPanel, tabsPanel;

	public GameObject tabButtonPrefab, tabPrefab;
	private bool closeShopAfterReward;

	void OnEnable(){
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

		Spil.Instance.RequestRewardVideo("Shop");
		OnPlayerDataUpdated("Opened", null);

		spilIds.text = "DeviceId: " + Spil.Instance.GetDeviceId() + "\nUserId: " + Spil.Instance.GetSpilUserId();
		
		ResetShop();
		CreateShop ();
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

	public void updatePlayerValues() {
		starsAmountText.text = Spil.PlayerData.GetCurrencyBalance (25).ToString ();
		diamonsAmountText.text = Spil.PlayerData.GetCurrencyBalance (28).ToString ();
		if (Spil.PlayerData.GetItemAmount(100077) < 0) {
			tapperAmountText.text = "0";
		} else {
			tapperAmountText.text = Spil.PlayerData.GetItemAmount(100077).ToString();
		}
		if (Spil.PlayerData.GetItemAmount(100088) < 0) {
			tappyChestAmountText.text = "0";
		} else {
			tappyChestAmountText.text = Spil.PlayerData.GetItemAmount(100088).ToString();
		}
	}
	
	void OnPlayerDataUpdated (string reason, PlayerDataUpdatedData updatedData) {
		starsAmountText.text = Spil.PlayerData.GetCurrencyBalance (25).ToString ();
		diamonsAmountText.text = Spil.PlayerData.GetCurrencyBalance (28).ToString ();
		if (Spil.PlayerData.GetItemAmount(100077) < 0) {
			tapperAmountText.text = "0";
		} else {
			tapperAmountText.text = Spil.PlayerData.GetItemAmount(100077).ToString();
		}
		if (Spil.PlayerData.GetItemAmount(100088) < 0) {
			tappyChestAmountText.text = "0";
		} else {
			tappyChestAmountText.text = Spil.PlayerData.GetItemAmount(100088).ToString();
		}
	}
		
	void OnAdAvailable (enumAdType adType) {
		if (adType == enumAdType.RewardVideo) {
			getFreeCoinsButton.SetActive (true);
		}
	}

	public void StartRewardedVideo(){
		Spil.Instance.OnAdFinished -= Spil_Instance_OnAdFinished;
		Spil.Instance.OnAdFinished += Spil_Instance_OnAdFinished;
		Spil.Instance.PlayVideo ("Shop Button");
	}

	void Spil_Instance_OnAdFinished (SpilAdFinishedResponse response) {
		getFreeCoinsButton.SetActive (false);
		if(response.reward != null) {
			gameController.latestRewardAmount = 0;
			gameController.latestRewardAmount = response.reward.reward;
			gameController.latestRewardType = "stars";

			closeShopAfterReward = false;
			starsRewardedText.text = "Thanks!\nStars Rewarded : " + response.reward.reward.ToString ();
			Spil.PlayerData.Wallet.Add (25, response.reward.reward, PlayerDataUpdateReasons.RewardAds, "Shop");
			rewardSucessPanel.SetActive (true);
		}
		Spil.Instance.RequestRewardVideo("Shop");
	}

	public void ShowReward(PlayerCurrencyData currency) {
		gameController.latestRewardAmount = currency.delta;
		gameController.latestRewardType = currency.name;

		closeShopAfterReward = true;
		starsRewardedText.text = "Rewarded: " + currency.delta + " " + currency.name + "!";
		rewardSucessPanel.SetActive (true);
	}

	public void RewardScreenClosed() {
		if (closeShopAfterReward) {
			rewardSucessPanel.SetActive (false);
			this.gameObject.SetActive (false);
			closeShopAfterReward = false;
		} else {
			rewardSucessPanel.SetActive (false);
		}
	}

	public void ShowHelpCenter(){
		Spil.Instance.ShowHelpCenter();
	}

	public void OpenTappyChest() {
		Spil.PlayerData.OpenGacha(100088, PlayerDataUpdateReasons.OpenGacha, "Shop", "Opening Tappy Chest");
	}
	
	void OnSplashScreenOpen() {
		Debug.Log ("SplashScreenOpen");
	}

	void OnSplashScreenClosed() {
		Debug.Log ("SplashScreenClosed");
	}

	void OnSplashScreenNotAvailable() {
		Debug.Log ("SplashScreenNotAvailable");
	}

	void OnSplashScreenError(SpilErrorMessage error) {
		Debug.Log ("SplashScreenError with reason: " + error.message);
	}

	void OnSplashScreenOpenShop() {
		Debug.Log ("SplashScreenOpenShop");
		gameController.OpenShop ();
	}

}

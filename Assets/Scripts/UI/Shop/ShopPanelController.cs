using UnityEngine;
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
	private bool closeShopAfterReward;

	void OnEnable(){
		Spil.Instance.OnAdAvailable -= Spil_Instance_OnAdAvailable;
		Spil.Instance.OnAdAvailable += Spil_Instance_OnAdAvailable;

		Spil.Instance.OnPlayerDataUpdated -= Spil_Instance_OnPlayerDataUpdated;	
		Spil.Instance.OnPlayerDataUpdated += Spil_Instance_OnPlayerDataUpdated;

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

		Spil.Instance.SendRequestRewardVideoEvent ();
		Spil_Instance_OnPlayerDataUpdated("Opened", null);
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
			gameController.latestRewardAmount = response.reward.reward;
			gameController.latestRewardType = "stars";

			closeShopAfterReward = false;
			starsRewardedText.text = "Thanks!\nStars Rewarded : " + response.reward.reward.ToString ();
			Spil.PlayerData.Wallet.Add (25, response.reward.reward,PlayerDataUpdateReasons.RewardAds, "Shop");
			rewardSucessPanel.SetActive (true);
		}
		Spil.Instance.SendRequestRewardVideoEvent ();
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
//		Spil.Instance.ShowHelpCenter();
		Spil.Instance.ResetWallet();
	}

	void OnSplashScreenOpen()
	{
		Debug.Log ("SplashScreenOpen");
	}

	void OnSplashScreenClosed()
	{
		Debug.Log ("SplashScreenClosed");
	}

	void OnSplashScreenNotAvailable()
	{
		Debug.Log ("SplashScreenNotAvailable");
	}

	void OnSplashScreenError(SpilErrorMessage error)
	{
		Debug.Log ("SplashScreenError with reason: " + error.message);
	}

	void OnSplashScreenOpenShop()
	{
		Debug.Log ("SplashScreenOpenShop");
		gameController.OpenShop ();
	}

}

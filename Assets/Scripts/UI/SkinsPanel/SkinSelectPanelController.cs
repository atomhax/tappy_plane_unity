using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;
using UnityEngine.EventSystems;
using SpilGames.Unity.Json;
using UnityEngine.UI;

public class SkinSelectPanelController : MonoBehaviour {

	public BackgroundButtonController[] backgroundButtons;

	public PlaneSkinButtonController[] skinButtons;

	public GameController gameController;

	public MyIAPManager iapManager;
	
	public GameObject pleaseWaitPanel, purchaseSuccessPanel, purchaseFailedPanel;
	
	public Text successPanelText;

	public void OnEnable() {
		#if UNITY_TVOS
		EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
		eventSystem.firstSelectedGameObject = GameObject.Find("BackButtonSkin");
		#endif
		Spil.Instance.RequestSplashScreen ("shop");
		
		Spil.Instance.OnSplashScreenOpenShop -= OnSplashScreenOpenShop;
		Spil.Instance.OnSplashScreenOpenShop += OnSplashScreenOpenShop;
		
		Spil.Instance.OnIAPRequestPurchase -= OnIapRequestPurchase;
		Spil.Instance.OnIAPRequestPurchase += OnIapRequestPurchase;
	}

	private void OnSplashScreenOpenShop() {
		CloseSkinsPanel();
		gameController.OpenShop();
	}

	private void OnIapRequestPurchase(string skuId) {
		iapManager.BuyProductID(skuId);
	}

	public void UpdateButtons(){
		for (int i = 0; i < backgroundButtons.Length; i++) {
			backgroundButtons [i].SetupButton ();
		}
		for (int i = 0; i < skinButtons.Length; i++) {
			skinButtons [i].SetupButton ();
		}
	}

	public void CloseSkinsPanel(){
		gameController.UpdateSkins ();
		SavePrivateGameState();
		gameObject.SetActive (false);
	}

	public void SavePrivateGameState(){
		int backgroundId = PlayerPrefs.GetInt("Background",0);
		int skinId = PlayerPrefs.GetInt("Skin",0);
		float speed = PlayerPrefs.GetFloat("Speed",1f);

		PrivateGameState gameState = new PrivateGameState();
		gameState.setBackground(backgroundId);
		gameState.setSkin(skinId);
		gameState.setSpeed(speed);

		string gameStateJson = JsonHelper.getJSONFromObject(gameState);
		Spil.Instance.SetPrivateGameState(gameStateJson);
	}
	
	public void PurchaseStarted(){
		if (!isActiveAndEnabled) {
			return;
		}
		pleaseWaitPanel.SetActive (true);
	}
	
	public void PurchaseSuccess(string purchase){
		if (!isActiveAndEnabled) {
			return;
		}
		pleaseWaitPanel.SetActive (false);
		successPanelText.text = "Purchase successful\n" + purchase;
		purchaseSuccessPanel.SetActive (true);
		Invoke("UpdateButtons", 1);
	}
	
	public void PurchaseFailed(){
		if (!isActiveAndEnabled) {
			return;
		}
		pleaseWaitPanel.SetActive (false);
		purchaseFailedPanel.SetActive (true);
	}
}

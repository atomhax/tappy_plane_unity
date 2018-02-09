using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;
using UnityEngine.EventSystems;
using SpilGames.Unity.Json;

public class SkinSelectPanelController : MonoBehaviour {

	public BackgroundButtonController[] backgroundButtons;

	public PlaneSkinButtonController[] skinButtons;

	public GameController gameController;

	public PrimeIAPManager iapManager;

	public void OnEnable() {
		#if UNITY_TVOS
		EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
		eventSystem.firstSelectedGameObject = GameObject.Find("BackButtonSkin");
		#endif
		Spil.Instance.RequestSplashScreen ();
		
		Spil.Instance.OnIAPRequestPurchase -= OnIapRequestPurchase;
		Spil.Instance.OnIAPRequestPurchase += OnIapRequestPurchase;
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

		PrivateGameState gameState = new PrivateGameState();
		gameState.setBackground(backgroundId);
		gameState.setSkin(skinId);

		string gameStateJson = JsonHelper.getJSONFromObject(gameState);
		Spil.Instance.SetPrivateGameState(gameStateJson);
	}

}

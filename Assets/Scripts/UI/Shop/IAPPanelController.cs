using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;
using SpilGames.Unity.Helpers.IAPPackages;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.Promotions;


public class IAPPanelController : MonoBehaviour {

	public IAPButtonController[] iapButtons;

	public MyIAPManager iapManager;

	public GameObject pleaseWaitPanel, purchaseSuccessPanel, purchaseFailedPanel;

	public Text successPanelText;

	void Awake() {
		Spil.Instance.OnIAPValid -= OnIAPValid;
		Spil.Instance.OnIAPValid += OnIAPValid;

		Spil.Instance.OnIAPInvalid -= OnIAPInvalid;
		Spil.Instance.OnIAPInvalid += OnIAPInvalid;

		Spil.Instance.OnIAPServerError -= OnIAPServerError;
		Spil.Instance.OnIAPServerError += OnIAPServerError;
	}

	public void PurchaseStarted(){
		pleaseWaitPanel.SetActive (true);
	}
	public void PurchaseSuccess(string purchase){
		pleaseWaitPanel.SetActive (false);
		successPanelText.text = "Purchase successful\n" + purchase;
		purchaseSuccessPanel.SetActive (true);
	}
	public void PurchaseFailed(){
		pleaseWaitPanel.SetActive (false);
		purchaseFailedPanel.SetActive (true);
	}
		
	public void SetupIAPButtons(){
		for(int i = 0 ; i < iapButtons.Length; i ++){
			iapButtons [i].gameObject.SetActive (false);
		}

		PackagesHelper helper = Spil.Instance.GetPackages ();
		if(helper != null){
			if (helper.Packages.Count == 0) {
			gameObject.SetActive (false);
			return;
			} else {
				gameObject.SetActive (true);
			}
			for (int i = 0; i < helper.Packages.Count; i++) {
				Package package = helper.Packages [i];
				string promotionText = "";
				string gemAmount = package.Items [0].Value;
				string cost = iapManager.packageCosts[package.PackageId];
				if(package.HasActivePromotion()) {
					Promotion packagePromotion = Spil.Instance.GetPromotions().GetPackagePromotion(package.PackageId);
					promotionText = "PROMOTION!\n" + packagePromotion.Label + packagePromotion.ExtraEntities[0].Amount + " extra gems!";
				}
				
				iapButtons [i].PopulateIAPButton (gemAmount, promotionText, package.HasActivePromotion(), cost, package.PackageId);
				iapButtons [i].gameObject.SetActive (true);
			}
		}

	}

	public void OnIAPValid(string data){
		Debug.Log("IAP valid with data: " + data);
	}

	public void OnIAPInvalid(string message){
		Debug.Log("IAP invalid with response: " + message);
	}

	public void OnIAPServerError(SpilErrorMessage errorMessage)
	{
		Debug.Log("IAP error with message: " + errorMessage.message);
	}
		
}

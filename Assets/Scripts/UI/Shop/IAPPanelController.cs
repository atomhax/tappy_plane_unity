using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;
public class IAPPanelController : MonoBehaviour {

	public IAPButtonController[] iapButtons;

	public MyIAPManager iapManager;

	public GameObject pleaseWaitPanel, purchaseSuccessPanel, purchaseFailedPanel;

	public Text successPanelText;

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

		Debug.Log ("GETTING PACKAGES AND APPLYING");
		PackagesHelper helper = Spil.Instance.GetPackagesAndPromotions ();

		Debug.Log ("Packages found: " + helper.Packages.Count);

		for (int i = 0; i < helper.Packages.Count; i++) {
			Package package = helper.Packages [i];
			string promotionText = "";
			string gemAmount = package.Items [0].OriginalValue;
			Debug.Log ("PackageID: " + package.Id);
			string cost = iapManager.packageCosts[package.Id];
			if(package.HasActivePromotion()){
				promotionText = package.PromotionDiscountLabel;
				gemAmount = package.Items [0].PromotionValue;
			}
			iapButtons [i].PopulateIAPButton (gemAmount, promotionText, cost, package.Id);
			iapButtons [i].gameObject.SetActive (true);
		}
	}







}

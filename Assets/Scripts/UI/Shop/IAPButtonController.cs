using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IAPButtonController : MonoBehaviour {

	public Text gemAmountText, promotionText, costText;

	public GameObject saleImage;

	public string bundleID;

	public MyIAPManager iapManager;

	public void PopulateIAPButton(string gemAmount, string packageText, bool hasActivePromotion, string cost, string bundleID){
		gemAmountText.text = gemAmount;
		this.promotionText.text = packageText;
		costText.text = cost;
		this.bundleID = bundleID;

		if (hasActivePromotion) {
			saleImage.SetActive(true);
		}
	}

	public void ButtonClicked(){
		iapManager.BuyProductID (bundleID);
	}
		
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IAPButtonController : MonoBehaviour {

	public Text gemAmountText, promotionText, costText;

	public GameObject saleImage;

	public string bundleID;

	public PrimeIAPManager iapManager;

	public void PopulateIAPButton(string gemAmount, string promotionText, string cost, string bundleID){
		gemAmountText.text = gemAmount;
		this.promotionText.text = promotionText;
		costText.text = cost;
		this.bundleID = bundleID;

		if (!promotionText.Equals("")) {
			saleImage.SetActive(true);
		}
	}

	public void ButtonClicked(){
		iapManager.BuyProductID (bundleID);
	}
		
}

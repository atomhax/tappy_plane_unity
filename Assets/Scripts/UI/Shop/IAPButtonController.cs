using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IAPButtonController : MonoBehaviour {

	public Text gemAmountText, promotionText, costText;

	public string bundleID;

	public MyIAPManager iapManager;




	public void PopulateIAPButton(string gemAmount, string promotionText, string cost, string bundleID){
		gemAmountText.text = gemAmount;
		this.promotionText.text = promotionText;
		costText.text = cost;
		this.bundleID = bundleID;
	}

	public void ButtonClicked(){
		iapManager.BuyProductID (bundleID);
	}



}

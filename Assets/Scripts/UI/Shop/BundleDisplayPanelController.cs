using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;

public class BundleDisplayPanelController : MonoBehaviour {

	public Text bundleTitleText;

	public Text starsCostText, diamondCostText;

	public Text listOfItemsInBundle;

	Bundle bundleDisplayed;

	public GameObject buyButton;

	public void SetupBundleDisplayPanel(Bundle bundle){
		bundleDisplayed = bundle;
		starsCostText.text = diamondCostText.text = "0";
		bundleTitleText.text = bundle.Name;
		listOfItemsInBundle.text = "ITEMS IN THIS BUNDLE\n";
		for(int i = 0 ; i < bundle.Items.Count; i++){
			Item item = Spil.SpilGameDataInstance.GetItem (bundle.Items [i].Id);
			listOfItemsInBundle.text += item.Name + "\n";
			for(int x = 0; x < bundle.Prices.Count; x ++){
				if(bundle.Prices[x].CurrencyId == 25){
					starsCostText.text = bundle.Prices [x].Value.ToString();
				}else if(bundle.Prices[x].CurrencyId == 28){
					diamondCostText.text = bundle.Prices [x].Value.ToString();
				}
			}
		}
		gameObject.SetActive (true);
	}

	public void BuyBundle(){
		if(CanAffordBundle()){
			SpendCurrencyForBundle ();
			AddItemsToInventory ();
			buyButton.SetActive (false);
			GameObject.Find ("GameController").GetComponent<GameController> ().InGamePurchaesSuccess ();
		}
	}
		
	bool CanAffordBundle(){
		bool canAfford = true;
		for(int i = 0 ; i < bundleDisplayed.Prices.Count; i ++){
			if(Spil.SpilPlayerDataInstance.GetCurrencyBalance(bundleDisplayed.Prices[i].CurrencyId) < bundleDisplayed.Prices[i].Value){
				canAfford = false;
			}
		}
		return canAfford;
	}

	void SpendCurrencyForBundle(){
		for (int i = 0; i < bundleDisplayed.Prices.Count; i++) {
			Spil.SpilPlayerDataInstance.Wallet.Subtract (bundleDisplayed.Prices [i].CurrencyId, bundleDisplayed.Prices [i].Value, PlayerDataUpdateReasons.ItemBought);
		}
	}
		
	void AddItemsToInventory(){
		for(int i = 0; i < bundleDisplayed.Items.Count; i ++){
			Spil.SpilPlayerDataInstance.Inventory.Add (bundleDisplayed.Items [i].Id, bundleDisplayed.Items [i].Amount, PlayerDataUpdateReasons.ItemBought);
		}
	}


}

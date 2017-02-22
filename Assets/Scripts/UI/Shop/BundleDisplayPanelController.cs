using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;
using SpilGames.Unity.Utils;

public class BundleDisplayPanelController : MonoBehaviour {

	public Text bundleTitleText;

	public Text starsCostText, diamondCostText;

	public Text listOfItemsInBundle;

	Bundle bundleDisplayed;

	public GameObject buyButton;

	public Image bundleImage;

	public void SetupBundleDisplayPanel(Bundle bundle){
		bundleDisplayed = bundle;
		starsCostText.text = diamondCostText.text = "0";
		bundleTitleText.text = bundle.Name;
		listOfItemsInBundle.text = "ITEMS IN THIS BUNDLE\n";
		for(int i = 0 ; i < bundle.Items.Count; i++){
			Item item = Spil.GameData.GetItem (bundle.Items [i].Id);
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

		Spil.Instance.OnImageLoaded -= OnImageLoaded;
		Spil.Instance.OnImageLoaded += OnImageLoaded;

		Spil.Instance.OnImageLoadSuccess -= OnImageLoadSuccess;
		Spil.Instance.OnImageLoadSuccess += OnImageLoadSuccess;

		if(bundle.HasImage()){
			string imagePath = bundle.GetImagePath();

			if(imagePath != null){
				Debug.Log("Image already preloaded with path: " + imagePath);
				Spil.Instance.LoadImage(this, imagePath);
			} 
		}

	}

	public void BuyBundle(){
		if (CanAffordBundle ()) {
			SpendCurrencyForBundle ();
			AddItemsToInventory ();
			buyButton.SetActive (false);
			GameObject.Find ("GameController").GetComponent<GameController> ().InGamePurchaesSuccess (bundleDisplayed.Name);
		} else {
			GameObject.Find ("GameController").GetComponent<GameController> ().InGamePurchaesFail (bundleDisplayed);
		}
	}
		
	bool CanAffordBundle(){
		bool canAfford = true;
		for(int i = 0 ; i < bundleDisplayed.Prices.Count; i ++){
			if(Spil.PlayerData.GetCurrencyBalance(bundleDisplayed.Prices[i].CurrencyId) < bundleDisplayed.Prices[i].Value){
				canAfford = false;
			}
		}
		return canAfford;
	}

	void SpendCurrencyForBundle(){
		for (int i = 0; i < bundleDisplayed.Prices.Count; i++) {
			Spil.PlayerData.Wallet.Subtract (bundleDisplayed.Prices [i].CurrencyId, bundleDisplayed.Prices [i].Value, PlayerDataUpdateReasons.ItemBought, "Shop");
		}
	}
		
	void AddItemsToInventory(){
		for(int i = 0; i < bundleDisplayed.Items.Count; i ++){
			Spil.PlayerData.Inventory.Add (bundleDisplayed.Items [i].Id, bundleDisplayed.Items [i].Amount, PlayerDataUpdateReasons.ItemBought, "Shop");
		}
	}

	public void OnImageLoaded(Texture2D image, string localPath){
		bundleImage.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2());
	}

	public void OnImageLoadSuccess(string localPath, ImageContext imageContext){
		if(localPath != null){
			Debug.Log("Finished downloading image with local path: " + localPath);
			Spil.Instance.LoadImage(this, localPath);
		}
	}
}

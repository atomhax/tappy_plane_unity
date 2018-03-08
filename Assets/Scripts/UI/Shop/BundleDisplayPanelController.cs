using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers.GameData;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.PlayerData;

public class BundleDisplayPanelController : MonoBehaviour {
    public static BundleDisplayPanelController panelInstance;

    public Text bundleTitleText, bundleDescriptionText;

    public Text starsCostText, diamondCostText;

    public Text listOfItemsInBundle;

    Bundle bundleDisplayed;

    public GameObject buyButton;

    public Texture2D gachaImage;

    public Image bundleImage;

    public void SetupBundleDisplayPanel(Bundle bundle, Entry entry) {
        panelInstance = this;
        bundleDisplayed = bundle;
        Promotion bundlePromotion = Spil.GameData.GetPromotion(bundleDisplayed.Id);      
        starsCostText.text = diamondCostText.text = "0";

        if (bundle.DisplayName != null && !bundle.DisplayName.Equals("")) {
            bundleTitleText.text = bundle.DisplayName;
        } else {
            bundleTitleText.text = bundle.Name;
        }

        if (bundle.DisplayDescription != null && !bundle.DisplayDescription.Equals("")) {
            bundleDescriptionText.text = bundle.DisplayDescription;
        } else {
            bundleDescriptionText.text = "";
        }

        starsCostText.color = Color.white;
        diamondCostText.color = Color.white;
        
        listOfItemsInBundle.text = "Contains:";
        for (int i = 0; i < bundle.Items.Count; i++) {
            Item item = Spil.GameData.GetItem(bundle.Items[i].Id);
            listOfItemsInBundle.text += "\n" + "• " + item.Name;
            if (bundlePromotion != null) {
                for (int x = 0; x < bundlePromotion.Prices.Count; x++) {
                    if (bundlePromotion.Prices[x].CurrencyId == 25) {
                        starsCostText.text = bundlePromotion.Prices[x].Value.ToString();
                        starsCostText.color = new Color(0.76f, 0.1f, 0.11f);
                    } else if (bundlePromotion.Prices[x].CurrencyId == 28) {
                        diamondCostText.text = bundlePromotion.Prices[x].Value.ToString();
                        diamondCostText.color = new Color(0.76f, 0.1f, 0.11f);
                    }
                } 
            } else {
                for (int x = 0; x < bundle.Prices.Count; x++) {
                    if (bundle.Prices[x].CurrencyId == 25) {
                        starsCostText.text = bundle.Prices[x].Value.ToString();                       
                    } else if (bundle.Prices[x].CurrencyId == 28) {
                        diamondCostText.text = bundle.Prices[x].Value.ToString();                        
                    }
                }
            }
            
        }

        gameObject.SetActive(true);

        Spil.Instance.OnImageLoaded -= OnImageLoaded;
        Spil.Instance.OnImageLoaded += OnImageLoaded;

        Spil.Instance.OnImageLoadSuccess -= OnImageLoadSuccess;
        Spil.Instance.OnImageLoadSuccess += OnImageLoadSuccess;
        
        Spil.Instance.OnImageLoadFailed -= OnImageLoadFailed;
        Spil.Instance.OnImageLoadFailed += OnImageLoadFailed;

        if (bundlePromotion != null && bundlePromotion.ImageEntries != null && bundlePromotion.ImageEntries.Count > 0) {
            string promotionImage = bundlePromotion.ImageEntries[0].ImageUrl;
            
            if (promotionImage != null) {
                Debug.Log("Image already preloaded with path: " + promotionImage);
                Spil.Instance.LoadImage(this, promotionImage);
            }
        } else if (entry.ImageEntries != null && entry.ImageEntries.Count > 0) {
            string entryImage = entry.ImageEntries[0].ImageUrl;
            
            if (entryImage != null) {
                Debug.Log("Image already preloaded with path: " + entryImage);
                Spil.Instance.LoadImage(this, entryImage);
            }
        } else if (bundle.HasImage()) {
            string imagePath = bundle.GetImagePath();

            if (imagePath != null) {
                Debug.Log("Image already preloaded with path: " + imagePath);
                Spil.Instance.LoadImage(this, imagePath);
            }
        } else {
            if (bundleDisplayed.Id == 100100) {
                panelInstance.bundleImage.sprite = Sprite.Create(gachaImage, new Rect(0, 0, gachaImage.width, gachaImage.height), new Vector2());
                panelInstance.bundleImage.preserveAspect = true;
            } else {
                bundleImage.sprite = null;
            }
        }
    }

    private void OnImageLoadFailed(ImageContext imageContext, SpilErrorMessage errorMessage) {
        if (bundleDisplayed.Id == 100100) {
            panelInstance.bundleImage.sprite = Sprite.Create(gachaImage, new Rect(0, 0, gachaImage.width, gachaImage.height), new Vector2());
            panelInstance.bundleImage.preserveAspect = true;
        }
    }

    public void BuyBundle() {
        if (CanAffordBundle()) {
            BuyBundleFromSDK();
            if (bundleDisplayed.Id != 100100) {
                buyButton.SetActive(false);
            }

            GameObject.Find("GameController").GetComponent<GameController>().InGamePurchaesSuccess(bundleDisplayed.Name);
        } else {
            GameObject.Find("GameController").GetComponent<GameController>().InGamePurchaesFail(bundleDisplayed);
        }
    }

    bool CanAffordBundle() {
        bool canAfford = true;
        for (int i = 0; i < bundleDisplayed.Prices.Count; i++) {
            if (Spil.PlayerData.GetCurrencyBalance(bundleDisplayed.Prices[i].CurrencyId) < bundleDisplayed.Prices[i].Value) {
                canAfford = false;
            }
        }
        return canAfford;
    }

    void BuyBundleFromSDK() {
        Spil.Instance.BuyBundle(bundleDisplayed.Id, PlayerDataUpdateReasons.ItemBought, "Shop");
    }

    public void OnImageLoaded(Texture2D image, string localPath) {
        panelInstance.bundleImage.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2());
    }

    public void OnImageLoadSuccess(string localPath, ImageContext imageContext) {
        if (localPath != null) {
            Debug.Log("Finished downloading image with local path: " + localPath);
            Spil.Instance.LoadImage(this, localPath);
        }
    }
}
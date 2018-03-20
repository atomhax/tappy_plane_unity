using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers.GameData;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.PlayerData;
using SpilGames.Unity.Helpers.Promotions;

public class BundleDisplayPanelController : MonoBehaviour {
    public static BundleDisplayPanelController panelInstance;

    public Text bundleTitleText, bundleDescriptionText;

    public Text starsCostText, diamondCostText;

    public Text listOfItemsInBundle;

    Bundle bundleDisplayed;

    public GameObject buyButton;

    public Texture2D gachaImage;

    public Image bundleImage;

    public Image stickerImage;

    private static string entryImageUrl;

    void Start() {
        Spil.Instance.OnImageLoaded -= OnImageLoaded;
        Spil.Instance.OnImageLoaded += OnImageLoaded;

        Spil.Instance.OnImageLoadSuccess -= OnImageLoadSuccess;
        Spil.Instance.OnImageLoadSuccess += OnImageLoadSuccess;
        
        Spil.Instance.OnImageLoadFailed -= OnImageLoadFailed;
        Spil.Instance.OnImageLoadFailed += OnImageLoadFailed;
    }

    public void SetupBundleDisplayPanel(Bundle bundle, Entry entry) {
        panelInstance = this;
        bundleDisplayed = bundle;

        entryImageUrl = null;
        stickerImage.gameObject.SetActive(false);
        
        Promotion bundlePromotion = Spil.Instance.GetPromotions().GetBundlePromotion(bundleDisplayed.Id);      
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
                for (int x = 0; x < bundlePromotion.PriceOverride.Count; x++) {
                    if (bundlePromotion.PriceOverride[x].Id == 25) {
                        starsCostText.text = bundlePromotion.PriceOverride[x].Amount.ToString();
                        starsCostText.color = new Color(0.76f, 0.1f, 0.11f);
                    } else if (bundlePromotion.PriceOverride[x].Id == 28) {
                        diamondCostText.text = bundlePromotion.PriceOverride[x].Amount.ToString();
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

        if (bundlePromotion != null && bundlePromotion.GameAsset.Count > 0) {
            string promotionImageUrl = bundlePromotion.GameAsset[0].Value;
            
            if (promotionImageUrl != null) {
                Debug.Log("Image already preloaded with path: " + promotionImageUrl);
                Spil.Instance.LoadImage(bundleImage, promotionImageUrl);
            }
        } else if (bundle.HasImage()) {
            string bundleImageUrl = bundle.GetImagePath();

            if (bundleImageUrl != null) {
                Debug.Log("Image already preloaded with path: " + bundleImageUrl);
                Spil.Instance.LoadImage(bundleImage, bundleImageUrl);
                
                Color c = panelInstance.bundleImage.color;
                c.a = 255;
                panelInstance.bundleImage.color = c;
            }
        } else {
            if (bundleDisplayed.Id == 100100) {
                panelInstance.bundleImage.sprite = Sprite.Create(gachaImage, new Rect(0, 0, gachaImage.width, gachaImage.height), new Vector2());
                panelInstance.bundleImage.preserveAspect = true;
                Color c = panelInstance.bundleImage.color;
                c.a = 255;
                panelInstance.bundleImage.color = c;
            } else {
                bundleImage.sprite = null;
            }
        }
        
        if (entry.ImageEntries != null && entry.ImageEntries.Count > 0) {
            stickerImage.gameObject.SetActive(true);
            entryImageUrl = entry.ImageEntries[0].ImageUrl;
            
            if (entryImageUrl != null) {
                Debug.Log("Image already preloaded with path: " + entryImageUrl);
                Spil.Instance.LoadImage(stickerImage, entryImageUrl);
                
                Color c = panelInstance.stickerImage.color;
                c.a = 255;
                panelInstance.stickerImage.color = c;
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
        if (localPath.Equals(entryImageUrl)) {
            panelInstance.stickerImage.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2());
        } else {
            panelInstance.bundleImage.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2());
        }
    }

    public void OnImageLoadSuccess(string localPath, ImageContext imageContext) {
        if (localPath != null) {
            Debug.Log("Finished downloading image with local path: " + localPath);
            if (localPath.Equals(entryImageUrl)) {
                Spil.Instance.LoadImage(stickerImage, localPath);
                
                Color c = panelInstance.stickerImage.color;
                c.a = 255;
                panelInstance.stickerImage.color = c;
            } else {
                Spil.Instance.LoadImage(bundleImage, localPath);
                
                Color c = panelInstance.bundleImage.color;
                c.a = 255;
                panelInstance.bundleImage.color = c;
            }                
        }
    }
}
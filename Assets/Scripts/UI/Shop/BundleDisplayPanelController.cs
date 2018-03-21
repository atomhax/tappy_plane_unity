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

    public Image promoImage;

    public GameObject promotionScreenButton;

    private static string entryImageUrl;
    private static string promoImageUrl;

    private Promotion bundlePromotion;

    void Start() {
        Spil.Instance.OnImageLoaded -= OnImageLoaded;
        Spil.Instance.OnImageLoaded += OnImageLoaded;

        Spil.Instance.OnImageLoadSuccess -= OnImageLoadSuccess;
        Spil.Instance.OnImageLoadSuccess += OnImageLoadSuccess;
        
        Spil.Instance.OnImageLoadFailed -= OnImageLoadFailed;
        Spil.Instance.OnImageLoadFailed += OnImageLoadFailed;
        
        Spil.Instance.OnPromotionAmountBought -= OnPromotionAmountBought;
        Spil.Instance.OnPromotionAmountBought += OnPromotionAmountBought;
    }

    public void SetupBundleDisplayPanel(Bundle bundle, Entry entry) {
        panelInstance = this;
        bundleDisplayed = bundle;

        gameObject.SetActive(true);
        
        entryImageUrl = null;
        promoImageUrl = null;
        stickerImage.gameObject.SetActive(false);
        promoImage.gameObject.SetActive(false);
        promotionScreenButton.SetActive(false);
        
        bundlePromotion = Spil.Instance.GetPromotions().GetBundlePromotion(bundleDisplayed.Id);
        if (bundlePromotion != null) {
            promotionScreenButton.SetActive(true);
        }
        starsCostText.text = diamondCostText.text = "0";

        if (bundle.DisplayName != null && !bundle.DisplayName.Equals("")) {
            bundleTitleText.text = bundle.DisplayName;
        } else {
            bundleTitleText.text = bundle.Name;
        }

        if (bundlePromotion != null) {
            bundleTitleText.text += " - Promotion";
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

        if (bundlePromotion != null) {
            if (bundlePromotion.ExtraEntities.Count > 0) {
                listOfItemsInBundle.text += "\n\nExtra: ";
            }
            foreach (ExtraEntity extraEntity in bundlePromotion.ExtraEntities) {
                if (extraEntity.Type.Equals("CURRENCY")) {
                    Currency currency = Spil.GameData.GetCurrency(extraEntity.Id);
                    listOfItemsInBundle.text += "\n" + "• " + currency.Name + ": " + extraEntity.Amount;
                } else {
                    Item item = Spil.GameData.GetItem(extraEntity.Id);
                    listOfItemsInBundle.text += "\n" + "• " + item.Name + ": " + extraEntity.Amount;
                }
            }
            
            listOfItemsInBundle.text += "\n\nPromo Status: ";
            listOfItemsInBundle.text += "\n" + "• " + "Bought: " + bundlePromotion.AmountPurchased;

            if (bundlePromotion.MaxPurchase > 0) {
                listOfItemsInBundle.text += "\n" + "• " + "Max: " + bundlePromotion.MaxPurchase;
            }

            if (bundlePromotion.GameAsset.Count > 0) {
                promoImage.gameObject.SetActive(true);
                promoImageUrl = bundlePromotion.GameAsset[0].Value;
            
                if (promoImageUrl != null) {
                    Debug.Log("Image already preloaded with path: " + promoImageUrl);
                    Spil.Instance.LoadImage(promoImage, promoImageUrl);
                
                    Color c = panelInstance.promoImage.color;
                    c.a = 255;
                    panelInstance.promoImage.color = c;
                }
            }
        }

        if (bundle.HasImage()) {
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

        bundleImage.preserveAspect = true;
    }

    private void OnImageLoadFailed(ImageContext imageContext, SpilErrorMessage errorMessage) {
        if (bundleDisplayed.Id == 100100) {
            panelInstance.bundleImage.sprite = Sprite.Create(gachaImage, new Rect(0, 0, gachaImage.width, gachaImage.height), new Vector2());
            panelInstance.bundleImage.preserveAspect = true;
        }
    }

    private void OnPromotionAmountBought(int promotionId, int currentAmount, bool maxAmountReached) {
        Debug.Log("Current promo amount: " + currentAmount);
        if (maxAmountReached) {
            Debug.Log("Promtion amount reached!");
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

    public void ShowPromotionScreen() {
        if (bundlePromotion != null) {
            Spil.Instance.ShowPromotionScreen(bundlePromotion.Id);
        }
    }
    
    bool CanAffordBundle() {
        bool canAfford = true;

        if (bundlePromotion != null) {
            for (int i = 0; i < bundlePromotion.PriceOverride.Count; i++) {
                if (Spil.PlayerData.GetCurrencyBalance(bundleDisplayed.Prices[i].CurrencyId) < bundlePromotion.PriceOverride[i].Amount) {
                    canAfford = false;
                }
            }
        } else {
            for (int i = 0; i < bundleDisplayed.Prices.Count; i++) {
                if (Spil.PlayerData.GetCurrencyBalance(bundleDisplayed.Prices[i].CurrencyId) < bundleDisplayed.Prices[i].Value) {
                    canAfford = false;
                }
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
        } else if (localPath.Equals(promoImageUrl)) {
            panelInstance.promoImage.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2());
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
            } else if(localPath.Equals(promoImageUrl)) {
                Spil.Instance.LoadImage(promoImage, localPath);
                
                Color c = panelInstance.promoImage.color;
                c.a = 255;
                panelInstance.promoImage.color = c;
            } else {
                Spil.Instance.LoadImage(bundleImage, localPath);
                
                Color c = panelInstance.bundleImage.color;
                c.a = 255;
                panelInstance.bundleImage.color = c;
            }                
        }
    }
}
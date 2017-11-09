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

    public void SetupBundleDisplayPanel(Bundle bundle) {
        panelInstance = this;
        bundleDisplayed = bundle;
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

        listOfItemsInBundle.text = "Contains:";
        for (int i = 0; i < bundle.Items.Count; i++) {
            Item item = Spil.GameData.GetItem(bundle.Items[i].Id);
            listOfItemsInBundle.text += "\n" + "• " + item.Name;
            for (int x = 0; x < bundle.Prices.Count; x++) {
                if (bundle.Prices[x].CurrencyId == 25) {
                    starsCostText.text = bundle.Prices[x].Value.ToString();
                } else if (bundle.Prices[x].CurrencyId == 28) {
                    diamondCostText.text = bundle.Prices[x].Value.ToString();
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

        if (bundle.HasImage()) {
            string imagePath = bundle.GetImagePath();

            if (imagePath != null) {
                Debug.Log("Image already preloaded with path: " + imagePath);
                Spil.Instance.LoadImage(this, imagePath);
            }
        } else {
            if (bundleDisplayed.Id == 100100) {
                panelInstance.bundleImage.sprite = Sprite.Create(gachaImage, new Rect(0, 0, gachaImage.width, gachaImage.height), new Vector2());
            } else {
                bundleImage.sprite = null;
            }
        }
    }

    private void OnImageLoadFailed(ImageContext imageContext, SpilErrorMessage errorMessage) {
        if (bundleDisplayed.Id == 100100) {
            panelInstance.bundleImage.sprite = Sprite.Create(gachaImage, new Rect(0, 0, gachaImage.width, gachaImage.height), new Vector2());
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
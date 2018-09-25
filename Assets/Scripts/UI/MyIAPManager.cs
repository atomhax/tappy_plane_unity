using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Purchasing;
using SpilGames.Unity;
using SpilGames.Unity.Base.Implementations.Tracking;
using SpilGames.Unity.Helpers;
using SpilGames.Unity.Json;
using SpilGames.Unity.Helpers.IAPPackages;
using SpilGames.Unity.Helpers.PlayerData;
using SpilGames.Unity.Helpers.Promotions;
using SpilGames.Unity.Base.Implementations;
using UnityEngine.UI;
using Facebook.Unity;

using Facebook.Unity.Canvas;

public class MyIAPManager : MonoBehaviour, IStoreListener
{

	public string[] googleProductIDs;

	public string[] appleProductIDs;

	string lastProductSKU = "";

	IStoreController m_StoreController;
	IExtensionProvider m_StoreExtensionProvider;
	IAppleExtensions m_AppleExtensions;

	public Dictionary<string, string> packageCosts = new Dictionary<string,string> ();

	public bool restore = false;

	public IAPPanelController iapPanelController;
	public SkinSelectPanelController skinSelectPanelController;

	public Button RestoreIAPButton;

	void Start ()
	{
		#if UNITY_TVOS
		return;
		#endif

        #if !UNITY_WEBGL || UNITY_EDITOR
		if (m_StoreController == null) {
			InitializePurchasing ();
		}
        #endif
		
		Spil.Instance.OnPackagesAvailable -= OnPackagesAvailable;
		Spil.Instance.OnPackagesAvailable += OnPackagesAvailable;
		
		Spil.Instance.OnPromotionsAvailable -= OnPromotionsAvailable;
		Spil.Instance.OnPromotionsAvailable += OnPromotionsAvailable;
		
		#if !UNITY_IOS
		RestoreIAPButton.gameObject.SetActive(false);
		#endif
	}

	public void OnPackagesAvailable() {
		iapPanelController.SetupIAPButtons();
	}
	
	public void OnPromotionsAvailable() {
		iapPanelController.SetupIAPButtons();
	}
	
	public void InitializePurchasing ()
	{
		#if UNITY_TVOS || UNITY_WEBGL
		return;
		#endif

		if (IsInitialized ()) {
			return;
		}
		var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance ());
		#if UNITY_ANDROID
		for (int i = 0; i < googleProductIDs.Length; i++) {
			builder.AddProduct (googleProductIDs [i], ProductType.Consumable);
		}
		#endif
		#if UNITY_IOS
		for(int i = 0; i < appleProductIDs.Length; i ++){
		builder.AddProduct(appleProductIDs[i], ProductType.Consumable);
		}
		#endif
		UnityPurchasing.Initialize (this, builder);
	}

	private bool IsInitialized ()
	{
		#if UNITY_TVOS || UNITY_WEBGL
		return false;
		#endif

		// Only say we are initialized if both the Purchasing references are set.
		return m_StoreController != null && m_StoreExtensionProvider != null;
	}

	public void BuyProductID (string productId)
	{
		#if UNITY_TVOS
		return;
		#endif

		if (productId.Equals("com.spilgames.tappyplane.goldplane")) {
			foreach (PlayerItem playerItem in Spil.PlayerData.Inventory.Items) {
				if (playerItem.Id == 100291) {
					return;
				}
			}
		}
		
		lastProductSKU = productId;
		
		#if UNITY_EDITOR
		if (productId.Equals("com.spilgames.tappyplane.goldplane")) {
			Spil.Instance.AddItemToInventory(100291, 1, PlayerDataUpdateReasons.IAP, "Splash Screen", null, "EditorTransaction");
		} else {
			RewardPlayer ("EditorTransaction");
		}
		
		iapPanelController.PurchaseSuccess (productId);
		skinSelectPanelController.PurchaseSuccess (productId);
		
		return;
        #elif UNITY_WEBGL          

        iapPanelController.PurchaseStarted ();
        skinSelectPanelController.PurchaseStarted ();

        FB.Canvas.PayWithProductId(productId, "purchaseiap", 1, null, null, null, null, null, PayCallback);

        return;

		#endif

		iapPanelController.PurchaseStarted ();
		skinSelectPanelController.PurchaseStarted ();
		
		// If the stores throw an unexpected exception, use try..catch to protect my logic here.
		try {
			// If Purchasing has been initialized ...
			if (IsInitialized ()) {
				// ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
				Product product = m_StoreController.products.WithID (productId);

				// If the look up found a product for this device's store and that product is ready to be sold ... 
				if (product != null && product.availableToPurchase) {
					Debug.Log (string.Format ("Purchasing product asychronously: '{0}'", product.definition.id));// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
					m_StoreController.InitiatePurchase (product);
				}
				// Otherwise ...
				else {
					// ... report the product look-up failure situation  
					Debug.Log ("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
				}
			}
			// Otherwise ...
			else {
				// ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or retrying initiailization.
				Debug.Log ("BuyProductID FAIL. Not initialized.");
			}
		}
		// Complete the unexpected exception handling ...
		catch (Exception e) {
			// ... by reporting any unexpected exception for later diagnosis.
			Debug.Log ("BuyProductID: FAIL. Exception during purchase. " + e);
			iapPanelController.pleaseWaitPanel.SetActive(false);
			skinSelectPanelController.pleaseWaitPanel.SetActive(false);
		}
	}

	public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
	{
		#if UNITY_TVOS || UNITY_WEBGL
		return;
		#endif

		packageCosts.Clear ();
		m_StoreController = controller;
		
		m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
		m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);
		
		m_StoreExtensionProvider = extensions;
		StoreProductPrices ();
	}

	void StoreProductPrices ()
	{
		#if UNITY_TVOS || UNITY_WEBGL
		return;
		#endif

		for (int i = 0; i < m_StoreController.products.all.Length; i++) {
			packageCosts.Add (m_StoreController.products.all [i].definition.storeSpecificId, m_StoreController.products.all [i].metadata.localizedPriceString);
		}
		iapPanelController.SetupIAPButtons ();
	}

	public void OnInitializeFailed (InitializationFailureReason error)
	{
		#if UNITY_TVOS || UNITY_WEBGL
		return;
		#endif

		Invoke ("InitializePurchasing", 1);
	}

	public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs args)
	{
		#if UNITY_TVOS || UNITY_WEBGL
		return PurchaseProcessingResult.Complete;
		#endif

        string skuId = "";
        string transactionID = "";

		#if UNITY_ANDROID
		string token = "";
	
		switch (Spil.MonoInstance.AndroidStore.ToString()) {
				case "GooglePlay":
					//parse the json
					Debug.Log(args.purchasedProduct.receipt);
					Dictionary<String,object> hashOfReceipt = JsonConvert.DeserializeObject<Dictionary<String,object>>(args.purchasedProduct.receipt);
					string stringOfPayload = hashOfReceipt ["Payload"].ToString ();
					JSONObject jsonFromObject = new JSONObject (stringOfPayload);
					string jsonFieldString = jsonFromObject.GetField ("json").str;
					jsonFieldString = jsonFieldString.Replace (@"\", "");
					JSONObject finalJsonObject = new JSONObject (jsonFieldString);

					//the info to track
					skuId = args.purchasedProduct.definition.id;
					transactionID = args.purchasedProduct.transactionID;
					token = finalJsonObject.GetField ("purchaseToken").str;

					if(transactionID == null || transactionID.Equals("")){
						transactionID = token;
					}

					SpilTracking.IAPPurchased(skuId, transactionID)
						.AddToken(token)
						.AddReason("purchase")
						.AddLocation("store")
						.Track();
					break;
				case "Amazon":
					skuId = args.purchasedProduct.definition.id;
					transactionID = args.purchasedProduct.transactionID;
					token = transactionID;

					string localPrice = args.purchasedProduct.metadata.localizedPriceString;
					string localCurrency = args.purchasedProduct.metadata.isoCurrencyCode;
					
					Debug.Log(args.purchasedProduct.receipt);
					Dictionary<String,object> receipt = JsonConvert.DeserializeObject<Dictionary<String,object>>(args.purchasedProduct.receipt);
					JSONObject payloadJSON = new JSONObject (receipt ["Payload"].ToString ());
					string amazonUserId = "";
					if (payloadJSON.HasField("userId")) {
						amazonUserId = payloadJSON.GetField("userId").str;
					}
					
					SpilTracking.IAPPurchased(skuId, transactionID)
						.AddToken(token)
						.AddReason("purchase")
						.AddLocation("store")
						.AddLocalPrice(localPrice)
						.AddLocalCurrency(localCurrency)
						.AddAmazonUserId(amazonUserId)
						.Track();
					break;
		}


		#elif UNITY_IOS
		
		skuId = args.purchasedProduct.definition.id;
		transactionID = args.purchasedProduct.transactionID;

		if (!restore) {
			Spil.Instance.TrackIAPPurchasedEvent(skuId, transactionID, null, "diamondPurchase", "store");
		}
		else {
			Spil.Instance.TrackIAPRestoredEvent(skuId, transactionID, "2018-04-30T11:54:48.5247936+02:00", "IAP Restore"); // TODO: How to get the original purchase date?
		}
		
		#endif

		if (skuId.Equals("com.spilgames.tappyplane.goldplane")) {
			Spil.Instance.AddItemToInventory(100291, 1, PlayerDataUpdateReasons.IAP, "Splash Screen", null, transactionID);
		} else {
			RewardPlayer (args.purchasedProduct.transactionID);
		}	

		iapPanelController.PurchaseSuccess (args.purchasedProduct.metadata.localizedTitle);
		skinSelectPanelController.PurchaseSuccess (args.purchasedProduct.metadata.localizedTitle);
		return PurchaseProcessingResult.Complete;
	}

	void RewardPlayer (String transactionId)
	{
		#if UNITY_TVOS
		return;
		#endif

		PackagesHelper helper = Spil.Instance.GetPackages ();
		for (int i = 0; i < helper.Packages.Count; i++) {
			if (lastProductSKU == helper.Packages [i].PackageId) {
				int packageValue = int.Parse(helper.Packages[i].Items[0].Value.Replace(".0", ""));
				if (helper.Packages[i].HasActivePromotion()) {
					Promotion packagePromotion = Spil.Instance.GetPromotions().GetPackagePromotion(helper.Packages[i].PackageId);
					packageValue = packageValue + packagePromotion.ExtraEntities[0].Amount;
				}
                // TODO: This only works for IAPs that reward currencies?
				Spil.PlayerData.Wallet.Add (int.Parse (helper.Packages [i].Items [0].Id), packageValue, PlayerDataUpdateReasons.IAP, "Shop", "Shop Purchase", transactionId);
			}
		}

	}

	public void OnPurchaseFailed (Product product, PurchaseFailureReason failureReason)
	{
		#if UNITY_TVOS || UNITY_WEBGL
		return;
		#endif

		iapPanelController.PurchaseFailed ();
		skinSelectPanelController.PurchaseFailed ();
		SpilTracking.IAPFailed(product.definition.storeSpecificId, failureReason.ToString()).Track();
	}
	
	public void RestoreIAPs()
	{
		restore = true;
		m_AppleExtensions.RestoreTransactions(OnTransactionsRestored);
	}
	
	private void OnTransactionsRestored(bool success)
	{
		Debug.Log("Transactions restored.");
		restore = false;
	}
	
	private void OnDeferred(Product item)
	{
		Debug.Log("Purchase deferred: " + item.definition.id);
		/*if (item.definition.id.Equals("com.spilgames.tappyplane.goldplane"))
		{
			Spil.Instance.AddItemToInventory(100291, 1, PlayerDataUpdateReasons.IAP, "IAP Restore", null, null);
			Spil.Instance.TrackIAPRestoredEvent(item.definition.id, item.transactionID, item, "IAP Restore");
		}*/
	}

    // FB IAP's

    // Called on app start to request IAP data
    public void requestFBIAPS()
    {
        FB.API(
          "app/products",
          HttpMethod.GET,
          productCallback // callback that receives a IGraphResult
        );
    }

    void productCallback(IGraphResult result)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JSONObject graphResult = new JSONObject(JsonHelper.getJSONFromObject(result));

        packageCosts.Clear();

        // Fill iapDetails for splash screens, fill packageCosts for iapPanel
        JSONObject iapDetails = new JSONObject();

        foreach (JSONObject product in graphResult.GetField("ResultDictionary").GetField("data"))
        {
            JSONObject iapDetailsProduct = new JSONObject();
            iapDetailsProduct.AddField("title", product.GetField("title"));
            iapDetailsProduct.AddField("currency", product.GetField("price_currency_code"));
            iapDetailsProduct.AddField("price", product.GetField("price"));
            iapDetailsProduct.AddField("skuId", product.GetField("product_id"));
            iapDetails.Add(iapDetailsProduct);

            packageCosts.Add(product.GetField("product_id").str, product.GetField("price").str);
        }

        SpilWebGLJavaScriptInterface.iapDetailsWebGL = iapDetails;

        iapPanelController.SetupIAPButtons();
#endif
    }

#if UNITY_WEBGL
    void PayCallback(IPayResult result)
    {
        iapPanelController.pleaseWaitPanel.SetActive(false);
        skinSelectPanelController.pleaseWaitPanel.SetActive(false);

        if (result != null)
        {
            if(result.Error == null)
            {
                var response = result.ResultDictionary;
                if (Convert.ToString(response["status"]) == "completed")
                {
                    if (((string)result.ResultDictionary["product_id"]).Equals("com.spilgames.tappyplane.goldplane"))
                    {
                        Spil.Instance.AddItemToInventory(100291, 1, PlayerDataUpdateReasons.IAP, "Splash Screen", null, "EditorTransaction");
                    } else {
                        RewardPlayer((string)result.ResultDictionary["payment_id"]);
                    }

                    String localisedName = null;
                    foreach(JSONObject product in SpilWebGLJavaScriptInterface.iapDetailsWebGL)
                    {
                        if(product.GetField("skuId").str.Equals((string)result.ResultDictionary["product_id"]))
                        {
                            localisedName = product.GetField("title").str;
                            break;
                        }
                    }

                    // TODO: Add store parameter? (amazon/facebook?)
                    SpilTracking.IAPPurchased((string)result.ResultDictionary["product_id"], (string)result.ResultDictionary["payment_id"])
                    .AddToken((string)result.ResultDictionary["purchase_token"])
                    .AddReason("purchase")
                    .AddLocation("store")
                    .AddLocalPrice((string)result.ResultDictionary["amount"])
                    .AddLocalCurrency((string)result.ResultDictionary["currency"])
                    .Track();

                    FB.API(
                        "/" + (string)result.ResultDictionary["purchase_token"] + "/consume?access_token=" + AccessToken.CurrentAccessToken.TokenString,
                        HttpMethod.POST,
                        this.consumeCallback // callback that receives a IGraphResult
                    );

                    iapPanelController.PurchaseSuccess(localisedName);
                    skinSelectPanelController.PurchaseSuccess(localisedName);

                } else {
                    // Payment not yet complete? TODO: When does this happen?
                    SpilTracking.IAPFailed(lastProductSKU, "IAP status: " + Convert.ToString(response["status"])).Track();
                    iapPanelController.PurchaseFailed();
                    skinSelectPanelController.PurchaseFailed();
                }
            } else {
                SpilTracking.IAPFailed(lastProductSKU, result.Error).Track();
                iapPanelController.PurchaseFailed();
                skinSelectPanelController.PurchaseFailed();
            }
        } else {
            SpilTracking.IAPFailed(lastProductSKU, "IPayResult was null").Track();
            iapPanelController.PurchaseFailed();
            skinSelectPanelController.PurchaseFailed();
        }

        // Give the WebGL player focus or it won't update the UI for WebGL
        SpilWebGLJavaScriptInterface.GivePlayerFocusJS();
    }
#endif
	
    // RawResult should contain: { success: true }
    void consumeCallback(IGraphResult result) { }        
}
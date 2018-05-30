using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Purchasing;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;
using SpilGames.Unity.Json;
using SpilGames.Unity.Helpers.IAPPackages;
using SpilGames.Unity.Helpers.PlayerData;
using SpilGames.Unity.Helpers.Promotions;


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


	void Start ()
	{
		#if UNITY_TVOS
		return;
		#endif

		if (m_StoreController == null) {
			InitializePurchasing ();
		}
		
		Spil.Instance.OnPackagesAvailable -= OnPackagesAvailable;
		Spil.Instance.OnPackagesAvailable += OnPackagesAvailable;
		
		Spil.Instance.OnPromotionsAvailable -= OnPromotionsAvailable;
		Spil.Instance.OnPromotionsAvailable += OnPromotionsAvailable;
	}

	public void OnPackagesAvailable() {
		iapPanelController.SetupIAPButtons();
	}
	
	public void OnPromotionsAvailable() {
		iapPanelController.SetupIAPButtons();
	}
	
	public void InitializePurchasing ()
	{
		#if UNITY_TVOS
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
		#if UNITY_TVOS
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
		
		iapPanelController.PurchaseStarted ();
		skinSelectPanelController.PurchaseStarted ();
		lastProductSKU = productId;
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
		#if UNITY_TVOS
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
		#if UNITY_TVOS
		return;
		#endif

		for (int i = 0; i < m_StoreController.products.all.Length; i++) {
			packageCosts.Add (m_StoreController.products.all [i].definition.storeSpecificId, m_StoreController.products.all [i].metadata.localizedPriceString);
		}
		iapPanelController.SetupIAPButtons ();
	}

	public void OnInitializeFailed (InitializationFailureReason error)
	{
		#if UNITY_TVOS
		return;
		#endif

		Invoke ("InitializePurchasing", 1);
	}

	public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs args)
	{
		#if UNITY_TVOS
		return PurchaseProcessingResult.Complete;
		#endif

		#if UNITY_ANDROID

		//parse the json
		
		Dictionary<String,object> hashOfReceipt = JsonConvert.DeserializeObject<Dictionary<String,object>>(args.purchasedProduct.receipt);
		string stringOfPayload = hashOfReceipt ["Payload"].ToString ();
		JSONObject jsonFromObject = new JSONObject (stringOfPayload);
		string jsonFieldString = jsonFromObject.GetField ("json").str;
		jsonFieldString = jsonFieldString.Replace (@"\", "");
		JSONObject finalJsonObject = new JSONObject (jsonFieldString);

		//the info to track

		string skuId = args.purchasedProduct.definition.id;
		string transactionID = args.purchasedProduct.transactionID;
		string token = finalJsonObject.GetField ("purchaseToken").str;

		if(transactionID == null || transactionID.Equals("")){
			transactionID = token;
		}

		Spil.Instance.TrackIAPPurchasedEvent (skuId, transactionID, token, "purchase", "store");

		#elif UNITY_IOS
		
		string skuId = args.purchasedProduct.definition.id;
		string transactionID = args.purchasedProduct.transactionID;

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
				Spil.PlayerData.Wallet.Add (int.Parse (helper.Packages [i].Items [0].Id), packageValue, PlayerDataUpdateReasons.IAP, "Shop", "Shop Purchase", transactionId);
			}
		}

	}

	public void OnPurchaseFailed (Product product, PurchaseFailureReason failureReason)
	{
		#if UNITY_TVOS
		return;
		#endif

		iapPanelController.PurchaseFailed ();
		skinSelectPanelController.PurchaseFailed ();
		Spil.Instance.TrackIAPFailedEvent (failureReason.ToString (), product.definition.storeSpecificId);
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
}
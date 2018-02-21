using System;
using System.Collections.Generic;
using Prime31;
using SpilGames.Unity;
using SpilGames.Unity.Helpers.IAPPackages;
using SpilGames.Unity.Helpers.PlayerData;
using UnityEngine;

public class PrimeIAPManager : MonoBehaviour {
    public string[] googleProducts;
	public string googleIAPKey;
	
    public string[] appleProducts;

    public Dictionary<string, string> packageCosts = new Dictionary<string, string>();

    public IAPPanelController iapPanelController;

	private bool isInitialized;
	
	string lastProductSKU = "";
	
    void Start() {
        #if UNITY_TVOS
		return;
		#endif

        InitializePurchasing();
    }

	void InitializePurchasing() {
    	#if UNITY_TVOS
		return;
		#endif
	    
		if (IsInitialized ()) {
			return;
		}
		
		#if UNITY_ANDROID
		GoogleIAB.enableLogging(true);
	    GoogleIABManager.billingSupportedEvent -= GoogleIabManagerOnBillingSupportedEvent;
	    GoogleIABManager.billingSupportedEvent += GoogleIabManagerOnBillingSupportedEvent;
		
		GoogleIABManager.billingNotSupportedEvent -= GoogleIabManagerOnBillingNotSupportedEvent;
		GoogleIABManager.billingNotSupportedEvent += GoogleIabManagerOnBillingNotSupportedEvent;
		
		GoogleIABManager.purchaseSucceededEvent -= GoogleIabManagerOnPurchaseSucceededEvent;
		GoogleIABManager.purchaseSucceededEvent += GoogleIabManagerOnPurchaseSucceededEvent;
		
		GoogleIABManager.purchaseFailedEvent -= GoogleIabManagerOnPurchaseFailedEvent;
		GoogleIABManager.purchaseFailedEvent += GoogleIabManagerOnPurchaseFailedEvent;
		#endif
		
		#if UNITY_IOS
		if(StoreKitBinding.canMakePayments()) {
			isInitialized = true;
		}
		#endif
		
		IAP.init(googleIAPKey);
		
		#if UNITY_IOS
		StoreKitManager.purchaseSuccessfulEvent -= StoreKitManagerOnPurchaseSuccessfulEvent;
		StoreKitManager.purchaseSuccessfulEvent += StoreKitManagerOnPurchaseSuccessfulEvent;
		
		StoreKitManager.purchaseFailedEvent -= StoreKitManagerOnPurchaseFailedEvent;
		StoreKitManager.purchaseFailedEvent += StoreKitManagerOnPurchaseFailedEvent;

		StoreKitManager.purchaseCancelledEvent -= StoreKitManagerOnPurchaseCancelledEvent;
		StoreKitManager.purchaseCancelledEvent += StoreKitManagerOnPurchaseCancelledEvent;
		
		RequestIAPData();
		string restoredPurchases = PlayerPrefs.GetString("iosRestoredPurchase", "false");

		if (restoredPurchases.Equals("false")) {
			StoreKitBinding.restoreCompletedTransactions();
			PlayerPrefs.SetString("iosRestoredPurchase", "true");
		}
		
		#endif
	}

	private void GoogleIabManagerOnBillingSupportedEvent() {
		isInitialized = true;
		RequestIAPData();
	}

	private void GoogleIabManagerOnBillingNotSupportedEvent(string s) {
		isInitialized = false;
	}
	
	private bool IsInitialized () {
		#if UNITY_TVOS
		return false;
		#endif

		if (isInitialized) {
			return true;
		}

		return false;
	}

	private void RequestIAPData() {
		if (!isInitialized) {
			return;
		}
		
		IAP.requestProductData(appleProducts, googleProducts, IAPRequestDataHandler);
	}

	private void IAPRequestDataHandler(List<IAPProduct> iapProducts) {
		#if UNITY_TVOS
		return;
		#endif

		foreach (IAPProduct iapProduct in iapProducts) {
			packageCosts.Add(iapProduct.productId, iapProduct.price);
		}
		
		iapPanelController.SetupIAPButtons ();
	}
	
	public void BuyProductID (string productId)
	{
		#if UNITY_TVOS
		return;
		#endif

		iapPanelController.PurchaseStarted ();
		lastProductSKU = productId;

		try {
			if (IsInitialized ()) {
				IAP.purchaseConsumableProduct(lastProductSKU, PurchaseIAPHandler);
				Debug.Log (string.Format ("Purchasing product asychronously: '{0}'", productId));
			}
			else {
				Debug.Log ("BuyProductID FAIL. Not initialized.");
			}
		}
		catch (Exception e) {
			Debug.Log ("BuyProductID: FAIL. Exception during purchase. " + e);
		}
	}

	private void PurchaseIAPHandler(bool valid, string error) {
		if (valid) {
			Debug.Log("IAP Successful!");
		} else {
			Debug.Log("IAP failed with error: " + error);
		}
	}
	
	#if UNITY_ANDROID
	private void GoogleIabManagerOnPurchaseSucceededEvent(GooglePurchase googlePurchase) {		
		string skuId = googlePurchase.productId;
		string transactionID = googlePurchase.orderId;
		string token = googlePurchase.purchaseToken;

		if(transactionID == null || transactionID.Equals("")){
			transactionID = token;
		}

		Spil.Instance.TrackIAPPurchasedEvent (skuId, transactionID, token);

		RewardPlayer (transactionID);

		iapPanelController.PurchaseSuccess (googlePurchase.productId);
	}
	
	private void GoogleIabManagerOnPurchaseFailedEvent(string s, int i) {
		iapPanelController.PurchaseFailed ();
		Spil.Instance.TrackIAPFailedEvent (s, lastProductSKU);
	}
	#endif
	
	#if UNITY_IOS
	private void StoreKitManagerOnPurchaseSuccessfulEvent(StoreKitTransaction storeKitTransaction) {
		string skuId = storeKitTransaction.productIdentifier;
		string transactionID = storeKitTransaction.transactionIdentifier;

		Spil.Instance.TrackIAPPurchasedEvent (skuId, transactionID);

		RewardPlayer (transactionID);

		iapPanelController.PurchaseSuccess (skuId);
	}
	
	private void StoreKitManagerOnPurchaseFailedEvent(string s) {
		iapPanelController.PurchaseFailed ();
		Spil.Instance.TrackIAPFailedEvent (s, lastProductSKU);
	}

	private void StoreKitManagerOnPurchaseCancelledEvent(string s) {
		iapPanelController.PurchaseFailed ();
		Spil.Instance.TrackIAPFailedEvent (s, lastProductSKU);
	}
	#endif
	
	void RewardPlayer (String transactionId) {
		#if UNITY_TVOS
		return;
		#endif

		PackagesHelper helper = Spil.Instance.GetPackagesAndPromotions ();
		for (int i = 0; i < helper.Packages.Count; i++) {
			if (lastProductSKU == helper.Packages [i].Id) {
				Spil.PlayerData.Wallet.Add (int.Parse (helper.Packages [i].Items [0].Id), int.Parse (helper.Packages [i].Items [0].GetRealValue ().Replace (".0", "")), PlayerDataUpdateReasons.IAP, "Shop", transactionId);
			}
		}

	}
}
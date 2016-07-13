using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;


public class MyIAPManager : MonoBehaviour, IStoreListener {

	public string[] googleProductIDs;

	public string[] appleProductIDs;

	string lastProduct = "";

	//the store apa connectors
	IStoreController m_StoreController;
	IExtensionProvider m_StoreExtensionProvider;

	public Dictionary<string, string> packageCosts = new Dictionary<string,string>();

	public IAPPanelController iapPanelController;


	void Start()
	{
		// If we haven't set up the Unity Purchasing reference
		if (m_StoreController == null)
		{
			// Begin to configure our connection to Purchasing
			InitializePurchasing();
		}

	}


	public void InitializePurchasing() 
	{
		// If we have already connected to Purchasing ...
		if (IsInitialized())
		{
			// ... we are done here.
			return;
		}
		// Create a builder, first passing in a suite of Unity provided stores.
		var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		// Add a product to sell / restore by way of its identifier, associating the general identifier with its store-specific identifiers.


		#if UNITY_ANDROID
		for(int i = 0; i < googleProductIDs.Length; i ++){
			builder.AddProduct(googleProductIDs[i], ProductType.Consumable);
		}
		#endif
		#if UNITY_IOS
		for(int i = 0; i < appleProductIDs.Length; i ++){
		builder.AddProduct(appleProductIDs[i], ProductType.Consumable);
		}
		#endif


		UnityPurchasing.Initialize(this, builder);
	}

	private bool IsInitialized()
	{
		// Only say we are initialized if both the Purchasing references are set.
		return m_StoreController != null && m_StoreExtensionProvider != null;
	}

	public void BuyProductID(string productId)
	{
		iapPanelController.PurchaseStarted ();
		lastProduct = productId;
		// If the stores throw an unexpected exception, use try..catch to protect my logic here.
		try
		{
			// If Purchasing has been initialized ...
			if (IsInitialized())
			{
				// ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
				Product product = m_StoreController.products.WithID(productId);

				// If the look up found a product for this device's store and that product is ready to be sold ... 
				if (product != null && product.availableToPurchase)
				{
					Debug.Log (string.Format("Purchasing product asychronously: '{0}'", product.definition.id));// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
					m_StoreController.InitiatePurchase(product);
				}
				// Otherwise ...
				else
				{
					// ... report the product look-up failure situation  
					Debug.Log ("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
				}
			}
			// Otherwise ...
			else
			{
				// ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or retrying initiailization.
				Debug.Log("BuyProductID FAIL. Not initialized.");
			}
		}
		// Complete the unexpected exception handling ...
		catch (Exception e)
		{
			// ... by reporting any unexpected exception for later diagnosis.
			Debug.Log ("BuyProductID: FAIL. Exception during purchase. " + e);
		}
	}

	//  
	// --- IStoreListener
	//

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		packageCosts.Clear ();
		// Purchasing has succeeded initializing. Collect our Purchasing references.
//		Debug.Log("OnInitialized: PASS");

		// Overall Purchasing system, configured with products for this application.
		m_StoreController = controller;
		// Store specific subsystem, for accessing device-specific store features.
		m_StoreExtensionProvider = extensions;



		for(int i = 0 ; i < m_StoreController.products.all.Length; i ++ ){
			packageCosts.Add (m_StoreController.products.all[i].definition.storeSpecificId ,m_StoreController.products.all[i].metadata.localizedPriceString);
//			Debug.Log ("PURCHASE INIT: " + m_StoreController.products.all[i].definition.storeSpecificId);
		}
		iapPanelController.SetupIAPButtons ();
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		// Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
//		Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);

		Invoke ("InitializePurchasing",1);
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) 
	{

		RewardPlayer ();

		iapPanelController.PurchaseSuccess(args.purchasedProduct.metadata.localizedTitle);

		return PurchaseProcessingResult.Complete;
	}

	void RewardPlayer(){

		PackagesHelper helper = Spil.Instance.GetPackagesAndPromotions ();

//		Debug.Log ("REWARDING PLAYER: " + lastProduct);

		for(int i = 0; i < helper.Packages.Count; i ++){
//			Debug.Log ("VALUE: " + helper.Packages [i].Items [0].GetRealValue ());
			if(lastProduct == helper.Packages[i].Id){
				Spil.SpilPlayerDataInstance.Wallet.Add (int.Parse (helper.Packages [i].Items [0].Id), int.Parse (helper.Packages [i].Items [0].GetRealValue ().Replace(".0","")), PlayerDataUpdateReasons.IAP);
//				Debug.Log ("REWARDING PLAYER: " + helper.Packages [i].Items [0].GetRealValue ());
			}
		}


	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
		iapPanelController.PurchaseFailed ();
		Spil.Instance.SendiapFailedEvent (failureReason.ToString (), product.definition.storeSpecificId);
		// A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing this reason with the user.
//		Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",product.definition.storeSpecificId, failureReason));
	}

}
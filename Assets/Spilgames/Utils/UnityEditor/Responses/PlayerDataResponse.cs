#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using SpilGames.Unity.Implementations;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;
using SpilGames.Unity.Utils.UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace SpilGames.Unity.Utils.UnityEditor.Responses
{
	public class PlayerDataResponse : Response
	{

		public WalletData Wallet;
		public InventoryData Inventory;

		public void ProcessPlayerDataResponse (ResponseEvent response)
		{

			WalletData receivedWallet = null;
			InventoryData receivedInventory = null;

			if (response.data.HasField ("wallet")) {
				
				JSONObject walletJSON = response.data.GetField ("wallet");

				receivedWallet = new WalletData ();

				if (walletJSON.HasField ("currencies")) {
					receivedWallet.currencies = new List<PlayerCurrencyData> ();

					JSONObject currenciesJSON = walletJSON.GetField ("currencies");

					for (int i = 0; i < currenciesJSON.Count; i++) {
						PlayerCurrencyData currency = new PlayerCurrencyData ();

						currency.id = (int)currenciesJSON.list [i].GetField ("id").n;
						currency.currentBalance = (int)currenciesJSON.list [i].GetField ("currentBalance").n;
						currency.delta = (int)currenciesJSON.list [i].GetField ("delta").n;

						receivedWallet.currencies.Add (currency);
					}
				}

				receivedWallet.offset = (int)walletJSON.GetField ("offset").n;
				receivedWallet.logic = walletJSON.GetField ("logic").str;

			}

			if (response.data.HasField ("inventory")) {

				JSONObject inventoryJSON = response.data.GetField ("inventory");

				receivedInventory = new InventoryData ();

				if (inventoryJSON.HasField ("items")) {
					receivedInventory.items = new List<PlayerItemData> ();

					JSONObject itemsJSON = inventoryJSON.GetField ("items");

					for (int i = 0; i < itemsJSON.Count; i++) {
						PlayerItemData item = new PlayerItemData ();

						item.id = (int)itemsJSON.list [i].GetField ("id").n;
						item.amount = (int)itemsJSON.list [i].GetField ("amount").n;
						item.delta = (int)itemsJSON.list [i].GetField ("delta").n;

						receivedInventory.items.Add (item);
					}
				}

				receivedInventory.offset = (int)inventoryJSON.GetField ("offset").n;
				receivedInventory.logic = inventoryJSON.GetField ("logic").str;
				
			}

			CalculatePlayerDataResponse (receivedWallet, receivedInventory);

		}

		public WalletData InitWallet ()
		{
			if (Wallet == null) {
				TempUserInfo temp;
				try{
					string playerData = System.IO.File.ReadAllText (Application.streamingAssetsPath + "/defaultPlayerData.json");
					temp = JsonHelper.getObjectFromJson<TempUserInfo> (playerData);
				} catch(FileNotFoundException e){
					Debug.Log("defaultPlayerData.json not found. Creating a placeholder!" + e.ToString());
					string placeholder = "{\"wallet\":{\"currencies\":[],\"offset\": 0,\"logic\": \"CLIENT\"},\"inventory\":{\"items\":[],\"offset\":0,\"logic\": \"\"}}";
					temp = JsonHelper.getObjectFromJson<TempUserInfo> (placeholder);
				}



				return temp.wallet;
			}

			return Wallet;
		}

		public void SetInitalWalletValues(){

			string init = PlayerPrefs.GetString("walletInit-" + Spil.SpilUserIdEditor, "false");

			if(Wallet != null && init.Equals("false")){

				for(int i = 0; i < SpilUnityEditorImplementation.gData.currencies.Count; i++){
					if(SpilUnityEditorImplementation.gData.currencies[i].initialValue > 0){
						WalletOperation("add", SpilUnityEditorImplementation.gData.currencies[i].id, SpilUnityEditorImplementation.gData.currencies[i].initialValue, PlayerDataUpdateReasons.InitialValue, "", null);
					}
				}

			}

			PlayerPrefs.SetString("walletInit-" + Spil.SpilUserIdEditor, "true");
			
		}

		public void ResetWallet(){
			for(int i = 0; i < SpilUnityEditorImplementation.pData.Wallet.currencies.Count; i++){
				int initialValue = SpilUnityEditorImplementation.gData.currencies.FirstOrDefault(a => a.id == SpilUnityEditorImplementation.pData.Wallet.currencies[i].id).initialValue;
				int newDelta = initialValue - SpilUnityEditorImplementation.pData.Wallet.currencies[i].currentBalance;

				SpilUnityEditorImplementation.pData.Wallet.currencies[i].currentBalance = initialValue;
				SpilUnityEditorImplementation.pData.Wallet.currencies[i].delta = newDelta + SpilUnityEditorImplementation.pData.Wallet.currencies[i].delta;
			}
		}

		public InventoryData InitInventory ()
		{

			if (Inventory == null) {
				TempUserInfo temp;
				try{
					string playerData = System.IO.File.ReadAllText (Application.streamingAssetsPath + "/defaultPlayerData.json");
					temp = JsonHelper.getObjectFromJson<TempUserInfo> (playerData);
				} catch(FileNotFoundException e){
					Debug.Log("defaultPlayerData.json not found. Creating a placeholder! " + e.ToString());
					string placeholder = "{\"wallet\":{\"currencies\":[],\"offset\": 0,\"logic\": \"CLIENT\"},\"inventory\":{\"items\":[],\"offset\":0,\"logic\": \"\"}}";
					temp = JsonHelper.getObjectFromJson<TempUserInfo> (placeholder);
				}


				return temp.inventory;
			}

			return Inventory;
		}

		public void SetInitalInventoryValues(){

			string init = PlayerPrefs.GetString("inventoryInit-" + Spil.SpilUserIdEditor, "false");

			if(Inventory != null && init.Equals("false")){

				for(int i = 0; i < SpilUnityEditorImplementation.gData.items.Count; i++){
					if(SpilUnityEditorImplementation.gData.items[i].initialValue > 0){
						InventoryOperation("add", SpilUnityEditorImplementation.gData.items[i].id, SpilUnityEditorImplementation.gData.items[i].initialValue, PlayerDataUpdateReasons.InitialValue, "", null);
					}
				}

			}

			PlayerPrefs.SetString("inventoryInit-" + Spil.SpilUserIdEditor, "true");
			
		}

		public void ResetInventory(){
			for(int i = 0; i < SpilUnityEditorImplementation.pData.Inventory.items.Count; i++){
				int initialValue = SpilUnityEditorImplementation.gData.items.FirstOrDefault(a => a.id == SpilUnityEditorImplementation.pData.Inventory.items[i].id).initialValue;
				int newDelta = initialValue - SpilUnityEditorImplementation.pData.Inventory.items[i].amount;

				SpilUnityEditorImplementation.pData.Inventory.items[i].amount = initialValue;
				SpilUnityEditorImplementation.pData.Inventory.items[i].delta = newDelta + SpilUnityEditorImplementation.pData.Inventory.items[i].delta;
			}
		}

		public void ResetPlayerData(){
			ResetWallet();
			ResetInventory();
		}

		private void CalculatePlayerDataResponse (WalletData receivedWallet, InventoryData receivedInventory)
		{
			bool updated = false;
			PlayerDataUpdatedData updatedData = new PlayerDataUpdatedData ();

			if (receivedWallet != null) {

				for (int i = 0; i < Wallet.currencies.Count; i++) {
					Wallet.currencies[i].delta = 0;
				}

				if (Wallet.offset < receivedWallet.offset && receivedWallet.currencies.Count > 0) {

					for (int i = 0; i < receivedWallet.currencies.Count; i++) {
						if (receivedWallet.logic.Equals ("CLIENT")) {

							PlayerCurrencyData currency = Wallet.currencies.FirstOrDefault(a => a.id == receivedWallet.currencies[i].id);

							if(currency != null){

								if (Wallet.offset == 0 && receivedWallet.offset != 0) {
									currency.currentBalance = receivedWallet.currencies [i].currentBalance;
								} else {
									if(receivedWallet.currencies [i].delta != 0){
										int updatedBalance = currency.currentBalance + receivedWallet.currencies [i].delta;

										if (updatedBalance < 0) {
											updatedBalance = 0;
										}

										currency.currentBalance = updatedBalance;
									}

								}

								updated = true;
								updatedData.currencies.Add (currency);
							}

							
						} else if (receivedWallet.logic.Equals ("SERVER")) {
							
						}
					}

				}

				Wallet.offset = receivedWallet.offset;
				Wallet.logic = receivedWallet.logic;

			}

			if (receivedInventory != null) {
				for (int i = 0; i < Inventory.items.Count; i++) {
					Inventory.items[i].delta = 0;
				}

				if(Inventory.offset < receivedInventory.offset && receivedInventory.items.Count > 0){
					List<PlayerItemData> itemsToBeAdded = new List<PlayerItemData>();

					for(int i = 0; i < receivedInventory.items.Count; i++){
						if(receivedInventory.logic.Equals("CLIENT")){
							PlayerItemData item = Inventory.items.FirstOrDefault(a => a.id == receivedInventory.items[i].id);
							if(item != null && receivedInventory.items[i].delta != 0){
								item.amount = item.amount + receivedInventory.items[i].delta;
							} else {
								itemsToBeAdded.Add(receivedInventory.items[i]);
							}

							updated = true;
						} else if(receivedInventory.logic.Equals("SERVER")){

						}

						updatedData.items.Add(receivedInventory.items[i]);
					}

					for(int i = 0; i < itemsToBeAdded.Count; i++){
						SpilItemData item = GetItemFromObjects(itemsToBeAdded[i].id);

						if(item != null && itemsToBeAdded[i].amount > 0){
							PlayerItemData playerItem = new PlayerItemData();
							playerItem.id = item.id;
							playerItem.name = item.name;
							playerItem.type = item.type;
							playerItem.amount = itemsToBeAdded[i].amount;
							playerItem.value = itemsToBeAdded[i].value;
							playerItem.delta = 0;

							Inventory.items.Add(playerItem);

							updated = true;
						}
					}
				}

				Inventory.offset = receivedInventory.offset;
				Inventory.logic = receivedInventory.logic;

			}

			if (updated) {
				updatedData.reason = PlayerDataUpdateReasons.ServerUpdate;

				SpilUnityImplementationBase.firePlayerDataUpdated (JsonHelper.getJSONFromObject (updatedData));
			}
		}

		public void WalletOperation (string action, int currencyId, int amount, string reason, string location, string transactionId = null)
		{

			if(currencyId <= 0 || reason == null){
				Debug.Log ("Error updating wallet!");
				return;
			}

			PlayerCurrencyData currency = null;

			for (int i = 0; i < Wallet.currencies.Count; i++) {
				if (Wallet.currencies [i].id == currencyId) {
					currency = Wallet.currencies [i];
				}
			}

			if (currency == null) {
				Debug.Log ("Currency does not exist!");
				return;
			}

			int currentBalance = currency.currentBalance;

			if (action.Equals ("subtract")) {
				amount = -amount;
			}

			int updatedBalance = currentBalance + amount;

			if (updatedBalance < 0) {
				Debug.Log ("Not enough balance for currency!");
				return;
			}

			int updatedDelta = amount + currency.delta;

			if (updatedDelta == 0) {
				updatedDelta = amount;
			}

			currency.delta = updatedDelta;
			currency.currentBalance = updatedBalance;

			if (Wallet.logic.Equals ("CLIENT")) {

				UpdateCurrency (currency);

				PlayerDataUpdatedData updatedData = new PlayerDataUpdatedData ();
				updatedData.currencies.Add (currency);
				updatedData.reason = reason;
				updatedData.location = location;

				SpilUnityImplementationBase.firePlayerDataUpdated (JsonHelper.getJSONFromObject (updatedData));

				SendUpdatePlayerDataEvent (reason, location, transactionId);

			} else if (Wallet.logic.Equals ("SERVER")) {

			}

		}

		private PlayerCurrencyData GetCurrencyFromWallet (int currencyId){
			for(int i = 0; i < Wallet.currencies.Count; i++){
				if(Wallet.currencies[i].id == currencyId){
					return Wallet.currencies[i];
				}
			}

			return null;
		}

		private void UpdateCurrency (PlayerCurrencyData currency)
		{
			for (int i = 0; i < Wallet.currencies.Count; i++) {
				if (Wallet.currencies [i].id == currency.id) {
					Wallet.currencies [i].currentBalance = currency.currentBalance;
					Wallet.currencies [i].delta = currency.delta;
				}
			}
		}

		public void InventoryOperation (string action, int itemId, int amount, string reason, string location, string transactionId = null)
		{
			SpilItemData gameItem = GetItemFromObjects(itemId);

			if(gameItem == null || itemId <= 0 || action == null || reason == null){
				Debug.Log("Error updating item to player inventory!");
				return;
			}

			PlayerItemData item = new PlayerItemData();
			item.id = gameItem.id;
			item.name = gameItem.name;
			item.type = gameItem.type;
			item.amount = amount;
			item.delta = amount;

			PlayerItemData inventoryItem = GetItemFromInventory(itemId);

			if(inventoryItem != null){
				int inventoryItemAmount = inventoryItem.amount;

				if(action.Equals("add")){
					inventoryItemAmount = inventoryItemAmount + amount;
				} else if(action.Equals("subtract")){
					inventoryItemAmount = inventoryItemAmount - amount;

					if(inventoryItemAmount < 0){
						Debug.Log("Could not remove item as amount is too low!");
						return;
					}
				}

				inventoryItem.delta = amount;
				inventoryItem.amount = inventoryItemAmount;
				UpdateItem(inventoryItem);
			} else {
				if(action.Equals("add")){
					Inventory.items.Add(item);
				} else if (action.Equals("subtract")){
					Debug.Log("Could not remove item as amount is too low!");
				}	
			}

			PlayerDataUpdatedData updatedData = new PlayerDataUpdatedData();
			updatedData.items.Add(item);
			updatedData.reason = reason;
			updatedData.location = location;

			SpilUnityImplementationBase.firePlayerDataUpdated (JsonHelper.getJSONFromObject (updatedData));

			SendUpdatePlayerDataEvent(item, reason, location, transactionId);


		}

		private SpilItemData GetItemFromObjects(int itemId){
			for(int i = 0; i < SpilUnityEditorImplementation.gData.items.Count; i++){
				if(SpilUnityEditorImplementation.gData.items[i].id == itemId){
					return SpilUnityEditorImplementation.gData.items[i];
				}
			}

			return null;
		}

		private PlayerItemData GetItemFromInventory(int itemId){
			for(int i = 0; i < Inventory.items.Count; i++){
				if(Inventory.items[i].id == itemId){
					return Inventory.items[i];
				}
			}

			return null;
		}

		private void UpdateItem(PlayerItemData item){
			for (int i = 0; i < Inventory.items.Count; i++) {
				if (Inventory.items[i].id == item.id) {
					Inventory.items[i].amount = item.amount;
					Inventory.items[i].delta = item.delta;
				}
			}
		}

		public void BuyBundle (int bundleId, string reason, string location, string transactionId)
		{
			PlayerDataUpdatedData updatedData = new PlayerDataUpdatedData();

			SpilBundleData bundle = GetBundleFromObjects(bundleId);

			if(bundle == null || reason == null){
				Debug.Log("Error adding bundle to player inventory!");
				return;
			}

			SpilShopPromotionData promotion = GetPromotionFromObjects(bundleId);
			bool isPromotionValid = false;

			if(promotion != null){
				isPromotionValid = IsPromotionValid(promotion);
			}

			List<SpilBundlePriceData> bundlePrices;

			if(isPromotionValid){
				bundlePrices = promotion.prices;
			} else {
				bundlePrices = bundle.prices;
			}

			for(int i = 0; i < bundlePrices.Count; i++){
				PlayerCurrencyData currency = GetCurrencyFromWallet(bundlePrices[i].currencyId);

				if(currency == null){
					Debug.Log("Currency does not exist!");
					return;
				}

				int currentBalance = currency.currentBalance;
				int updatedBalance = currentBalance - bundlePrices[i].value;

				if(updatedBalance < 0){
					Debug.Log("Not enough balance for currency!");
					return;
				}

				int updatedDelta = - bundlePrices[i].value + currency.delta;

				if(updatedDelta == 0){
					updatedDelta = - bundlePrices[i].value;
				}

				currency.delta = updatedDelta;
				currency.currentBalance = updatedBalance;

				UpdateCurrency(currency);
				updatedData.currencies.Add(currency);
			}

			for(int i = 0; i < bundle.items.Count; i++){
				SpilItemData gameItem = GetItemFromObjects(bundle.items[i].id);

				if(gameItem != null){
					PlayerItemData item = new PlayerItemData();
					item.id = gameItem.id;
					item.name = gameItem.name;
					item.type = gameItem.type;

					PlayerItemData inventoryItem = GetItemFromInventory(bundle.items[i].id);

					int inventoryItemAmount;

					if(inventoryItem != null){
						inventoryItemAmount = inventoryItem.amount;

						int promoAmount = 1;

						if(isPromotionValid){
							promoAmount = promotion.amount;
						}

						inventoryItemAmount = inventoryItemAmount + bundle.items[i].amount * promoAmount;

						inventoryItem.delta = bundle.items[i].amount;
						inventoryItem.amount = inventoryItemAmount;

						UpdateItem(inventoryItem);

						updatedData.items.Add(inventoryItem);
					} else {
						inventoryItemAmount = bundle.items[i].amount;

						if(isPromotionValid){
							inventoryItemAmount = inventoryItemAmount * promotion.amount;
						}

						item.delta = inventoryItemAmount;
						item.amount = inventoryItemAmount;

						Inventory.items.Add(item);

						updatedData.items.Add(inventoryItem);
					}
				} 
			}

			updatedData.reason = reason;
			updatedData.location = location;

			SpilUnityImplementationBase.firePlayerDataUpdated (JsonHelper.getJSONFromObject (updatedData));

			SendUpdatePlayerDataEvent(bundle, reason, location, transactionId);
		}

		private SpilBundleData GetBundleFromObjects(int bundleId){
			for(int i = 0; i < SpilUnityEditorImplementation.gData.bundles.Count; i++){
				if(SpilUnityEditorImplementation.gData.bundles[i].id == bundleId){
					return SpilUnityEditorImplementation.gData.bundles[i];
				}
			}

			return null;
		}

		private SpilShopPromotionData GetPromotionFromObjects(int bundleId){
			for(int i = 0; i < SpilUnityEditorImplementation.gData.promotions.Count; i++){
				if(SpilUnityEditorImplementation.gData.promotions[i].bundleId == bundleId){
					return SpilUnityEditorImplementation.gData.promotions[i];
				}
			}

			return null;
		}

		private bool IsPromotionValid(SpilShopPromotionData promotion){

			if(DateTime.Now > promotion.startDate && DateTime.Now < promotion.endDate){
				return true;
			}

			return false;
		}

		private void SendUpdatePlayerDataEvent (string reason, string location, string transationId){
			SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent> ();
			spilEvent.eventName = "updatePlayerData";

			JSONObject walletJSON = new JSONObject ();
			List<PlayerCurrencyData> list = new List<PlayerCurrencyData> ();

			for (int i = 0; i < Wallet.currencies.Count; i++) {
				if (Wallet.currencies [i].delta != 0) {
					list.Add (Wallet.currencies [i]);
				}
			}

			JSONObject currenciesJSON = new JSONObject (JSONObject.Type.ARRAY);

			for (int i = 0; i < list.Count; i++) {
				JSONObject obj = new JSONObject ();
				obj.AddField ("id", list [i].id);
				obj.AddField ("currentBalance", list [i].currentBalance);
				obj.AddField ("delta", list [i].delta);

				currenciesJSON.Add (obj);
			}

			walletJSON.AddField ("currencies", currenciesJSON);
			walletJSON.AddField ("offset", Wallet.offset);

			spilEvent.customData.AddField ("wallet", walletJSON);
			spilEvent.customData.AddField ("reason", reason);

			if(location != null){
				spilEvent.customData.AddField ("location", location);
			}

			if(transationId != null){
				spilEvent.customData.AddField ("transactionId", transationId);
			}

			spilEvent.Send ();
		}

		private void SendUpdatePlayerDataEvent (PlayerItemData item, string reason, string location, string transationId)
		{
			SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent> ();
			spilEvent.eventName = "updatePlayerData";

			JSONObject walletJSON = new JSONObject();
			walletJSON.AddField("offset", Wallet.offset);

			spilEvent.customData.AddField("wallet", walletJSON);

			JSONObject inventoryJSON = new JSONObject();

			JSONObject itemsJSON = new JSONObject(JsonHelper.getJSONFromObject(Inventory.items));

			inventoryJSON.AddField("items", itemsJSON);
			inventoryJSON.AddField("offset", Inventory.offset);

			spilEvent.customData.AddField("inventory", inventoryJSON);

			spilEvent.customData.AddField("item", new JSONObject(JsonHelper.getJSONFromObject(item)));

			spilEvent.customData.AddField("reason", reason);

			if(location != null){
				spilEvent.customData.AddField ("location", location);
			}

			if(transationId != null){
				spilEvent.customData.AddField ("transactionId", transationId);
			}

			spilEvent.Send();
		}

		private void SendUpdatePlayerDataEvent (SpilBundleData bundle, string reason, string location, string transationId)
		{
			SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent> ();
			spilEvent.eventName = "updatePlayerData";

			JSONObject walletJSON = new JSONObject();

			JSONObject currenciesJSON = new JSONObject(JsonHelper.getJSONFromObject(Wallet.currencies));

			walletJSON.AddField("currencies", currenciesJSON);
			walletJSON.AddField("offset", Wallet.offset);

			spilEvent.customData.AddField("wallet", walletJSON);

			JSONObject inventoryJSON = new JSONObject();

			JSONObject itemsJSON = new JSONObject(JsonHelper.getJSONFromObject(Inventory.items));

			inventoryJSON.AddField("items", itemsJSON);
			inventoryJSON.AddField("offset", Inventory.offset);

			spilEvent.customData.AddField("inventory", inventoryJSON);

			spilEvent.customData.AddField("bundle", new JSONObject(JsonHelper.getJSONFromObject(bundle)));

			spilEvent.customData.AddField("reason", reason);

			if(location != null){
				spilEvent.customData.AddField ("location", location);
			}

			if(transationId != null){
				spilEvent.customData.AddField ("transactionId", transationId);
			}

			spilEvent.Send();
		}

		public void SendUpdatePlayerDataEvent(bool wallet, bool inventory, string reason){
			SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent> ();
			spilEvent.eventName = "updatePlayerData";

			if(wallet){
				JSONObject walletJSON = new JSONObject();

				JSONObject currenciesJSON = new JSONObject(JsonHelper.getJSONFromObject(SpilUnityEditorImplementation.pData.Wallet.currencies));

				walletJSON.AddField("currencies", currenciesJSON);
				walletJSON.AddField("offset", Wallet.offset);

				spilEvent.customData.AddField("wallet", walletJSON);
			}

			if(inventory){
				JSONObject inventoryJSON = new JSONObject();

				JSONObject itemsJSON = new JSONObject(JsonHelper.getJSONFromObject(SpilUnityEditorImplementation.pData.Inventory.items));

				inventoryJSON.AddField("items", itemsJSON);
				inventoryJSON.AddField("offset", Inventory.offset);

				spilEvent.customData.AddField("inventory", inventoryJSON);
			}

			spilEvent.customData.AddField("reason", reason);

			spilEvent.Send();
		}

		public class TempUserInfo
		{
			public WalletData wallet;
			public InventoryData inventory;
		}


	}
}

#endif



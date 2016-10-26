﻿#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using SpilGames.Unity.Implementations;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;
using SpilGames.Unity.Utils.UnityEditor;
using System.Collections.Generic;
using System;

namespace SpilGames.Unity.Utils.UnityEditor.Responses
{
	public class PlayerData : Data
	{
		public WalletData Wallet;
		public InventoryData Inventory;

		public void ProcessPlayerData (ResponseEvent response)
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
						item.amount = (int)itemsJSON.list [i].GetField ("ammount").n;
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
				string playerData = System.IO.File.ReadAllText (Application.streamingAssetsPath + "/defaultPlayerData.json");
				TempUserInfo temp = JsonHelper.getObjectFromJson<TempUserInfo> (playerData);

				return temp.wallet;
			}

			return Wallet;
		}

		public InventoryData InitInventory ()
		{

			if (Inventory == null) {
				string playerData = System.IO.File.ReadAllText (Application.streamingAssetsPath + "/defaultPlayerData.json");
				TempUserInfo temp = JsonHelper.getObjectFromJson<TempUserInfo> (playerData);
				return temp.inventory;
			}

			return Inventory;
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
						for (int j = 0; j < Wallet.currencies.Count; j++) {
							if (receivedWallet.logic.Equals ("CLIENT")) {
								if (Wallet.currencies [j].id == receivedWallet.currencies [i].id && receivedWallet.currencies [i].delta != 0) {
									int updatedBalance = 0;

									if (Wallet.offset == 0 && receivedWallet.offset != 0) {
										updatedBalance = receivedWallet.currencies [i].currentBalance;
									} else {
										updatedBalance = Wallet.currencies [j].currentBalance + receivedWallet.currencies [i].delta;

										if (updatedBalance < 0) {
											updatedBalance = 0;
										}
									}

									Wallet.currencies [j].currentBalance = updatedBalance;

									updated = true;
									updatedData.currencies.Add (Wallet.currencies [j]);
								}
							} else if (receivedWallet.logic.Equals ("SERVER")) {
								
							}
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
						for(int j = 0; j < Inventory.items.Count; j++){
							if(receivedInventory.logic.Equals("CLIENT")){
								if(Inventory.items[j].id == receivedInventory.items[i].id && receivedInventory.items[i].delta != 0){
									int updatedAmount = Inventory.items[j].amount + receivedInventory.items[i].delta;

									Inventory.items[j].amount = updatedAmount;
								} else {
									itemsToBeAdded.Add(receivedInventory.items[i]);
								}

								updated = true;
							} else if(receivedInventory.logic.Equals("SERVER")){

							}
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

				SpilUnityImplementationBase.firePlayerDataUpdated (JsonUtility.ToJson (updatedData));
			}

		}

		public void WalletOperation (string action, int currencyId, int amount, string reason)
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

				SpilUnityImplementationBase.firePlayerDataUpdated (JsonUtility.ToJson (updatedData));

				SendUpdatePlayerDataEvent (reason);

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

		public void InventoryOperation (string action, int itemId, int amount, string reason)
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

			SpilUnityImplementationBase.firePlayerDataUpdated (JsonUtility.ToJson (updatedData));

			SendUpdatePlayerDataEvent(item, reason);


		}

		private SpilItemData GetItemFromObjects(int itemId){
			Debug.Log(itemId);
			Debug.Log(JsonUtility.ToJson(SpilUnityEditorImplementation.gData.items));
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

		public void ConsumeBundle (int bundleId, string reason)
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

			SpilUnityImplementationBase.firePlayerDataUpdated (JsonUtility.ToJson (updatedData));

			SendUpdatePlayerDataEvent(bundle, reason);
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

		private void SendUpdatePlayerDataEvent (string reason)
		{
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

			spilEvent.Send ();
		}

		private void SendUpdatePlayerDataEvent (PlayerItemData item, string reason)
		{
			
		}

		private void SendUpdatePlayerDataEvent (SpilBundleData bundle, string reason)
		{
			
		}

		public class TempUserInfo
		{
			public WalletData wallet;
			public InventoryData inventory;
		}


	}
}

#endif



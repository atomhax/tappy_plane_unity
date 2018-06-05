#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.GameData;
using SpilGames.Unity.Helpers.PlayerData;
using SpilGames.Unity.Helpers.Promotions;
using SpilGames.Unity.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpilGames.Unity.Base.UnityEditor.Managers {
    public class PlayerDataManager {
        public WalletData Wallet;
        public InventoryData Inventory;

        public WalletData InitWallet() {
            if (Wallet != null) return Wallet;
            TempUserInfo temp;
            try {
                string playerData =
                    File.ReadAllText(Application.streamingAssetsPath + "/defaultPlayerData.json");
                temp = JsonHelper.getObjectFromJson<TempUserInfo>(playerData);
            }
            catch (FileNotFoundException e) {
                SpilLogging.Log("defaultPlayerData.json not found. Creating a placeholder!" + e);
                string placeholder =
                    "{\"wallet\":{\"currencies\":[],\"offset\": 0,\"logic\": \"CLIENT\"},\"inventory\":{\"items\":[],\"offset\":0,\"logic\": \"\"}}";
                temp = JsonHelper.getObjectFromJson<TempUserInfo>(placeholder);
            }

            // Currencies and items loaded from playerdata don't have fields like initialValue because those are defined in the gamedata. Add the missing information.
            // TODO: Should playerdata ever be used to access these fields or only gamedata? A. Check where these fields are being used (they shouldnt be? see SendUpdatePlayerDataEvent tho) and B. Consider maybe this is the wrong inheritance structure / we're exposing fields that shouldn't be exposed for playerdata items/currencies?
            // TODO: Make this prettier? Load both defaultPlayerData and defaultGameData.json and create/initialise wallet+inventory by combining data (instead of deserialising playerdata and adding missing data from gamedata afterwards)?
            if (temp != null && temp.wallet != null && temp.wallet.currencies != null) {

                if (SpilUnityEditorImplementation.gData == null) {
                    throw new NotImplementedException("GameData must be initialised before calling this method.");
                }
                
                foreach(PlayerCurrencyData currency in temp.wallet.currencies) {
                    SpilCurrencyData gameDataCurrency = SpilUnityEditorImplementation.gData.currencies.FirstOrDefault(a => a.id == currency.id);
                    if (gameDataCurrency != null) {
                        currency.displayDescription = gameDataCurrency.displayDescription;
                        currency.displayName = gameDataCurrency.displayName;
                        currency.imageUrl = gameDataCurrency.imageUrl;
                        currency.initialValue = gameDataCurrency.initialValue;
                        currency.name = gameDataCurrency.name;
                        currency.type = gameDataCurrency.type;
                    } else {
                        // TODO: Playerdata contains a currency that is not defined in the gamedata, should this throw an exception?
                        // TODO: Remove the currency from the list or keep it with missing data (as it is now)?
                    }
                }
            }

            return temp.wallet;
        }

        public void SetInitalWalletValues() {
            string init = PlayerPrefs.GetString("walletInit-" + Spil.SpilUserIdEditor, "false");

            if (Wallet != null && init.Equals("false")) {
                foreach (SpilCurrencyData currency in SpilUnityEditorImplementation.gData.currencies) {
                    if (currency.initialValue > 0) {
                        WalletOperation("add", currency.id, currency.initialValue, PlayerDataUpdateReasons.InitialValue,
                            null, "sdk", null);
                    }
                }
            }

            PlayerPrefs.SetString("walletInit-" + Spil.SpilUserIdEditor, "true");
        }

        public void ResetWallet() {

            // For currencies already in the player wallet reset the amount to the initialValue.

            foreach (PlayerCurrencyData playerCurrency in SpilUnityEditorImplementation.pData.Wallet.currencies) {
                int initialValue = SpilUnityEditorImplementation.gData.currencies.FirstOrDefault(a => a.id == playerCurrency.id).initialValue;
                int newDelta = initialValue - playerCurrency.currentBalance;

                playerCurrency.currentBalance = initialValue;
                playerCurrency.delta = newDelta + playerCurrency.delta;
            }

            // Check the currencies in the game data, if a currency is not currently in the wallet but has an initialvalue > 0 then add it.

            foreach (SpilCurrencyData gameDataCurrency in SpilUnityEditorImplementation.gData.currencies) {
                if (gameDataCurrency.initialValue > 0 && !SpilUnityEditorImplementation.pData.Wallet.currencies.Any(a => a.id == gameDataCurrency.id)) {
                    PlayerCurrencyData playerCurrency = new PlayerCurrencyData(gameDataCurrency);

                    playerCurrency.currentBalance = gameDataCurrency.initialValue;
                    playerCurrency.delta = gameDataCurrency.initialValue;

                    SpilUnityEditorImplementation.pData.Wallet.currencies.Add(playerCurrency);
                }
            }
        }

        public InventoryData InitInventory() {
            if (Inventory != null) return Inventory;
            TempUserInfo temp;
            try {
                string playerData =
                    File.ReadAllText(Application.streamingAssetsPath + "/defaultPlayerData.json");
                temp = JsonHelper.getObjectFromJson<TempUserInfo>(playerData);
            }
            catch (FileNotFoundException e) {
                SpilLogging.Log("defaultPlayerData.json not found. Creating a placeholder! " + e);
                string placeholder =
                    "{\"wallet\":{\"currencies\":[],\"offset\": 0,\"logic\": \"CLIENT\"},\"inventory\":{\"items\":[],\"offset\":0,\"logic\": \"\"}}";
                temp = JsonHelper.getObjectFromJson<TempUserInfo>(placeholder);
            }

            // Currencies and items loaded from playerdata don't have fields like initialValue because those are defined in the gamedata. Add the missing information.
            // TODO: Should playerdata ever be used to access these fields or only gamedata? A. Check where these fields are being used (they shouldnt be? see SendUpdatePlayerDataEvent tho) and B. Consider maybe this is the wrong inheritance structure / we're exposing fields that shouldn't be exposed for playerdata items/currencies?
            // TODO: Make this prettier? Load both defaultPlayerData and defaultGameData.json and create/initialise wallet+inventory by combining data (instead of deserialising playerdata and adding missing data from gamedata afterwards)?
            if (temp != null && temp.inventory != null && temp.inventory.items != null) {

                if (SpilUnityEditorImplementation.gData == null) {
                    throw new NotImplementedException("GameData must be initialised before calling this method.");
                }

                foreach (PlayerItemData item in temp.inventory.items) {
                    SpilItemData gameDataItem = SpilUnityEditorImplementation.gData.items.FirstOrDefault(a => a.id == item.id);
                    if (gameDataItem != null) {
                        item.content = gameDataItem.content;
                        item.displayDescription = gameDataItem.displayDescription;
                        item.displayName = gameDataItem.displayName;
                        item.imageUrl = gameDataItem.imageUrl;
                        item.initialValue = gameDataItem.initialValue;
                        item.isGacha = gameDataItem.isGacha;
                        item.name = gameDataItem.name;
                        item.type = gameDataItem.type;
                    } else {
                        // TODO: Playerdata contains an item that is not defined in the gamedata, should this throw an exception?
                        // TODO: Remove the item from the list or keep it with missing data (as it is now)?
                    }
                }
            }

            return temp.inventory;
        }

        public void SetInitalInventoryValues() {
            string init = PlayerPrefs.GetString("inventoryInit-" + Spil.SpilUserIdEditor, "false");

            if (Inventory != null && init.Equals("false")) {
                foreach (SpilItemData item in SpilUnityEditorImplementation.gData.items) {
                    if (item.initialValue > 0) {
                        InventoryOperation("add", item.id, item.initialValue, PlayerDataUpdateReasons.InitialValue,
                            null, "sdk", null);
                    }
                }
            }

            PlayerPrefs.SetString("inventoryInit-" + Spil.SpilUserIdEditor, "true");
        }

        public void ResetInventory() {

            // For items already in the player inventory reset the amount to the initialValue.

            foreach (PlayerItemData playerItem in SpilUnityEditorImplementation.pData.Inventory.items) {
                int initialValue = SpilUnityEditorImplementation.gData.items.FirstOrDefault(a => a.id == playerItem.id).initialValue;
                int newDelta = initialValue - playerItem.amount;

                playerItem.amount = initialValue;
                playerItem.delta = newDelta + playerItem.delta;
            }

            // Check the items in the game data, if an item is not currently in the inventory but has an initialvalue > 0 then add it.

            foreach (SpilItemData gameDataItem in SpilUnityEditorImplementation.gData.items) {
                if(gameDataItem.initialValue > 0 && !SpilUnityEditorImplementation.pData.Inventory.items.Any(a => a.id == gameDataItem.id)) {
                    PlayerItemData playerItem = new PlayerItemData(gameDataItem);

                    playerItem.amount = gameDataItem.initialValue;
                    playerItem.delta = gameDataItem.initialValue;

                    SpilUnityEditorImplementation.pData.Inventory.items.Add(playerItem);
                }
            }
        }

        public void ResetPlayerData() {
            ResetWallet();
            ResetInventory();
        }

        public void CalculatePlayerDataResponse(WalletData receivedWallet, InventoryData receivedInventory, bool fromInit) {
            bool updated = false;
            PlayerDataUpdatedData updatedData = new PlayerDataUpdatedData();

            if (receivedWallet != null) {
                foreach (PlayerCurrencyData playerCurrency in Wallet.currencies) {
                    playerCurrency.delta = 0;
                }

                if (Wallet.offset < receivedWallet.offset && receivedWallet.currencies.Count > 0) {
                    foreach (PlayerCurrencyData playerCurrency in receivedWallet.currencies) {
                        if (receivedWallet.logic.Equals("CLIENT")) {
                            PlayerCurrencyData currency =
                                Wallet.currencies.FirstOrDefault(a => a.id == playerCurrency.id);

                            if (currency == null) continue;
                            if (Wallet.offset == 0 && receivedWallet.offset != 0) {
                                currency.currentBalance = playerCurrency.currentBalance;
                            }
                            else {
                                if (playerCurrency.delta != 0) {
                                    int updatedBalance =
                                        currency.currentBalance + playerCurrency.delta;

                                    if (updatedBalance < 0) {
                                        updatedBalance = 0;
                                    }

                                    currency.currentBalance = updatedBalance;
                                }
                            }

                            updated = true;
                            updatedData.currencies.Add(currency);
                        }
                        else if (receivedWallet.logic.Equals("SERVER")) {
                        }
                    }
                }

                Wallet.offset = receivedWallet.offset;
                Wallet.logic = receivedWallet.logic;
            }

            if (receivedInventory != null) {
                foreach (PlayerItemData playerItem in Inventory.items) {
                    playerItem.delta = 0;
                }

                if (Inventory.offset < receivedInventory.offset && receivedInventory.items.Count > 0) {
                    List<PlayerItemData> itemsToBeAdded = new List<PlayerItemData>();

                    foreach (PlayerItemData playerItem in receivedInventory.items) {
                        if (receivedInventory.logic.Equals("CLIENT")) {
                            PlayerItemData item = Inventory.items.FirstOrDefault(a => a.id == playerItem.id);
                            if (item != null && playerItem.delta != 0) {
                                item.amount = item.amount + playerItem.delta;
                            }
                            else {
                                itemsToBeAdded.Add(playerItem);
                            }

                            updated = true;
                        }
                        else if (receivedInventory.logic.Equals("SERVER")) {
                        }

                        updatedData.items.Add(playerItem);
                    }

                    foreach (PlayerItemData itemToAdd in itemsToBeAdded) {
                        SpilItemData item = GetItemFromObjects(itemToAdd.id);

                        if (item != null && itemToAdd.amount > 0) {
                            PlayerItemData playerItem = new PlayerItemData();
                            playerItem.id = item.id;
                            playerItem.name = item.name;
                            playerItem.type = item.type;
                            playerItem.amount = itemToAdd.amount;
                            playerItem.value = itemToAdd.value;
                            playerItem.delta = 0;

                            playerItem.displayName = item.displayName;
                            playerItem.displayDescription = item.displayDescription;
                            playerItem.isGacha = item.isGacha;
                            playerItem.content = item.content;

                            Inventory.items.Add(playerItem);

                            updated = true;
                        }
                    }
                }

                Inventory.offset = receivedInventory.offset;
                Inventory.logic = receivedInventory.logic;
            }

            if (updated) {
                UserDataManager.UpdateUserDataVersions();
                updatedData.reason = PlayerDataUpdateReasons.ServerUpdate;

				Spil.Instance.firePlayerDataUpdated(JsonHelper.getJSONFromObject(updatedData));
            }

            if (!fromInit) {
				Spil.Instance.fireUserDataAvailable();
            }
        }

        public void WalletOperation(string action, int currencyId, int amount, string reason, string reasonDetails, string location, string transactionId) {
            if (currencyId <= 0 || reason == null) {
                SpilLogging.Error("Error updating wallet!");
                return;
            }

            PlayerCurrencyData currency = null;

            foreach (PlayerCurrencyData playerCurrency in Wallet.currencies) {
                if (playerCurrency.id == currencyId) {
                    currency = playerCurrency;
                }
            }

            if (currency == null) {
                SpilLogging.Error("Currency does not exist!");
                return;
            }

            int currentBalance = currency.currentBalance;

            if (action.Equals("subtract")) {
                amount = -amount;
            }

            int updatedBalance = currentBalance + amount;

            if (updatedBalance < 0) {
                SpilLogging.Error("Not enough balance for currency!");
                return;
            }

            int updatedDelta = amount + currency.delta;

            if (updatedDelta == 0) {
                updatedDelta = amount;
            }

            currency.delta = updatedDelta;
            currency.currentBalance = updatedBalance;

            if (Wallet.logic.Equals("CLIENT")) {
                UpdateCurrency(currency);

                UserDataManager.UpdateUserDataVersions();
                UserDataManager.UpdateUserDataMeta();
                
                PlayerDataUpdatedData updatedData = new PlayerDataUpdatedData();
                updatedData.currencies.Add(currency);
                updatedData.reason = reason;

				Spil.Instance.firePlayerDataUpdated(JsonHelper.getJSONFromObject(updatedData));

                SendUpdatePlayerDataEvent(null, reason, reasonDetails, location, transactionId);
                
                UpdateTieredEvent(currency.id, amount, "CURRENCY");
            }
            else if (Wallet.logic.Equals("SERVER")) {
            }
        }

        private PlayerCurrencyData GetCurrencyFromWallet(int currencyId) {
            foreach (PlayerCurrencyData playerCurrency in Wallet.currencies) {
                if (playerCurrency.id == currencyId) {
                    return playerCurrency;
                }
            }

            return null;
        }

        private void UpdateCurrency(PlayerCurrencyData currency) {
            foreach (PlayerCurrencyData playerCurrency in Wallet.currencies) {
                if (playerCurrency.id != currency.id) continue;
                playerCurrency.currentBalance = currency.currentBalance;
                playerCurrency.delta = currency.delta;
            }
        }

        public void InventoryOperation(string action, int itemId, int amount, string reason, string reasonDetails, string location, string transactionId) {
            SpilItemData gameItem = GetItemFromObjects(itemId);

            if (gameItem == null || itemId <= 0 || action == null || reason == null) {
                SpilLogging.Error("Error updating item to player inventory!");
                return;
            }

            PlayerItemData item = new PlayerItemData();
            item.id = gameItem.id;
            item.name = gameItem.name;
            item.type = gameItem.type;
            item.displayName = gameItem.displayName;
            item.displayDescription = gameItem.displayDescription;
            item.isGacha = gameItem.isGacha;
            item.content = gameItem.content;
            item.amount = amount;
            item.delta = amount;

            PlayerItemData inventoryItem = GetItemFromInventory(itemId);

            if (inventoryItem != null) {
                int inventoryItemAmount = inventoryItem.amount;

                if (action.Equals("add")) {
                    inventoryItemAmount = inventoryItemAmount + amount;
                }
                else if (action.Equals("subtract")) {
                    inventoryItemAmount = inventoryItemAmount - amount;

                    if (inventoryItemAmount < 0) {
                        SpilLogging.Error("Could not remove item as amount is too low!");
                        return;
                    }
                }

                inventoryItem.delta = amount;
                inventoryItem.amount = inventoryItemAmount;
                UpdateItem(inventoryItem);
            }
            else {
                if (action.Equals("add")) {
                    Inventory.items.Add(item);
                }
                else if (action.Equals("subtract")) {
                    SpilLogging.Error("Could not remove item as amount is too low!");
                }
            }

            UserDataManager.UpdateUserDataVersions();
            UserDataManager.UpdateUserDataMeta();
            
            PlayerDataUpdatedData updatedData = new PlayerDataUpdatedData();
            updatedData.items.Add(item);
            updatedData.reason = reason;

			Spil.Instance.firePlayerDataUpdated(JsonHelper.getJSONFromObject(updatedData));

            SendUpdatePlayerDataEvent(null, reason, reasonDetails, location, transactionId);
            
            UpdateTieredEvent(item.id, amount, item.isGacha ? "GACHA" : "ITEM");
        }

        private SpilItemData GetItemFromObjects(int itemId) {
            foreach (SpilItemData item in SpilUnityEditorImplementation.gData.items) {
                if (item.id == itemId) {
                    return item;
                }
            }

            return null;
        }

        private PlayerItemData GetItemFromInventory(int itemId) {
            foreach (PlayerItemData item in Inventory.items) {
                if (item.id == itemId) {
                    return item;
                }
            }

            return null;
        }

        private SpilItemData GetGachaFromObjects(int gachaId) {
            foreach (SpilItemData item in SpilUnityEditorImplementation.gData.items) {
                if (item.id == gachaId && item.isGacha) {
                    return item;
                }
            }

            return null;
        }

        private PlayerItemData GetGachaFromInventory(int gachaId) {
            foreach (PlayerItemData item in Inventory.items) {
                if (item.id == gachaId && item.isGacha) {
                    return item;
                }
            }

            return null;
        }

        private void UpdateItem(PlayerItemData item) {
            foreach (PlayerItemData playerItem in Inventory.items) {
                if (playerItem.id != item.id) continue;
                playerItem.amount = item.amount;
                playerItem.delta = item.delta;
            }
        }

        public void BuyBundle(int bundleId, string reason, string reasonDetails, string location, string transactionId) {
            PlayerDataUpdatedData updatedData = new PlayerDataUpdatedData();

            SpilBundleData bundle = GetBundleFromObjects(bundleId);

            if (bundle == null || reason == null) {
                SpilLogging.Error("Error adding bundle to player inventory!");
                return;
            }

            Promotion promotion = Spil.Instance.GetPromotions().GetBundlePromotion(bundleId);
            bool isPromotionValid = false;

            if (promotion != null) {
                isPromotionValid = promotion.IsValid();
            }

            List<SpilBundlePriceData> bundlePrices = new List<SpilBundlePriceData>();
            
            if (isPromotionValid) {
                foreach (PriceOverride priceOverride in promotion.PriceOverride) {
                    SpilBundlePriceData bundlePriceData = new SpilBundlePriceData();
                    bundlePriceData.currencyId = priceOverride.Id;
                    bundlePriceData.value = priceOverride.Amount;
                    
                    bundlePrices.Add(bundlePriceData);
                }
            } 

            if (bundlePrices.Count == 0) {
                bundlePrices = bundle.prices;
            }

            bundle.prices = bundlePrices;
            
            foreach (SpilBundlePriceData bundlePrice in bundlePrices) {
                PlayerCurrencyData currency = GetCurrencyFromWallet(bundlePrice.currencyId);

                if (currency == null) {
                    SpilLogging.Error("Currency does not exist!");
                    return;
                }

                int currentBalance = currency.currentBalance;
                int updatedBalance = currentBalance - bundlePrice.value;

                if (updatedBalance < 0) {
                    SpilLogging.Error("Not enough balance for currency!");
                    return;
                }

                int updatedDelta = -bundlePrice.value + currency.delta;

                if (updatedDelta == 0) {
                    updatedDelta = -bundlePrice.value;
                }

                currency.delta = updatedDelta;
                currency.currentBalance = updatedBalance;

                UpdateCurrency(currency);
                updatedData.currencies.Add(currency);
            }

            foreach (SpilBundleItemData bundleItem in bundle.items) {
                if (bundleItem.type.Equals("CURRENCY")) {
                    PlayerCurrencyData currency = GetCurrencyFromWallet(bundleItem.id);
                        
                    if (currency == null) {
                        SpilLogging.Error("Currency does not exist!");
                        return;
                    }

                    currency.currentBalance = currency.currentBalance + bundleItem.amount;
                    currency.delta = currency.delta + bundleItem.amount;

                    UpdateCurrency(currency);
                } else {
                    SpilItemData gameItem = GetItemFromObjects(bundleItem.id);

                    if (gameItem == null) {
                        SpilLogging.Error("Item does not exist!");
                        return;
                    };
                    PlayerItemData item = new PlayerItemData();
                    item.id = gameItem.id;
                    item.name = gameItem.name;
                    item.type = gameItem.type;
                    item.displayName = gameItem.displayName;
                    item.displayDescription = gameItem.displayDescription;
                    item.isGacha = gameItem.isGacha;
                    item.content = gameItem.content;

                    PlayerItemData inventoryItem = GetItemFromInventory(bundleItem.id);

                    int inventoryItemAmount;

                    if (inventoryItem != null) {
                        inventoryItemAmount = inventoryItem.amount;

                        inventoryItemAmount = inventoryItemAmount + bundleItem.amount;

                        inventoryItem.delta = bundleItem.amount;
                        inventoryItem.amount = inventoryItemAmount;

                        UpdateItem(inventoryItem);

                        updatedData.items.Add(inventoryItem);
                    }
                    else {
                        inventoryItemAmount = bundleItem.amount;

                        item.delta = inventoryItemAmount;
                        item.amount = inventoryItemAmount;

                        Inventory.items.Add(item);

                        updatedData.items.Add(item);
                    } 
                }
            }

            if (isPromotionValid) {
                foreach (ExtraEntity extraEntity in promotion.ExtraEntities) {
                    if (extraEntity.Type.Equals("CURRENCY")) {
                        PlayerCurrencyData currency = GetCurrencyFromWallet(extraEntity.Id);
                        
                        if (currency == null) {
                            SpilLogging.Error("Currency does not exist!");
                            return;
                        }

                        currency.currentBalance = currency.currentBalance + extraEntity.Amount;
                        currency.delta = currency.delta + extraEntity.Amount;

                        UpdateCurrency(currency);

                        PlayerCurrencyData temp = null;
                        
                        foreach (PlayerCurrencyData playerCurrency in updatedData.currencies) {
                            if (playerCurrency.id == extraEntity.Id) {
                                temp = playerCurrency;
                            }
                        }

                        if (temp != null) {
                            updatedData.currencies.Remove(temp);
                        }
                        
                        updatedData.currencies.Add(currency);
                    } else if (extraEntity.Type.Equals("ITEM") || extraEntity.Type.Equals("GACHA")) {
                        SpilItemData gameItem = GetItemFromObjects(extraEntity.Id);

                        if (gameItem == null) {
                            SpilLogging.Error("Item does not exist!");
                            return;
                        };
                        PlayerItemData item = new PlayerItemData();
                        item.id = gameItem.id;
                        item.name = gameItem.name;
                        item.type = gameItem.type;
                        item.displayName = gameItem.displayName;
                        item.displayDescription = gameItem.displayDescription;
                        item.isGacha = gameItem.isGacha;
                        item.content = gameItem.content;

                        PlayerItemData inventoryItem = GetItemFromInventory(extraEntity.Id);

                        int inventoryItemAmount;

                        if (inventoryItem != null) {
                            inventoryItemAmount = inventoryItem.amount;

                            inventoryItemAmount = inventoryItemAmount + extraEntity.Amount;

                            inventoryItem.delta = extraEntity.Amount;
                            inventoryItem.amount = inventoryItemAmount;

                            UpdateItem(inventoryItem);

                            PlayerItemData temp = null;
                        
                            foreach (PlayerItemData playerItem in updatedData.items) {
                                if (playerItem.id == extraEntity.Id) {
                                    temp = playerItem;
                                }
                            }

                            if (temp != null) {
                                updatedData.items.Remove(temp);
                            }
                            
                            updatedData.items.Add(inventoryItem);
                        }
                        else {
                            inventoryItemAmount = extraEntity.Amount;

                            item.delta = inventoryItemAmount;
                            item.amount = inventoryItemAmount;

                            Inventory.items.Add(item);

                            updatedData.items.Add(item);
                        }
                    }
                }
            }
            
            UserDataManager.UpdateUserDataVersions();
            UserDataManager.UpdateUserDataMeta();
            
            updatedData.reason = reason;

			Spil.Instance.firePlayerDataUpdated(JsonHelper.getJSONFromObject(updatedData));

            if (isPromotionValid) {
                PromotionsManager.PromotionData.First(a => a.id == promotion.Id).amountPurchased++;
                
                PromotionsManager.SendBoughtPromotion(promotion.Id);
            }
            
            SendUpdatePlayerDataEvent(bundle, reason, reasonDetails, location, transactionId);

            foreach (SpilBundlePriceData bundlePrice in bundlePrices) {
                UpdateTieredEvent(bundlePrice.currencyId, -bundlePrice.value, "CURRENCY");
            }
        }

        private void OpenBundle(int bundleId, int amount, string reason, string reasonDetails, string location) {
        }

        private SpilBundleData GetBundleFromObjects(int bundleId) {
            foreach (SpilBundleData bundleData in SpilUnityEditorImplementation.gData.bundles) {
                if (bundleData.id == bundleId) {
                    return bundleData;
                }
            }

            return null;
        }

        public void OpenGacha(int gachaId, string reason, string reasonDetails, string location) {
            PlayerItemData gachaPlayerItem = GetGachaFromInventory(gachaId);
            SpilItemData gachaItem = GetGachaFromObjects(gachaId);

            if (gachaPlayerItem == null || gachaItem == null || gachaId <= 0 || reason == null || !gachaPlayerItem.isGacha) {
                SpilLogging.Error("Error opening gacha!");
                return;
            }

            if (!gachaPlayerItem.content.All(gachaItem.content.Contains) && gachaPlayerItem.content.Count == gachaItem.content.Count) {
                gachaPlayerItem.content = gachaItem.content;
            }

            if (gachaPlayerItem.amount < 1) {
                SpilLogging.Error("Not enough gacha boxes in the inventory!");
                return;
            }

            if (gachaPlayerItem.content.Count < 1) {
                SpilLogging.Error("Error opening gacha! No content present!");
                return;
            }

            int weightSum = 0;

            foreach (SpilGachaContent gachaContent in gachaPlayerItem.content) {
                weightSum = weightSum + gachaContent.weight;
            }

            if (weightSum == 0) {
                SpilLogging.Error("Error opening gacha!");
                return;
            }

            int rand = Random.Range(0, weightSum);

            int low = 0;
            int high = 0;

            for (int i = 0; i < gachaPlayerItem.content.Count; i++) {
                SpilGachaContent gachaContent = gachaPlayerItem.content[i];

                if (i != 0) {
                    low = high;
                }

                high = low + gachaContent.weight;

                if (rand >= low && rand < high) {
                    gachaPlayerItem.amount = gachaPlayerItem.amount - 1;
                    gachaPlayerItem.delta = gachaPlayerItem.delta - 1;
                    
                    UpdateItem(gachaPlayerItem);

                    switch (gachaContent.type) {
                        case "CURRENCY":
                            WalletOperation("add", gachaContent.id, gachaContent.amount, reason, reasonDetails, location, null);
                            break;
                        case "ITEM":
                            InventoryOperation("add", gachaContent.id, gachaContent.amount, reason, reasonDetails, location, null);
                            break;
                        case "BUNDLE":
                            OpenBundle(gachaContent.id, gachaContent.amount, reason, reasonDetails, location);
                            break;
                        case "GACHA":
                            InventoryOperation("add", gachaContent.id, gachaContent.amount, reason, reasonDetails, location, null);
                            break;
                        case "NONE":
                            UserDataManager.UpdateUserDataVersions();
                            UserDataManager.UpdateUserDataMeta();
                            
                            PlayerDataUpdatedData updatedData = new PlayerDataUpdatedData();
                            updatedData.items.Add(gachaPlayerItem);
                            updatedData.reason = reason;

							Spil.Instance.firePlayerDataEmptyGacha();
							Spil.Instance.firePlayerDataUpdated(JsonHelper.getJSONFromObject(updatedData));
                            break;
                        default:
                            SpilLogging.Error("Error opening gacha!");
                            return;
                    }

                    break;
                }
            }
        }

        public void SendUpdatePlayerDataEvent(SpilBundleData bundle, string reason, string reasonDetails, string location, string transactionId) {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "updatePlayerData";

            JSONObject walletObject = new JSONObject();
            List<PlayerCurrencyData> currencyList = new List<PlayerCurrencyData>();

            foreach (PlayerCurrencyData currencyData in Wallet.currencies) {
                if (currencyData.delta != 0) {
                    currencyList.Add(currencyData);
                }
            }

            if (currencyList.Count > 0) {
                JSONObject currenciesJSON = new JSONObject();
                foreach (PlayerCurrencyData currencyData in currencyList) {
                    JSONObject obj = new JSONObject();
                    obj.AddField("id", currencyData.id);
                    obj.AddField("currentBalance", currencyData.currentBalance);
                    obj.AddField("delta", currencyData.delta);

                    currenciesJSON.Add(obj);
                }

                walletObject.AddField("currencies", currenciesJSON);

                walletObject.AddField("offset", Wallet.offset);

                spilEvent.customData.AddField("wallet", walletObject);

                foreach (PlayerCurrencyData currencyData in Wallet.currencies) {
                    currencyData.delta = 0;
                    UpdateCurrency(currencyData);
                }
            }

            JSONObject inventoryObject = new JSONObject();
            List<PlayerItemData> itemsList = new List<PlayerItemData>();

            foreach (PlayerItemData playerItemData in Inventory.items) {
                if (playerItemData.delta != 0) {
                    itemsList.Add(playerItemData);
                }
            }

            if (itemsList.Count > 0) {
                JSONObject itemsJSON = new JSONObject(JSONObject.Type.ARRAY);

                foreach (PlayerItemData playerItemData in itemsList) {
                    JSONObject obj = new JSONObject();
                    obj.AddField("id", playerItemData.id);
                    obj.AddField("amount", playerItemData.amount);
                    obj.AddField("delta", playerItemData.delta);

                    itemsJSON.Add(obj);
                }

                inventoryObject.AddField("items", itemsJSON);

                inventoryObject.AddField("offset", Inventory.offset);

                spilEvent.customData.AddField("inventory", inventoryObject);

                foreach (PlayerItemData playerItemData in Inventory.items) {
                    playerItemData.delta = 0;
                    UpdateItem(playerItemData);
                }
            }

            if (bundle != null) {
                spilEvent.customData.AddField("bundle", new JSONObject(JsonHelper.getJSONFromObject(bundle)));
            }

            spilEvent.customData.AddField("reason", reason);

            if (reasonDetails != null) {
                spilEvent.customData.AddField("reasonDetails", reasonDetails);
            }

            if (location != null) {
                spilEvent.customData.AddField("location", location);
            }

            if (transactionId != null) {
                spilEvent.customData.AddField("transactionId", transactionId);
            }
            
            spilEvent.customData.AddField("deviceVersions", UserDataManager.GenerateUserDataVersionsJSON(UserDataManager.userDataVersions));

            spilEvent.Send();
        }

        public void SendUpdatePlayerDataEvent() {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "updatePlayerData";

            JSONObject walletObject = new JSONObject();
            walletObject.AddField("offset", Wallet.offset);

            spilEvent.customData.AddField("wallet", walletObject);

            JSONObject inventoryObject = new JSONObject();
            inventoryObject.AddField("offset", Inventory.offset);

            spilEvent.customData.AddField("inventory", inventoryObject);
            
            spilEvent.customData.AddField("deviceVersions", UserDataManager.GenerateUserDataVersionsJSON(UserDataManager.userDataVersions));

            spilEvent.Send();
        }

        public void SendUpdatePlayerDataEvent(bool resetWallet, bool resetInventory, string reason) {
            if (resetWallet) {
                foreach (PlayerCurrencyData currency in Wallet.currencies) {
                    int newDelta = currency.initialValue - currency.currentBalance;

                    currency.currentBalance = currency.initialValue;
                    currency.delta = newDelta + currency.delta;

                    UpdateCurrency(currency);
                }
            }

            if (resetInventory) {
                foreach (PlayerItemData item in Inventory.items) {
                    int newDelta = item.initialValue - item.amount;

                    item.amount = item.initialValue;
                    item.delta = newDelta + item.delta;

                    UpdateItem(item);
                }
            }

            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "updatePlayerData";

            JSONObject walletObject = new JSONObject();

            JSONObject currenciesJSON = new JSONObject(JSONObject.Type.ARRAY);

            foreach (PlayerCurrencyData currencyData in Wallet.currencies) {
                JSONObject obj = new JSONObject();
                obj.AddField("id", currencyData.id);
                obj.AddField("currentBalance", currencyData.currentBalance);
                obj.AddField("delta", currencyData.delta);

                currenciesJSON.Add(obj);
            }

            walletObject.AddField("currencies", currenciesJSON);
            walletObject.AddField("offset", Wallet.offset);

            spilEvent.customData.AddField("wallet", walletObject);

            JSONObject inventoryObject = new JSONObject();

            JSONObject itemsJSON = new JSONObject(JSONObject.Type.ARRAY);

            foreach (PlayerItemData playerItemData in Inventory.items) {
                JSONObject obj = new JSONObject();
                obj.AddField("id", playerItemData.id);
                obj.AddField("amount", playerItemData.amount);
                obj.AddField("delta", playerItemData.delta);

                itemsJSON.Add(obj);
            }

            inventoryObject.AddField("items", itemsJSON);
            inventoryObject.AddField("offset", Inventory.offset);

            spilEvent.customData.AddField("inventory", inventoryObject);

            spilEvent.customData.AddField("reason", reason);
            
            spilEvent.customData.AddField("deviceVersions", UserDataManager.GenerateUserDataVersionsJSON(UserDataManager.userDataVersions));

            spilEvent.Send();
        }

        public class TempUserInfo {
            public WalletData wallet;
            public InventoryData inventory;
        }
        
        private static void UpdateTieredEvent(int entityId, int amount, String entityType) {
            TieredEvent selectedTieredEvent = null;
    
            if(TieredEventManager.tieredEventsOverview.tieredEvents.Count == 0) {
                return;
            }

            foreach (TieredEvent tieredEvent in TieredEventManager.tieredEventsOverview.tieredEvents.Values) {
                if (!(tieredEvent.endDate > System.DateTime.Now.Millisecond)) {
                    continue;
                }

                foreach (TieredEventTier tier in tieredEvent.tiers) {
                    if (tier.entityId == entityId && tier.entityType.Equals(entityType)) {
                        selectedTieredEvent = tieredEvent;
                        break;
                    }
                }
            }
    
            if (selectedTieredEvent == null) {
                return;
            }
    
            if ((selectedTieredEvent.type.Equals("spend") && amount > 0)) {
                SpilLogging.Log("Entity operation not meeting Tiered Event requirements. Tiered Event progress will not be updated.");
                return;
            }
    
            TieredEventProgress tieredProgress = TieredEventManager.tieredEventsOverview.progress[selectedTieredEvent.id];

            if (tieredProgress == null || tieredProgress.completed) {
                return;
            }
            
            TieredEventTier currentTier = null;

            foreach (TieredEventTier tier in selectedTieredEvent.tiers) {
                if (tier.id == tieredProgress.currentTierId) {
                    currentTier = tier;
                    break;
                }
            }
    
            if (currentTier == null) {
                return;
            }
    
            if (amount < 0) {
                amount = -amount;
            }
    
            TieredEventManager.UpdateTierProgress(selectedTieredEvent.id, currentTier.id, entityId, entityType, amount);
        }
    }

    public class PlayerDataResponse : Response {
        public static void ProcessPlayerDataResponse(ResponseEvent response) {
            WalletData receivedWallet = null;
            InventoryData receivedInventory = null;

            if (response.data.HasField("wallet")) {
                JSONObject walletJSON = response.data.GetField("wallet");

                receivedWallet = new WalletData();

                if (walletJSON.HasField("currencies")) {
                    receivedWallet.currencies = new List<PlayerCurrencyData>();

                    JSONObject currenciesJSON = walletJSON.GetField("currencies");

                    for (int i = 0; i < currenciesJSON.Count; i++) {
                        PlayerCurrencyData currency = new PlayerCurrencyData();

                        currency.id = (int) currenciesJSON.list[i].GetField("id").n;
                        currency.currentBalance = (int) currenciesJSON.list[i].GetField("currentBalance").n;
                        currency.delta = (int) currenciesJSON.list[i].GetField("delta").n;

                        receivedWallet.currencies.Add(currency);
                    }
                }

                receivedWallet.offset = (long) walletJSON.GetField("offset").n;
                receivedWallet.logic = walletJSON.GetField("logic").str;
            }

            if (response.data.HasField("inventory")) {
                JSONObject inventoryJSON = response.data.GetField("inventory");

                receivedInventory = new InventoryData();

                if (inventoryJSON.HasField("items")) {
                    receivedInventory.items = new List<PlayerItemData>();

                    JSONObject itemsJSON = inventoryJSON.GetField("items");

                    for (int i = 0; i < itemsJSON.Count; i++) {
                        PlayerItemData item = new PlayerItemData();

                        item.id = (int) itemsJSON.list[i].GetField("id").n;
                        item.amount = (int) itemsJSON.list[i].GetField("amount").n;
                        item.delta = (int) itemsJSON.list[i].GetField("delta").n;

                        receivedInventory.items.Add(item);
                    }
                }

                receivedInventory.offset = (long) inventoryJSON.GetField("offset").n;
                receivedInventory.logic = inventoryJSON.GetField("logic").str;
            }

            if (response.action.ToLower().Trim().Equals("update")) {
                SpilUnityEditorImplementation.pData.CalculatePlayerDataResponse(receivedWallet, receivedInventory, false);
            } else if (response.action.ToLower().Trim().Equals("syncerror")) {
                UserDataManager.ProcessSyncError();
            } else if (response.action.ToLower().Trim().Equals("dropped")) {
                UserDataManager.ProcessDroppedResponse("Event dropped!");
            } else if (response.action.ToLower().Trim().Equals("lockerror")) {
                UserDataManager.ProcessLockError();
            }
            
        }
    }
}

#endif
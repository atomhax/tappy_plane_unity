using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpilGames.Unity.Helpers.PlayerData;
using SpilGames.Unity.Helpers.PlayerData.Perk;

namespace SpilGames.Unity.Base.SDK {
    public class PlayerCurrencyData : SpilCurrencyData {
        public int currentBalance;
        public int delta;
        public int overflow;

        public PlayerCurrencyData() {
        }

        public PlayerCurrencyData(SpilCurrencyData spilCurrencyData) {
            id = spilCurrencyData.id;
            name = spilCurrencyData.name;
            initialValue = spilCurrencyData.initialValue;
            type = spilCurrencyData.type;
            imageUrl = spilCurrencyData.imageUrl;
            displayName = spilCurrencyData.displayName;
            displayDescription = spilCurrencyData.displayDescription;
            limit = spilCurrencyData.limit;
        }
    }

    public class WalletData {
        public List<PlayerCurrencyData> currencies;
        public long offset;
        public string logic;
    }

    public class PlayerItemData : SpilItemData {
        public int amount;
        public int delta;
        public int value;
        public int overflow;

        public PlayerItemData() {
        }

        public PlayerItemData(SpilItemData spilItemData) {
            id = spilItemData.id;
            name = spilItemData.name;
            initialValue = spilItemData.initialValue;
            type = spilItemData.type;
            imageUrl = spilItemData.imageUrl;
            displayName = spilItemData.displayName;
            displayDescription = spilItemData.displayDescription;
            isGacha = spilItemData.isGacha;
            content = spilItemData.content;
            properties = spilItemData.properties;
            limit = spilItemData.limit;
            isUnique = spilItemData.isUnique;
        }
    }

    public class UniquePlayerItemData : SpilItemData {
        public String uniqueId;
        public int amount;
        public int delta;
        public String status;
        public Dictionary<string, object> uniqueProperties = new Dictionary<string, object>();

        public UniquePlayerItemData() {
        }

        public UniquePlayerItemData(SpilItemData spilItemData) {
            id = spilItemData.id;
            name = spilItemData.name;
            initialValue = spilItemData.initialValue;
            type = spilItemData.type;
            imageUrl = spilItemData.imageUrl;
            displayName = spilItemData.displayName;
            displayDescription = spilItemData.displayDescription;
            isGacha = spilItemData.isGacha;
            content = spilItemData.content;
            properties = spilItemData.properties;
            limit = spilItemData.limit;
            isUnique = spilItemData.isUnique;

            status = "NONE";
        }
    }

    public class InventoryData {
        public List<PlayerItemData> items = new List<PlayerItemData>();
        public List<UniquePlayerItemData> uniqueItems = new List<UniquePlayerItemData>();
        public long offset;
        public string logic;
    }

    public class PlayerDataUpdatedData {
        public string reason;
        public int bundleId;
        public int gachaId;
        public List<PerkItem> perkItems;
        public List<PlayerItemData> items = new List<PlayerItemData>();
        public List<UniquePlayerItemData> uniqueItems = new List<UniquePlayerItemData>();
        public List<PlayerCurrencyData> currencies = new List<PlayerCurrencyData>();
    }

    public class PlayerDataNewUniqueItemInfo {
        public UniquePlayerItem uniquePlayerItem;
        public int bundleId;
        public int gachaId;
        public int tierId;
        public string reason;
    }
}
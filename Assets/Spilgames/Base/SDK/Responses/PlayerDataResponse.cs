using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        }
    }

    public class InventoryData {
        public List<PlayerItemData> items;
        public long offset;
        public string logic;
    }

    public class PlayerDataUpdatedData {
        public string reason;
        public List<PlayerItemData> items = new List<PlayerItemData>();
        public List<PlayerCurrencyData> currencies = new List<PlayerCurrencyData>();
    }
}
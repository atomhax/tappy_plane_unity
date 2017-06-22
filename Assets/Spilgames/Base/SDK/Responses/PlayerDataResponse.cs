using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SpilGames.Unity.Base.SDK {
    public class PlayerCurrencyData : SpilCurrencyData {
        public int currentBalance;
        public int delta;
    }

    public class WalletData {
        public List<PlayerCurrencyData> currencies;
        public int offset;
        public string logic;
    }

    public class PlayerItemData : SpilItemData {
        public int amount;
        public int delta;
        public int value;
    }

    public class InventoryData {
        public List<PlayerItemData> items;
        public int offset;
        public string logic;
    }

    public class PlayerDataUpdatedData {
        public string location;
        public string reason;
        public List<PlayerItemData> items = new List<PlayerItemData>();
        public List<PlayerCurrencyData> currencies = new List<PlayerCurrencyData>();
    }
}
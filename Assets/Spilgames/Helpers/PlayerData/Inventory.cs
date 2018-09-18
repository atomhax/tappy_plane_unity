using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.GameData;

namespace SpilGames.Unity.Helpers.PlayerData {
    /// <summary>
    /// This is the business object that the developer can use to work with the Inventory.
    /// </summary>
    public class Inventory {
        public List<PlayerItem> Items {
            get { return items; }
        }

        private List<PlayerItem> items;

        public Inventory(List<PlayerItemData> itemData) {
            items = new List<PlayerItem>();

            //Adding items of the player
            if (itemData != null) {
                foreach (PlayerItemData playerItemData in itemData) {
                    items.Add(new PlayerItem(playerItemData.id, playerItemData.name, playerItemData.type, playerItemData.amount, playerItemData.delta, playerItemData.value, playerItemData.imageUrl, playerItemData.reportingName, playerItemData.displayName, playerItemData.displayDescription, playerItemData.isGacha, playerItemData.content, playerItemData.properties, playerItemData.limit, playerItemData.overflow));
                }
            }
        }

        public void Add(int itemId, int amount, string reason, string location, string reasonDetails = null, string transactionId = null) {
            Spil.Instance.AddItemToInventory(itemId, amount, reason, location, reasonDetails, transactionId);
        }

        public void Subtract(int itemId, int amount, string reason, string location, string reasonDetails = null, string transactionId = null) {
            Spil.Instance.SubtractItemFromInventory(itemId, amount, reason, location, reasonDetails, transactionId);
        }
    }

    /// <summary>
    /// This is the business object that the developer can use to work with for the Player owned Item.
    /// </summary>
    public class PlayerItem : Item {
        public int Amount {
            get { return amount; }
        }

        private int amount;

        public int Delta {
            get { return delta; }
        }

        private int delta;
        
        public int Value {
            get { return value; }
        }

        private int value;

        public int Overflow {
            get { return overflow; }
        }

        private int overflow;
        
        public PlayerItem(int id, string name, int type, int amount, int delta, int value, string imageURL, string reportingName, string displayName, string displayDescription, bool isGacha, List<SpilGachaContent> content, Dictionary<string, object> properties, int limit, int overflow) : base(id, name, type, imageURL, reportingName, displayName, displayDescription, isGacha, content, properties, limit) {
            this.amount = amount;
            this.delta = delta;
            this.value = value;
            this.amount = amount;
            this.overflow = overflow;
        }
    }
}
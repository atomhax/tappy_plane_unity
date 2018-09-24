using System;
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
        
        public List<UniquePlayerItem> UniqueItems {
            get { return uniqueItems; }
        }

        private List<UniquePlayerItem> uniqueItems;
        
        public Inventory(List<PlayerItemData> itemData, List<UniquePlayerItemData> uniqueItemData) {
            items = new List<PlayerItem>();

            //Adding items of the player
            if (itemData != null) {
                foreach (PlayerItemData playerItemData in itemData) {
                    items.Add(new PlayerItem(playerItemData.id, playerItemData.name, playerItemData.type, playerItemData.amount, playerItemData.delta, playerItemData.value, playerItemData.imageUrl, playerItemData.reportingName, playerItemData.displayName, playerItemData.displayDescription, playerItemData.isGacha, playerItemData.content, playerItemData.properties, playerItemData.limit, playerItemData.isUnique, playerItemData.overflow));
                }
            }
            
            uniqueItems = new List<UniquePlayerItem>();
            
            if (uniqueItemData != null) {
                foreach (UniquePlayerItemData uniquePlayerItemData in uniqueItemData) {
                    uniqueItems.Add(new UniquePlayerItem(uniquePlayerItemData.id, uniquePlayerItemData.name, uniquePlayerItemData.type, uniquePlayerItemData.amount, uniquePlayerItemData.delta, uniquePlayerItemData.imageUrl, uniquePlayerItemData.reportingName, uniquePlayerItemData.displayName, uniquePlayerItemData.displayDescription, uniquePlayerItemData.isGacha, uniquePlayerItemData.content, uniquePlayerItemData.properties, uniquePlayerItemData.limit, uniquePlayerItemData.isUnique, uniquePlayerItemData.uniqueId, uniquePlayerItemData.status, uniquePlayerItemData.uniqueProperties));
                }
            }
        }

        public void Add(int itemId, int amount, string reason, string location, string reasonDetails = null, string transactionId = null) {
            Spil.Instance.AddItemToInventory(itemId, amount, reason, location, reasonDetails, transactionId);
        }

        public void Subtract(int itemId, int amount, string reason, string location, string reasonDetails = null, string transactionId = null) {
            Spil.Instance.SubtractItemFromInventory(itemId, amount, reason, location, reasonDetails, transactionId);
        }

        public UniquePlayerItem CreateUniquePlayerItem(int itemId, string uniqueId = null) {
            return Spil.Instance.CreateUniquePlayerItem(itemId, uniqueId);
        }

        public void AddUniquePlayerItemToInventory(UniquePlayerItem uniquePlayerItem, string reason, string location, string reasonDetails = null, string transactionId = null) {
            Spil.Instance.AddUniquePlayerItemToInventory(uniquePlayerItem, reason, location, reasonDetails, transactionId);
        }
        
        public void UpdateUniquePlayerItemFromInventory(UniquePlayerItem uniquePlayerItem, string reason, string location, string reasonDetails = null, string transactionId = null) {
            Spil.Instance.UpdateUniquePlayerItemFromInventory(uniquePlayerItem, reason, location, reasonDetails, transactionId);
        }
        
        public void RemoveUniquePlayerItemFromInventory(UniquePlayerItem uniquePlayerItem, string reason, string location, string reasonDetails = null, string transactionId = null) {
            Spil.Instance.RemoveUniquePlayerItemFromInventory(uniquePlayerItem, reason, location, reasonDetails, transactionId);
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
        
        public PlayerItem(int id, string name, int type, int amount, int delta, int value, string imageURL, string reportingName, string displayName, string displayDescription, bool isGacha, List<SpilGachaContent> content, Dictionary<string, object> properties, int limit, bool isUnique, int overflow) : base(id, name, type, imageURL, reportingName, displayName, displayDescription, isGacha, content, properties, limit, isUnique) {
            this.amount = amount;
            this.delta = delta;
            this.value = value;
            this.amount = amount;
            this.overflow = overflow;
        }
    }

    /// <summary>
    /// This is the business object that the developer can use to work with for the Player owned UniqueItem.
    /// </summary>
    public class UniquePlayerItem : Item {
        public String UniqueId {
            get { return uniqueId; }
            set { uniqueId = value; }
        }

        private String uniqueId;
        
        public int Amount {
            get { return amount; }
            set { amount = value; }
        }

        private int amount;
        
        public int Delta {
            get { return delta; }
            set { delta = value; }
        }

        private int delta;
        
        public String Status {
            get { return status; }
            set { status = value; }
        }

        private String status;
        
        public Dictionary<string, object> UniqueProperties {
            get { return uniqueProperties; }
            set { uniqueProperties = value; }
        }

        private Dictionary<string, object> uniqueProperties;

        public UniquePlayerItem(int id, string name, int type, int amount, int delta, string imageUrl, string reportingName, string displayName, string displayDescription, bool isGacha, List<SpilGachaContent> content, Dictionary<string, object> properties, int limit, bool isUnique, string uniqueId, string status, Dictionary<string, object> uniqueProperties) : base(id, name, type, imageUrl, reportingName, displayName, displayDescription, isGacha, content, properties, limit, isUnique) {
            this.uniqueId = uniqueId;
            this.amount = amount;
            this.delta = delta;
            this.status = status;
            this.uniqueProperties = uniqueProperties;
        }
    }
}
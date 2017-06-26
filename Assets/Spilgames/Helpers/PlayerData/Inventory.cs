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

            //Adding currencies of the player
            if (itemData != null) {
                foreach (PlayerItemData playerItemData in itemData) {
                    items.Add(new PlayerItem(playerItemData.id, playerItemData.name, playerItemData.type, playerItemData.amount, playerItemData.value, playerItemData.imageUrl, playerItemData.displayName, playerItemData.displayDescription));
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

        public int Value {
            get { return value; }
        }

        private int value;

        public PlayerItem(int id, string name, int type, int amount, int value, string imageURL, string displayName, string displayDescription) : base(id, name, type, imageURL, displayName, displayDescription) {
            this.amount = amount;
            this.value = value;
        }
    }
}
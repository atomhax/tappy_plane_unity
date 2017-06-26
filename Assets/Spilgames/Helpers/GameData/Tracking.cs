using UnityEngine;
using System.Collections;

namespace SpilGames.Unity.Helpers.GameData {
    /// <summary>
    /// Helper object for currencies used for the TrackWalletInventoryEvent
    /// </summary>
    public class TrackingCurrency {
        /// <summary>
        /// The name of the currency.
        /// </summary>
        public string name;

        /// <summary>
        /// The current balance of the currency
        /// </summary>
        public int currentBalance;

        /// <summary>
        /// The value with which the currency balance has changed
        /// </summary>
        public int delta;

        /// <summary>
        /// The type of currency. Should be 0 - 99
        /// </summary>
        public int type;
    }

    /// <summary>
    /// Helper object for items used for the TrackWalletInventoryEvent
    /// </summary>
    public class TrackingItem {
        /// <summary>
        /// The name of the item.
        /// </summary>
        public string name;

        /// <summary>
        /// The current amount of the item
        /// </summary>
        public int amount;

        /// <summary>
        /// The value with which the item amount has changed
        /// </summary>
        public int delta;

        /// <summary>
        /// The type of item. Should be -1 - +99
        /// </summary>
        public int type;
    }
}
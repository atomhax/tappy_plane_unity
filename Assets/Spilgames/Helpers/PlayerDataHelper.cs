using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SpilGames.Unity.Utils;
using SpilGames.Unity.Helpers;
using SpilGames.Unity.Implementations;

namespace SpilGames.Unity.Helpers
{
	/// <summary>
	/// This is the business object that the developer can use to work with Player Data (Wallet and Inventory).
	/// Much easier to work with than the raw data!
	/// </summary>
	public class PlayerDataHelper
	{
		public Wallet Wallet;
		public Inventory Inventory;

		public PlayerDataHelper (SpilUnityImplementationBase Instance)
		{
			string walletString = Instance.GetWalletFromSdk ();
			string inventoryString = Instance.GetInvetoryFromSdk ();
			if (walletString != null && inventoryString != null) {
				WalletData walletData = JsonHelper.getObjectFromJson<WalletData> (walletString);
				InventoryData inventoryData = JsonHelper.getObjectFromJson<InventoryData> (inventoryString);

				AddDataToHelper (walletData != null ? walletData.currencies : null, inventoryData != null ? inventoryData.items : null);
			}
		}

		/// <summary>
		/// Helper method that returns true if an item is in the inventory
		/// </summary>
		public Boolean InventoryHasItem (int itemId)
		{
			if (Inventory != null) {
				foreach (PlayerItem item in Inventory.Items) {
					if (item.Id == itemId) {
						return true;
					}
				}
				return false;
			} else {
				return false;
			}
		}

		/// <summary>
		/// Helper method that returns the Currency Balance given a currencyId
		/// Returns -1 if no currency was found
		/// </summary>
		public int GetCurrencyBalance (int currencyId)
		{
			if (Wallet != null) {
				foreach (PlayerCurrency currency in Wallet.Currencies) {
					if (currency.Id == currencyId) {
						return currency.CurrentBalance;
					}
				}
				return -1;
			} else {
				return -1;
			}
		}

		/// <summary>
		/// Helper method that returns the Item Amount given a itemId
		/// Returns -1 if no item was found
		/// </summary>
		public int GetItemAmount (int itemId)
		{
			if (Inventory != null) {
				foreach (PlayerItem item in Inventory.Items) {
					if (item.Id == itemId) {
						return item.Amount;
					}
				}
				return -1;
			} else {
				return -1;
			}
		}

		/// <summary>
		/// Helper method that consumes the bundle given a bundleId and a reason
		/// </summary>
		public void BuyBundle (int bundleId, string reason, string location, string transactionId = null)
		{
			Spil.Instance.BuyBundle (bundleId, reason, location, transactionId);
		}

		private void AddDataToHelper (List<PlayerCurrencyData> walletCurrencies, List<PlayerItemData> inventoryItems)
		{
			Wallet = new Wallet (walletCurrencies);
			Inventory = new Inventory (inventoryItems);
		}

		public void PlayerDataUpdatedHandler ()
		{
			UpdatePlayerData ();
		}

		private void UpdatePlayerData ()
		{
			string walletString = Spil.Instance.GetWalletFromSdk ();
			string inventoryString = Spil.Instance.GetInvetoryFromSdk ();
			if (walletString != null && inventoryString != null) {
				WalletData walletData = JsonHelper.getObjectFromJson<WalletData> (walletString);
				InventoryData inventoryData = JsonHelper.getObjectFromJson<InventoryData> (inventoryString);

				AddDataToHelper (walletData != null ? walletData.currencies : null, inventoryData != null ? inventoryData.items : null);
			}
		}
	}


	/// <summary>
	/// This is the business object that the developer can use to work with for the Player owned Currency.
	/// </summary>
	public class PlayerCurrency : Currency
	{
		public int CurrentBalance { get { return currentBalance; } }

		private int currentBalance;

		public int Delta { get { return delta; } }

		private int delta;

		public PlayerCurrency (int id, string name, int type, int currentBalance, int delta) : base (id, name, type)
		{
			this.currentBalance = currentBalance;
			this.delta = delta;
		}
	}

	/// <summary>
	/// This is the business object that the developer can use to work with the Wallet.
	/// </summary>
	public class Wallet
	{
		public List<PlayerCurrency> Currencies { get { return currencies; } }

		public List<PlayerCurrency> currencies;

		public Wallet (List<PlayerCurrencyData> currencyData)
		{
			currencies = new List<PlayerCurrency> ();

			// Adding currencies of the player
			if (currencyData != null) {
				foreach (PlayerCurrencyData playerCurrencyData in currencyData) {
					currencies.Add (new PlayerCurrency (playerCurrencyData.id, playerCurrencyData.name, playerCurrencyData.type, playerCurrencyData.currentBalance, playerCurrencyData.delta));
				}
			}			
		}

		public void Add (int currencyId, int amount, string reason, string location, string transactionId = null)
		{
			Spil.Instance.AddCurrencyToWallet (currencyId, amount, reason, location, transactionId);
		}

		public void Subtract (int currencyId, int amount, string reason, string location, string transactionId = null)
		{
			Spil.Instance.SubtractCurrencyFromWallet (currencyId, amount, reason, location, transactionId);
		}
	}

	/// <summary>
	/// This is the business object that the developer can use to work with for the Player owned Item.
	/// </summary>
	public class PlayerItem : Item
	{
		public int Amount { get { return amount; } }

		private int amount;

		public int Value { get { return value; } }

		private int value;

		public PlayerItem (int id, string name, int type, int amount, int value, string imageURL) : base (id, name, type, imageURL)
		{
			this.amount = amount;
			this.value = value;
		}
	}

	/// <summary>
	/// This is the business object that the developer can use to work with the Inventory.
	/// </summary>
	public class Inventory
	{
		public List<PlayerItem> Items { get { return items; } }

		private List<PlayerItem> items;

		public Inventory (List<PlayerItemData> itemData)
		{
			items = new List<PlayerItem> ();

			//Adding currencies of the player
			if (itemData != null) {
				foreach (PlayerItemData playerItemData in itemData) {
					items.Add (new PlayerItem (playerItemData.id, playerItemData.name, playerItemData.type, playerItemData.amount, playerItemData.value, playerItemData.imageURL));
				}
			}
		}

		public void Add (int itemId, int amount, string reason, string location, string transactionId = null)
		{
			Spil.Instance.AddItemToInventory (itemId, amount, reason, location, transactionId);
		}

		public void Subtract (int itemId, int amount, string reason, string location, string transactionId = null)
		{
			Spil.Instance.SubtractItemFromInventory (itemId, amount, reason, location, transactionId);
		}
	}

	/// <summary>
	/// This is the business object that the developer can use to work with the reasons for the Player Update Events.
	/// </summary>
	public class PlayerDataUpdateReasons
	{
		public static string RewardAds = "Reward Ads";
		public static string ItemBought = "Item Bought";
		public static string ItemSold = "Item Sold";
		public static string EventReward = "Event Reward";
		public static string LoginReward = "Login Reward";
		public static string IAP = "IAP";
		public static string PlayerLevelUp = "Player Level Up";
		public static string LevelComplete = "Level Complete";
		public static string ItemUpgrade = "Item Upgrade";
		public static string BonusFeatures = "Bonus Features";
		public static string Trade = "Trade";
		public static string ClientServerMismatch = "Client-Server Mismatch";
		public static string ItemPickedUp = "Item Picked Up";
		public static string ServerUpdate = "Server Update";
		public static string DailyBonus = "Daily Bonus From Client";
		public static string Rush = "Rush";
		public static string Gift = "Gift";
		public static string Cancel = "Cancel";
		public static string Collection = "Collection";
		public static string Reset = "Reset";
    		public static string InitialValue = "Initial Value";

		public PlayerDataUpdateReasons ()
		{
		}
	}
}


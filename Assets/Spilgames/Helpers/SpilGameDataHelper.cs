using System;
using System.Collections.Generic;
using UnityEngine;
using SpilGames.Unity.Utils;
using SpilGames.Unity.Implementations;

namespace SpilGames.Unity.Helpers
{
	/// <summary>
	/// This is the business object that the developer can use to work with Spil Game Data (Currencies, Items, Bundles, Shop).
	/// Much easier to work with than the raw data!
	/// </summary>
	public class SpilGameDataHelper
	{
		public List<Currency> Currencies = new List<Currency> ();
		public List<Item> Items = new List<Item> ();
		public List<Bundle> Bundles = new List<Bundle> ();
		public Shop Shop;
		public List<Promotion> Promotions = new List<Promotion> ();

		public SpilGameDataHelper (SpilUnityImplementationBase Instance)
		{
			string spilGameDataString = Instance.GetSpilGameDataFromSdk ();
			if (spilGameDataString != null) {
				SpilGameData spilGameData = JsonHelper.getObjectFromJson<SpilGameData> (spilGameDataString);
				if (spilGameData != null) {
					AddDataToHelper (spilGameData.currencies, spilGameData.items, spilGameData.bundles, spilGameData.shop, spilGameData.promotions);
					Debug.Log ("Spil GameData created");
				} else {
					Debug.Log ("Spil GameData not created, Instance.GetSpilGameDataFromsdk() did not return any data.");
				}				
			}
		}

		public void RefreshData(SpilUnityImplementationBase Instance){
			string spilGameDataString = Instance.GetSpilGameDataFromSdk ();
			if (spilGameDataString != null) {
				SpilGameData spilGameData = JsonHelper.getObjectFromJson<SpilGameData> (spilGameDataString);
				if (spilGameData != null) {
					AddDataToHelper (spilGameData.currencies, spilGameData.items, spilGameData.bundles, spilGameData.shop, spilGameData.promotions);
					Debug.Log ("Spil GameData Refreshed");
				} else {
					Debug.Log ("Spil GameData not created, Instance.GetSpilGameDataFromsdk() did not return any data.");
				}				
			}
		}

		/// <summary>
		/// Helper method that returns the Bundle for a given bundleId
		/// Returns null if no bundle was found
		/// </summary>
		public Bundle GetBundle (int bundleId)
		{
			if (Bundles != null) {
				foreach (Bundle bundle in Bundles) {
					if (bundle.Id == bundleId) {
						return bundle;
					}
				}
				return null;
			} else {
				return null;
			}
		}

		/// <summary>
		/// Helper method that returns the Item for a given itemId
		/// Returns null if no item was found
		/// </summary>
		public Item GetItem (int itemId)
		{
			if (Items != null) {
				foreach (Item item in Items) {
					if (item.Id == itemId) {
						return item;
					}
				}
				return null;
			} else {
				return null;
			}
		}

		/// <summary>
		/// Helper method that returns the Currency for a given currencyId
		/// Returns null if no currency was found
		/// </summary>
		public Currency GetCurrency (int currencyId)
		{
			if (Currencies != null) {
				foreach (Currency currency in Currencies) {
					if (currency.Id == currencyId) {
						return currency;
					}
				}
				return null;
			} else {
				return null;
			}
		}

		private void AddDataToHelper (List<SpilCurrencyData> currencies, List<SpilItemData> items, List<SpilBundleData> bundles, List<SpilShopTabData> shop, List<SpilShopPromotionData> promotions)
		{
			Currencies.Clear ();

			if (currencies != null) {
				foreach (SpilCurrencyData spilCurrencyData in currencies) {
					Currencies.Add (new Currency (spilCurrencyData.id, spilCurrencyData.name, spilCurrencyData.type));
				}
			}

			Items.Clear ();

			if (items != null) {
				foreach (SpilItemData spilCurrencyItems in items) {
					Items.Add (new Item (spilCurrencyItems.id, spilCurrencyItems.name, spilCurrencyItems.type));
				}
			}

			Bundles.Clear ();

			if (bundles != null) {
				foreach (SpilBundleData spilBundleData in bundles) {
					Bundles.Add (new Bundle (spilBundleData.id, spilBundleData.name, spilBundleData.prices, spilBundleData.items));
				}
			}

			//Adding shop data to helper
			Shop = new Shop (shop);

			Promotions.Clear ();

			if (promotions != null) {
				foreach (SpilShopPromotionData promotion in promotions) {
					Promotions.Add (new Promotion (promotion.bundleId, promotion.amount, promotion.prices, promotion.discount, promotion.startDate, promotion.endDate));
				}
			}
		}

		public void SpilGameDataHandler ()
		{
			UpdateSpilGameData ();
		}

		private void UpdateSpilGameData ()
		{
			string spilGameDataString = Spil.Instance.GetSpilGameDataFromSdk ();
			if (spilGameDataString != null) {
				SpilGameData spilGameData = JsonHelper.getObjectFromJson<SpilGameData> (spilGameDataString);
				AddDataToHelper (spilGameData.currencies, spilGameData.items, spilGameData.bundles, spilGameData.shop, spilGameData.promotions);
				Debug.Log ("Spil Game Data Updated");
			}
		}
	}

	/// <summary>
	/// This is the business object that the developer can use to work with a Currency.
	/// </summary>
	public class Currency
	{
		/// <summary>
		/// The currency Id
		/// </summary>
		public int Id { get { return _Id; } }

		private int _Id;

		/// <summary>
		/// The currency Name
		/// </summary>
		public string Name { get { return _Name; } }

		private string _Name;

		/// <summary>
		/// The currency Type (Premium and Non-Premium)
		/// </summary>
		public int Type { get { return _Type; } }

		public int _Type;

		public Currency (int id, string name, int type)
		{
			_Id = id;
			_Name = name;
			_Type = type;
		}
	}

	/// <summary>
	/// This is the business object that the developer can use to work with an Item.
	/// </summary>
	public class Item
	{
		/// <summary>
		/// The item Id
		/// </summary>
		public int Id { get { return _Id; } }

		private int _Id;

		/// <summary>
		/// The item Name
		/// </summary>
		public string Name { get { return _Name; } }

		private string _Name;

		/// <summary>
		/// The item Type (Consumable, Booster and Permanent)
		/// </summary>
		public int Type { get { return _Type; } }

		public int _Type;

		public Item (int id, string name, int type)
		{
			_Id = id;
			_Name = name;
			_Type = type;
		}
	}

	/// <summary>
	/// This is the business object that the developer can use to work a Bundle.
	/// </summary>
	public class Bundle
	{
		/// <summary>
		/// The bundle Id
		/// </summary>
		public int Id { get { return _Id; } }

		private int _Id;

		/// <summary>
		/// The bundle Name
		/// </summary>
		public string Name { get { return _Name; } }

		private string _Name;

		/// <summary>
		/// The bundle Prices. Can have multiple prices with different currencies
		/// </summary>
		public List<BundlePrice> Prices { get { return _Prices; } }

		private List<BundlePrice> _Prices = new List<BundlePrice> ();

		/// <summary>
		/// The bundle Items that are contained within a Bundle
		/// </summary>
		public List<BundleItem> Items { get { return _Items; } }

		private List<BundleItem> _Items = new List<BundleItem> ();

		public Bundle (int id, string name, List<SpilBundlePriceData> prices, List<SpilBundleItemData> items)
		{
			_Id = id;
			_Name = name;
					
			//Adding Prices for Bundle
			if (prices != null) {
				foreach (SpilBundlePriceData bundlePriceData in prices) {
					_Prices.Add (new BundlePrice (bundlePriceData.currencyId, bundlePriceData.value));
				}
			}
			
			//Adding Items to Bundle
			if (items != null) {
				foreach (SpilBundleItemData bundleItemData in items) {	
					_Items.Add (new BundleItem (bundleItemData.id, bundleItemData.amount));
				}
			}
		}
	}

	/// <summary>
	/// This is the business object that the developer can use to work with for the Bundle Price.
	/// </summary>
	public class BundlePrice
	{
		/// <summary>
		/// The currency Id assoctiated with the Bundle Price
		/// </summary>
		public int CurrencyId { get { return _CurrencyId; } }

		private int _CurrencyId;

		/// <summary>
		/// The value required in order to consume the bundle for the assoctiated currency
		/// </summary>
		public int Value { get { return _Value; } }

		private int _Value;

		public BundlePrice (int currencyId, int value)
		{
			_CurrencyId = currencyId;
			_Value = value;
		}
	}

	/// <summary>
	/// This is the business object that the developer can use to work with for the Items contained in a Bundle.
	/// </summary>
	public class BundleItem
	{
		/// <summary>
		/// The item Id assoctiated with the Bundle Item
		/// </summary>
		public int Id { get { return _Id; } }

		private int _Id;

		/// <summary>
		/// The amount if items contained withing the Bundle Item
		/// </summary>
		public int Amount { get { return _Amount; } }

		private int _Amount;

		public BundleItem (int id, int amount)
		{
			_Id = id;
			_Amount = amount;
		}
	}

	/// <summary>
	/// This is the business object that the developer can use to work with for the InGame Shop.
	/// </summary>
	public class Shop
	{
		/// <summary>
		/// The Shop Tabs as defined in SLOT
		/// </summary>
		public List<Tab> Tabs { get { return _Tabs; } }

		private List<Tab> _Tabs = new List<Tab> ();

		public Shop (List<SpilShopTabData> shop)
		{
			if (shop != null) {
				foreach (SpilShopTabData tab in shop) {
					_Tabs.Add (new Tab (tab.name, tab.entries));
				}
			}
		}
	}

	/// <summary>
	/// This is the business object that the developer can use to work with for the Tab contained in the Shop.
	/// </summary>
	public class Tab
	{
		/// <summary>
		/// The tab Name
		/// </summary>
		public string Name { get { return _Name; } }

		private string _Name;

		/// <summary>
		/// The Shop Entries as defined in SLOT
		/// </summary>
		public List<Entry> Entries { get { return _Entries; } }

		private List<Entry> _Entries = new List<Entry> ();

		public Tab (string name, List<SpilShopEntryData> entries)
		{
			_Name = name;
			if (entries != null) {
				foreach (SpilShopEntryData entry in entries) {
					_Entries.Add (new Entry (entry.bundleId, entry.label, entry.position));
				}
			}
		}
	}

	/// <summary>
	/// This is the business object that the developer can use to work with for the Entry contained in the Tab.
	/// </summary>
	public class Entry
	{
		/// <summary>
		/// The bundle Id assoctiated with this shop entry
		/// </summary>
		public int BundleId { get { return _BundleId; } }

		private int _BundleId;

		/// <summary>
		/// The entry label
		/// </summary>
		public string Label { get { return _Label; } }

		private string _Label;

		/// <summary>
		/// The entry position as defined in SLOT
		/// </summary>
		public int Position { get { return _Position; } }

		private int _Position;

		public Entry (int bundleId, string label, int position)
		{
			_BundleId = bundleId;
			_Label = label;
			_Position = position;
		}
	}

	/// <summary>
	/// This is the business object that the developer can use to work with for the InGame Shop Promotion.
	/// </summary>
	public class Promotion
	{
		/// <summary>
		/// The bundle Id assoctiated with this shop promotion
		/// </summary>
		public int BundleId { get { return _BundleId; } }

		private int _BundleId;

		/// <summary>
		/// The amount of bundles for the shop promotion
		/// </summary>
		public int Amount { get { return _Amount; } }

		private int _Amount;

		/// <summary>
		/// The promotion Prices. Can have multiple prices with different currencies
		/// </summary>
		public List<BundlePrice> Prices { get { return _Prices; } }

		private List<BundlePrice> _Prices = new List<BundlePrice> ();

		/// <summary>
		/// The promotion discount label
		/// </summary>
		public string Discount { get { return _Discount; } }

		private string _Discount;

		/// <summary>
		/// The begin date of an active promotion for this bundle. Returns DateTime.Min if there is no active promotion for this bundle.
		/// </summary>
		public DateTime PromotionBeginDate { get { return _StartDate; } }

		private DateTime _StartDate;

		/// <summary>
		/// The begin date of an active promotion for this bundle. Returns DateTime.Min if there is no active promotion for this bundle.
		/// </summary>
		public DateTime PromotionEndDate { get { return _EndDate; } }

		private DateTime _EndDate;

		public Promotion (int bundleId, int amount, List<SpilBundlePriceData> prices, string discount, DateTime startDate, DateTime endDate)
		{
			_BundleId = bundleId;
			_Amount = amount;

			if (prices != null) {
				foreach (SpilBundlePriceData bundlePriceData in prices) {
					_Prices.Add (new BundlePrice (bundlePriceData.currencyId, bundlePriceData.value));
				}
			}

			_Discount = discount;
			_StartDate = startDate;
			_EndDate = endDate;
		}
	}

	#region Tracking Objects

	/// <summary>
	/// Helper object for currencies used for the TrackWalletInventoryEvent
	/// </summary>
	public class TrackingCurrency
	{
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
	public class TrackingItem
	{
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
		/// The type of item. Should be 100 or higher
		/// </summary>
		public int type;
	}

	#endregion



}


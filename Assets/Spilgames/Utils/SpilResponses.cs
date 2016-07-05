using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpilGames.Unity.Utils
{ 
    public class SpilResponse
    {
        public string name;
        public string type;
        public string action;
        public string message;

        // The data field is not included here since it's object type varies between the different 
        // types of responses. Use the type field to identify the specific type of SpilResponse, 
        // then deserialize the JSON string again using that type. TODO: Make that prettier?
    }

    public enum enumAdType
    {
        RewardVideo = 0,
        Interstitial = 1,
        MoreApps = 2,
        Unknown = 3
    }

    public class SpilAdFinishedResponse
    {
        /// <summary>
        /// Possible values: interstitial / rewardVideo / moreApps
        /// </summary>
        public string type;

        public enumAdType GetTypeAsEnum()
        {
            enumAdType adType = enumAdType.Unknown;
            if (type.ToLower().Trim().Equals("rewardvideo"))
            {
                adType = enumAdType.RewardVideo;
            }
            else if (type.ToLower().Trim().Equals("interstitial"))
            {
                adType = enumAdType.Interstitial;
            }
            else if (type.ToLower().Trim().Equals("moreapps"))
            {
                adType = enumAdType.MoreApps;
            }
            if (adType == enumAdType.Unknown)
            {
                Debug.Log("SpilSDK-Unity AdNotAvailable event fired but type is unknown. Type: " + type);
            }
            return adType;
        }

        /// <summary>
        /// Possible values: close / dismiss
        /// </summary>
        public string reason;

        /// <summary>
        /// reward / empty
        /// </summary>
        public RewardData reward;

        /// <summary>
        /// DFP / Fyber / Chartboost
        /// </summary>
        public string network;
    }

    #region Rewards 

    // TODO: These classes are based on the old OnReward handler code in Spil.cs
    // Since the OnReward handler was deleted from Spil.cs because it was experimental
    // these classes here may not actually represent a proper RewardResponse JSON string.
    // When we actually do start using the OnReward handler and know what a proper
    // RewardResponse JSON string will look like update these classes!!

    // Example reward JSON string (contains RewardResponse object): "{ \"name\":\"reward\",\"action\":\"receive\",\"type\":\"reward\",\"data\":{ \"eventData\":{ \"currencyName\":\"Coins\",\"currencyId\":\"coins\",\"reward\":\"50\"} } }";

    public class RewardResponse : SpilResponse
    {
        public RewardEventData data;
    }

    public class RewardEventData
    {
        public RewardData eventData;
    }

    public class RewardData
    {
        public string currencyName;
        public string currencyId;
        public int reward;
    }


	// Pushnotifications reward classes

	public class PushNotificationRewardResponse : SpilResponse
	{
		public PushRewardEventData data;
	}

	public class PushRewardEventData
	{
		public NotificationRewardData eventData;
	}

	public class NotificationRewardData{
		public string currencyName;
		public int currencyId;
		public int reward;
	}





    #endregion

    #region Packages

        // Example Promotions JSON string (contains single Promotion object): "{\"packageId\":\"87600\",\"items\":[{\"id\":\"14\",\"type\":\"CURRENCY\",\"value\":\"400\"}],\"discountLabel\":\"30%\",\"startTime\":\"2016-01-02 01:02:03\",\"endTime\":\"2016-02-03 02:03:04\"}"

        public class PromotionData
        {
            public string packageId;
			public List<PackageItemData> items;
			public string discountLabel;
            public DateTime startTime;
            public DateTime endTime;
        }

        // Example packages JSON string (contains List<Packages>)): "[{ \"packageId\":\"87596\",\"items\":[{\"id\":\"13\",\"type\":\"CURRENCY\",\"value\":\"100\"},{\"id\":\"256\",\"type\":\"ITEM\",\"value\":\"2\"}],\"discountLabel\":\"35%\",\"hasPromotion\":false},{\"packageId\":\"87600\",\"items\":[{\"id\":\"14\",\"type\":\"CURRENCY\",\"value\":\"200\"}],\"discountLabel\":\"15%\",\"hasPromotion\":true}]"

        public class PackageData
        {
            public string packageId;
			public List<PackageItemData> items;
			public string discountLabel;
            public bool hasPromotion;
        }

        public class PackageItemData
        {
            public string id;
            public string type;
            public string value;
        }

    #endregion

	#region Spil Game Objects

		public class SpilCurrencyData
		{
			public int id;
			public string name;
			public int type;
		}
		
		public class SpilItemData
		{
			public int id;
			public string name;
			public int type;
		}
		
		public class SpilBundlePriceData
		{
			public int currencyId;
			public int value;
		}
		
		public class SpilBundleItemData
		{
			public int id;
			public int amount;
		}
		
		public class SpilBundleData
		{
			public int id;
			public string name;
			public List<SpilBundlePriceData> prices;
			public List<SpilBundleItemData> items;
		}
		
		public class SpilGameData
		{
			public List<SpilCurrencyData> currencies;
			public List<SpilItemData> items;
			public List<SpilBundleData> bundles;
			public List<SpilShopTabData> shop;
			public List<SpilShopPromotionData> promotions;
		}

		#region Shop

		public class SpilShopEntryData
		{
			public int bundleId;
			public string label;
			public int position;
		}

		public class SpilShopTabData
		{
			public string name;
			public List<SpilShopEntryData> entries;
		}

		public class SpilShopPromotionData
		{
			public int bundleId;
			public int amount;
			public List<SpilBundlePriceData> prices;
			public string discount;
			public DateTime startDate;
			public DateTime endDate;
		}

		#endregion

	#endregion
	
	#region Player Data Objects
	
		public class PlayerCurrencyData : SpilCurrencyData
		{
			public int currentBalance;
			public int delta;
		}
		
		public class WalletData
		{
			public List<PlayerCurrencyData> currencies;
			public int offset;
			public string logic;
		}
		
		public class PlayerItemData : SpilItemData
		{
			public int amount;
			public int value;
		}
		
		public class InventoryData
		{
			public List<PlayerItemData> items;
			public int offset;
			public string logic;
		}
	
	#endregion

	public class SpilErrorMessage
	{
		public int id;
		public string name;
		public string message;
	}	
}
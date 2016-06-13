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
            if (type.Equals("rewardVideo"))
            {
                adType = enumAdType.RewardVideo;
            }
            else if (type.Equals("interstitial"))
            {
                adType = enumAdType.Interstitial;
            }
            else if (type.Equals("moreApps"))
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

    #endregion

    #region Packages

        // Example Promotions JSON string (contains single Promotion object): "{\"packageId\":\"87600\",\"items\":[{\"id\":\"14\",\"type\":\"CURRENCY\",\"value\":\"400\"}],\"discountLabel\":\"30%\",\"startTime\":\"2016-01-02 01:02:03\",\"endTime\":\"2016-02-03 02:03:04\"}"

        public class PromotionData
        {
            public string packageId;
            public List<ItemData> items;
            public string discountLabel;
            public DateTime startTime;
            public DateTime endTime;
        }

        // Example packages JSON string (contains List<Packages>)): "[{ \"packageId\":\"87596\",\"items\":[{\"id\":\"13\",\"type\":\"CURRENCY\",\"value\":\"100\"},{\"id\":\"256\",\"type\":\"ITEM\",\"value\":\"2\"}],\"discountLabel\":\"35%\",\"hasPromotion\":false},{\"packageId\":\"87600\",\"items\":[{\"id\":\"14\",\"type\":\"CURRENCY\",\"value\":\"200\"}],\"discountLabel\":\"15%\",\"hasPromotion\":true}]"

        public class PackageData
        {
            public string packageId;
            public List<ItemData> items;
            public string discountLabel;
            public bool hasPromotion;
        }

        public class ItemData
        {
            public string id;
            public string type;
            public string value;
        }

    #endregion
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace SpilGames.Unity.Base.SDK {
    #region Spil Game Objects

    public class SpilCurrencyData {
        public int id;
        public string name;
        public int initialValue;
        public int type;
        public string imageUrl;
        public string displayName;
        public string displayDescription;
    }

    public class SpilItemData {
        public int id;
        public string name;
        public int initialValue;
        public int type;
        public string imageUrl;
        public string displayName;
        public string displayDescription;
        public bool isGacha;
        public List<SpilGachaContent> content;
    }

    public class SpilGachaContent {
        public int id;
        public string type;
        public int amount;
        public int weight;
    }

    public class SpilBundlePriceData {
        public int currencyId;
        public int value;
    }

    public class SpilBundleItemData {
        public int id;
        public int amount;
    }

    public class SpilBundleData {
        public int id;
        public string name;
        public List<SpilBundlePriceData> prices;
        public List<SpilBundleItemData> items;
        public string imageUrl;
        public string displayName;
        public string displayDescription;
    }

    public class SpilGameData {
        public List<SpilCurrencyData> currencies;
        public List<SpilItemData> items;
        public List<SpilBundleData> bundles;
        public List<SpilShopTabData> shop;
        public List<SpilShopPromotionData> promotions;
    }

    #endregion

    #region Shop

    public class SpilShopEntryData {
        public int bundleId;
        public string label;
        public int position;
        public List<SpilShopImageEntry> imageEntries;
    }

    public class SpilShopTabData {
        public string name;
        public List<SpilShopImageEntry> imageEntries;
        public List<SpilShopEntryData> entries;
    }

    public class SpilShopPromotionData {
        public int bundleId;
        public int amount;
        public List<SpilBundlePriceData> prices;
        public string discount;
        public DateTime startDate;
        public DateTime endDate;
        public List<SpilShopImageEntry> imageEntries;
    }

    public class SpilShopImageEntry {
        public string name;
        public string imageUrl;
    }

    #endregion
}
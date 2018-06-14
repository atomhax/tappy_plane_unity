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
        public Dictionary<string, object> properties = new Dictionary<string, object>();
    }

    public class SpilGachaContent {
        public int id;
        public string type;
        public int amount;
        public int weight;
        public int position;
        public string imageUrl;
    }

    public class SpilBundlePriceData {
        public int currencyId;
        public int value;
    }

    public class SpilBundleItemData {
        public int id;
        public string type;
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
    }

    #endregion

    #region Shop

    public class SpilShopEntryData {
        public int id;
        public string type;
        public string label;
        public int position;
        public List<SpilShopImageEntry> imageEntries;
    }

    public class SpilShopTabData {
        public string name;
        public List<SpilShopImageEntry> imageEntries;
        public List<SpilShopEntryData> entries;
    }

    public class SpilShopImageEntry {
        public string name;
        public string imageUrl;
    }
    
    #endregion
    
    #region Promotions
    
    public class SpilPromotionData {
        public int id;
        public string name;
        public int amountPurchased;
        public int maxPurchase;
        public string label;
        public long startDate;
        public long endDate;
        public List<SpilPromotionAffectedEntity> affectedEntities;
        public List<SpilPromotionExtraEntity> extraEntities;
        public List<SpilPromotionPriceOverride> priceOverride;
        public List<SpilPromotionGameAsset> gameAssets;
    }

    public class SpilPromotionAffectedEntity {
        public int id;
        public string type;
    }

    public class SpilPromotionExtraEntity {
        public int id;
        public string type;
        public int amount;
    }

    public class SpilPromotionGameAsset {
        public string name;
        public string locale;
        public string position;
        public string type;
        public string value;
    }

    public class SpilPromotionPriceOverride {
        public int id;
        public string type;
        public int amount;
    }
    
    #endregion
}
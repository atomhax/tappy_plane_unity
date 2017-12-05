using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace SpilGames.Unity.Base.SDK {
    // Example Promotions JSON string (contains single Promotion object): "{\"packageId\":\"87600\",\"items\":[{\"id\":\"14\",\"type\":\"CURRENCY\",\"value\":\"400\"}],\"discountLabel\":\"30%\",\"startTime\":\"2016-01-02 01:02:03\",\"endTime\":\"2016-02-03 02:03:04\","type":"EXTRAITEM"}

    public class PromotionData {
        public string packageId;
        public List<PackageItemData> items;
        public string discountLabel;
        public DateTime startTime;
        public DateTime endTime;
        public string type;
        public List<PackageImageEntry> imageEntries;

        public string GetImageUrl(string name) {
            foreach (PackageImageEntry entry in imageEntries) {
                if (entry.name == name) {
                    return entry.imageUrl;
                }
            }
            return "";
        }
    }

    // Example packages JSON string (contains List<Packages>)): "[{ \"packageId\":\"87596\",\"items\":[{\"id\":\"13\",\"type\":\"CURRENCY\",\"value\":\"100\"},{\"id\":\"256\",\"type\":\"ITEM\",\"value\":\"2\"}],\"discountLabel\":\"35%\",\"hasPromotion\":false},{\"packageId\":\"87600\",\"items\":[{\"id\":\"14\",\"type\":\"CURRENCY\",\"value\":\"200\"}],\"discountLabel\":\"15%\",\"hasPromotion\":true}]"

    public class PackageData {
        public string packageId;
        public List<PackageItemData> items;
        public string discountLabel;
        public bool hasPromotion;
        public List<PackageImageEntry> imageEntries;
    }

    public class PackageItemData {
        public string id;
        public string type;
        public string value;
    }
    
    public class PackageImageEntry {
        public string name;
        public string imageUrl;
    }
}
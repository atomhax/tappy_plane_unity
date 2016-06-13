using System;
using System.Collections.Generic;
using System.Linq;
using SpilGames.Unity.Utils;
using UnityEngine;

namespace SpilGames.Unity.Helpers
{ 
    /// <summary>
    /// This is the business object that the developer can use to work with Packages and Promotions.
    /// Much easier to work with than the raw data!
    /// </summary>
    public class PackagesHelper
    {
        public List<Package> Packages;

        public PackagesHelper(List<PackageData> packages)
        {
            Packages = new List<Package>();

            int promotions = 0;

            // Create package objects with promotion data (if any)
            List<Package> returnValue = new List<Package>();
            foreach (PackageData packageData in packages)
            {            
                if (packageData.hasPromotion) 
                {
                    string promotionString = Spil.Instance.getPromotion(packageData.packageId);
                    if (!string.IsNullOrEmpty(promotionString))
                    {
                        PromotionData promotionData = JsonHelper.getObjectFromJson<PromotionData>(promotionString);

                        // Check datetime, even though the server shouldn't send inactive promotions the data we're using might be old.
                        DateTime currentTime = DateTime.Now;
                        if (currentTime >= promotionData.startTime && currentTime <= promotionData.endTime)
                        {
                            promotions += 1;
                            Packages.Add(new Package(packageData.packageId, packageData.discountLabel, packageData.items, promotionData.items, promotionData.discountLabel, promotionData.startTime.ToString(), promotionData.endTime.ToString()));
                        } else {
                            Packages.Add(new Package(packageData.packageId, packageData.discountLabel, packageData.items));
                        }
                    }
                } else {
                    Packages.Add(new Package(packageData.packageId, packageData.discountLabel, packageData.items));
                }
            }
            //Debug.Log("SpilSDK-Unity Found " + packages.Count() + " packages and " + promotions + " promotions");
        }

        public Package GetPackageById(string packageId)
        {
            return Packages.FirstOrDefault(a => a.Id.Equals(packageId));
        }
    }

    /// <summary>
    /// This is the business object that the developer can use to work with Packages and Promotions.
    /// </summary>
    public class Package
    {
        /// <summary>
        /// The package Id
        /// </summary>
        public string Id { get { return id; } }
        private string id;

        /// <summary>
        /// The original discount label of the item before any promotion is applied.
        /// </summary>
        public string OriginalDiscountLabel { get { return originalDiscountLabel; } }
        private string originalDiscountLabel;

        /// <summary>
        /// The discount label of the package if there is an active promotion for this package. Returns null if there is no promotion.
        /// </summary>
        public string PromotionDiscountLabel { get { return promotionDiscountLabel; } }
        private string promotionDiscountLabel;

        /// <summary>
        /// The begin date of an active promotion for this package. Returns DateTime.Min if there is no active promotion for this package.
        /// </summary>
        public DateTime PromotionBeginDate { get { return promotionBeginDate; } }
        private DateTime promotionBeginDate;

        /// <summary>
        /// The begin date of an active promotion for this package. Returns DateTime.Min if there is no active promotion for this package.
        /// </summary>
        public DateTime PromotionEndDate { get { return promotionEndDate; } }
        private DateTime promotionEndDate;

        public List<Item> Items;

        public Package(string id, string originalDiscountLabel, List<ItemData> packageItems, List<ItemData> promotionItems = null, string promotionDiscountLabel = null, string promotionBeginDate = null, string promotionEndDate = null)
        {
            this.id = id;
            this.originalDiscountLabel = originalDiscountLabel;
            this.promotionDiscountLabel = promotionDiscountLabel;
            this.promotionBeginDate = promotionBeginDate != null ? DateTime.Parse(promotionBeginDate) : DateTime.MinValue;
            this.promotionEndDate = promotionEndDate != null ? DateTime.Parse(promotionEndDate) : DateTime.MinValue;

            Items = new List<Item>();

            // Populate items
            foreach(ItemData packageItem in packageItems)
            {
                string promotionvalue = null;
                if(promotionItems != null)
                {
                    foreach (ItemData promotionItem in promotionItems)
                    {
                        if(promotionItem.id.Equals(packageItem.id))
                        {
                            promotionvalue = promotionItem.value;
                        }
                    }       
                }
                Items.Add(new Item(packageItem.id, packageItem.type, packageItem.value, promotionvalue));
            }
        }

        /// <summary>
        /// Gets the actual discount label for this package. Checks for any active promotion for this package. 
        /// </summary>
        /// <returns>The original discount label if there is no active promotion or the promotion discount label if there is an active promotion.</returns>
        public string GetRealDiscountLabel()
        {
            return PromotionDiscountLabel != null ? PromotionDiscountLabel : originalDiscountLabel;
        }

        /// <summary>
        /// Checks for any active promotion for this package, also checks if the current date is between the promotion's begin and end date. 
        /// </summary>
        /// <returns>True if there is an active promotion, false if there is no active promotion.</returns>
        public bool HasActivePromotion()
        {
            return PromotionDiscountLabel != null;
        }

        public Item GetItemById(string itemId)
        {
            return Items.FirstOrDefault(a => a.Id.Equals(itemId));
        }
    }

    /// <summary>
    /// This is the business object that the developer can use to work with Items associated with Packages and Promotions.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// The item Id, also known as "SKU".
        /// </summary>
        public string Id { get { return id; } }
        private string id;

        /// <summary>
        /// The type of the item.
        /// </summary>
        public string Type { get { return Type; } }
        private string type;

        /// <summary>
        /// The original value of the item before any promotion is applied.
        /// </summary>
        public string OriginalValue { get { return originalValue; } }
        private string originalValue;

        /// <summary>
        /// The value of the item if there is an active promotion for this item. Returns null if there is no promotion.
        /// </summary>
        public string PromotionValue { get { return promotionValue; } }
        private string promotionValue;

        public Item(string id, string type, string originalValue, string promotionValue = null)
        {
            this.id = id;
            this.type = type;
            this.originalValue = originalValue;
            this.promotionValue = promotionValue;
        }

        /// <summary>
        /// Gets the actual value of this item. Checks for any active promotion for this item. 
        /// </summary>
        /// <returns>The original value if there is no active promotion or the promotion value if there is an active promotion.</returns>
        public string GetRealValue()
        {
            return PromotionValue != null ? PromotionValue : OriginalValue;
        }

        /// <summary>
        /// Checks for any active promotion for this item, also checks if the current date is between the promotion's begin and end date. 
        /// </summary>
        /// <returns>True if there is an active promotion, false if there is no active promotion.</returns>
        public bool HasActivePromotion()
        {
            return PromotionValue != null;
        }
    }
}
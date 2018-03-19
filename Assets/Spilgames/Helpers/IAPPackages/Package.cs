using UnityEngine;
using System.Collections;
using System;
using SpilGames.Unity.Base.SDK;
using System.Collections.Generic;
using System.Linq;

namespace SpilGames.Unity.Helpers.IAPPackages {
    /// <summary>
    /// This is the business object that the developer can use to work with Packages and Promotions.
    /// </summary>
    public class Package {

        public int Id {
            get { return id; }
        }

        private int id;

        /// <summary>
        /// The package Id
        /// </summary>
        public string PackageId {
            get { return packageId; }
        }

        private string packageId;
        
        /// <summary>
        /// The original discount label of the item before any promotion is applied.
        /// </summary>
        public string DiscountLabel {
            get { return discountLabel; }
        }

        private string discountLabel;

        public List<PackageItem> Items;

        public Package(int id, string packageId, string discountLabel, List<PackageItemData> packageItems) {
            this.id = id;
            this.packageId = packageId;
            this.discountLabel = discountLabel;

            Items = new List<PackageItem>();

            // Populate items
            foreach (PackageItemData packageItem in packageItems) {
                Items.Add(new PackageItem(packageItem.id, packageItem.type, packageItem.value));
            }
        }

        public PackageItem GetItemById(string itemId) {
            return Items.FirstOrDefault(a => a.Id.Equals(itemId));
        }
        
        /// <summary>
        /// Checks for any active promotion for this item, also checks if the current date is between the promotion's begin and end date. 
        /// </summary>
        /// <returns>True if there is an active promotion, false if there is no active promotion.</returns>
        public bool HasActivePromotion() {
            return Spil.Instance.GetPromotions().HasPackagePromotion(packageId);
        }
    }

    /// <summary>
    /// This is the business object that the developer can use to work with Items associated with Packages and Promotions.
    /// </summary>
    public class PackageItem {
        /// <summary>
        /// The item Id, also known as "SKU".
        /// </summary>
        public string Id {
            get { return id; }
        }

        private string id;

        /// <summary>
        /// The type of the item.
        /// </summary>
        public string Type {
            get { return type; }
        }

        private string type;

        /// <summary>
        /// The original value of the item before any promotion is applied.
        /// </summary>
        public string Value {
            get { return value; }
        }

        private string value;

        public PackageItem(string id, string type, string value) {
            this.id = id;
            this.type = type;
            this.value = value;
        }
    }
}
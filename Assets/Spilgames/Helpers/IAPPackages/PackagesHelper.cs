using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Json;

namespace SpilGames.Unity.Helpers.IAPPackages {
    /// <summary>
    /// This is the business object that the developer can use to work with Packages and Promotions.
    /// Much easier to work with than the raw data!
    /// </summary>
    public class PackagesHelper {
        public List<Package> Packages;

        public PackagesHelper(List<PackageData> packages) {
            Packages = new List<Package>();

            // Create package objects with promotion data (if any)
            if (packages != null) {
                foreach (PackageData packageData in packages) {
                    List<PromotionData> activePromotions = new List<PromotionData>();

                    if (packageData.hasPromotion) {
                        string promotionsString = Spil.Instance.GetPromotions(packageData.packageId);
                        if (!string.IsNullOrEmpty(promotionsString)) {
                            List<PromotionData> promotions = JsonHelper.getObjectFromJson<List<PromotionData>>(promotionsString);

                            // Check datetime, even though the server shouldn't send inactive promotions the data we're using might be old.
                            foreach (PromotionData promotionData in promotions) {
                                DateTime currentTime = DateTime.Now;
                                if (currentTime >= promotionData.startTime && currentTime <= promotionData.endTime) {
                                    activePromotions.Add(promotionData);
                                }
                            }
                        }
                    }

                    Packages.Add(new Package(packageData.packageId, packageData.discountLabel, packageData.items, activePromotions));
                }
            }
        }

        public Package GetPackageById(string packageId) {
            return Packages.FirstOrDefault(a => a.Id.Equals(packageId));
        }
    }
}
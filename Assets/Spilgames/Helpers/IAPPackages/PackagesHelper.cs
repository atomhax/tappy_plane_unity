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
                    Packages.Add(new Package(packageData.id, packageData.packageId, packageData.discountLabel, packageData.items));
                }
            }
        }

        public Package GetPackageById(string packageId) {
            return Packages.FirstOrDefault(a => a.PackageId.Equals(packageId));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Json;

namespace SpilGames.Unity.Helpers.IAPPackages
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
			//List<Package> returnValue = new List<Package>();
			if (packages != null) {
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
			}
		}
		
		public Package GetPackageById(string packageId)
		{
			return Packages.FirstOrDefault(a => a.Id.Equals(packageId));
		}
	}

}
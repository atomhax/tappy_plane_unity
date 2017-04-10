using UnityEngine;
using System.Collections;
using System;
using SpilGames.Unity.Base.SDK;
using System.Collections.Generic;
using System.Linq;

namespace SpilGames.Unity.Helpers.IAPPackages
{ 

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
		
		public List<PackageItem> Items;
		
		public Package(string id, string originalDiscountLabel, List<PackageItemData> packageItems, List<PackageItemData> promotionItems = null, string promotionDiscountLabel = null, string promotionBeginDate = null, string promotionEndDate = null)
		{
			this.id = id;
			this.originalDiscountLabel = originalDiscountLabel;
			this.promotionDiscountLabel = promotionDiscountLabel;
			this.promotionBeginDate = promotionBeginDate != null ? DateTime.Parse(promotionBeginDate) : DateTime.MinValue;
			this.promotionEndDate = promotionEndDate != null ? DateTime.Parse(promotionEndDate) : DateTime.MinValue;
			
			Items = new List<PackageItem>();
			
			// Populate items
			foreach(PackageItemData packageItem in packageItems)
			{
				string promotionvalue = null;
				if(promotionItems != null)
				{
					foreach (PackageItemData promotionItem in promotionItems)
					{
						if(promotionItem.id.Equals(packageItem.id))
						{
							promotionvalue = promotionItem.value;
						}
					}       
				}
				Items.Add(new PackageItem(packageItem.id, packageItem.type, packageItem.value, promotionvalue));
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
		
		public PackageItem GetItemById(string itemId)
		{
			return Items.FirstOrDefault(a => a.Id.Equals(itemId));
		}
	}
	
	/// <summary>
	/// This is the business object that the developer can use to work with Items associated with Packages and Promotions.
	/// </summary>
	public class PackageItem
	{
		/// <summary>
		/// The item Id, also known as "SKU".
		/// </summary>
		public string Id { get { return id; } }
		private string id;
		
		/// <summary>
		/// The type of the item.
		/// </summary>
		public string Type { get { return type; } }
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
		
		public PackageItem(string id, string type, string originalValue, string promotionValue = null)
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


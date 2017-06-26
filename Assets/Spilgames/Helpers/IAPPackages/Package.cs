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
		
		public List<PackageItem> Items;

		public List<PromotionData> promotions;

		public Package (string id, string originalDiscountLabel, List<PackageItemData> packageItems, List<PromotionData> promotions = null)
		{
			this.id = id;
			this.originalDiscountLabel = originalDiscountLabel;

			this.promotions = promotions;
			if (this.promotions == null) {
				this.promotions = new List<PromotionData> ();
			}

			Items = new List<PackageItem> ();

			// Populate items
			foreach(PackageItemData packageItem in packageItems)
			{
				double promotionValue = 0;
				double extraValue = 0;

				foreach (PromotionData promotion in this.promotions) {
					if (promotion.items != null) {
						// Apply regular promotions
						if (String.IsNullOrEmpty (promotion.type)) {
							foreach (PackageItemData promotionItem in promotion.items) {
								if (promotionItem.id.Equals (packageItem.id)) {
									double.TryParse (promotionItem.value, out promotionValue);
								}
							}
						}
						// Apply extra item promotions
						if (promotion.type.Equals("EXTRAITEM")) {
							foreach (PackageItemData promotionItem in promotion.items) {
								if (promotionItem.id.Equals (packageItem.id)) {
									double.TryParse (promotionItem.value, out extraValue);
								}
							}
						}
					}
				}

				double totalPromotion = promotionValue + extraValue;
				Items.Add(new PackageItem(packageItem.id, packageItem.type, packageItem.value, 
					totalPromotion != 0 ? totalPromotion.ToString() : null));
			}
		}

		/// <summary>
		/// Get a list with discounts labels which apply to this package. 
		/// </summary>
		/// <returns>The list with discount labels.</returns>
		public List<string> GetDiscountLabels()
		{
			List<string> result = new List<string> ();
			foreach (PromotionData promotion in promotions) {
				if (String.IsNullOrEmpty (promotion.discountLabel)) {
					continue;
				}
				result.Add (promotion.discountLabel);
			}
			return result;
		}

		/// <summary>
		/// Get a concatted discount label for this package. 
		/// </summary>
		/// <returns>The list with discount labels.</returns>
		public string GetConcattedDiscountLabel() {
			string result = "";
			List<string> labels = GetDiscountLabels ();
			foreach (string label in labels) {
				if (result.Length > 0) {
					result += " + ";
				}
				result += label;
			}
			return result;
		}

		/// <summary>
		/// Checks for any active promotion for this package, also checks if the current date is between the promotion's begin and end date. 
		/// </summary>
		/// <returns>True if there is an active promotion, false if there is no active promotion.</returns>
		public bool HasActivePromotion ()
		{
			return promotions.Count > 0;
		}

		public PackageItem GetItemById (string itemId)
		{
			return Items.FirstOrDefault (a => a.Id.Equals (itemId));
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

		public PackageItem (string id, string type, string originalValue, string promotionValue = null)
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
		public string GetRealValue ()
		{
			return PromotionValue != null ? PromotionValue : OriginalValue;
		}

		/// <summary>
		/// Checks for any active promotion for this item, also checks if the current date is between the promotion's begin and end date. 
		/// </summary>
		/// <returns>True if there is an active promotion, false if there is no active promotion.</returns>
		public bool HasActivePromotion ()
		{
			return PromotionValue != null;
		}
	}
}
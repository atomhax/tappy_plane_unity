using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using SpilGames.Unity.Base.SDK;

namespace SpilGames.Unity.Helpers.GameData {
    /// <summary>
    /// This is the business object that the developer can use to work a Bundle.
    /// </summary>
    public class Bundle {
        /// <summary>
        /// The bundle Id
        /// </summary>
        public int Id {
            get { return _Id; }
        }

        private int _Id;

        /// <summary>
        /// The bundle Name
        /// </summary>
        public string Name {
            get { return _Name; }
        }

        private string _Name;

        /// <summary>
        /// The bundle Prices. Can have multiple prices with different currencies
        /// </summary>
        public List<BundlePrice> Prices {
            get { return _Prices; }
        }

        private List<BundlePrice> _Prices = new List<BundlePrice>();

        /// <summary>
        /// The bundle Items that are contained within a Bundle
        /// </summary>
        public List<BundleItem> Items {
            get { return _Items; }
        }

        private List<BundleItem> _Items = new List<BundleItem>();

        public string imageURL; // Needs to be public so it's included when converting to json (for WebGL)


        /// <summary>
        /// Get the local image path of the item. (disk cache)
        /// </summary>
        public string GetImagePath() {
            string imagePath = Spil.Instance.GetImagePath(imageURL);

            if (imagePath != null) {
                return imagePath;
            }
            else {
                Spil.Instance.RequestImage(imageURL, _Id, "bundle");
                return null;
            }
        }

        /// <summary>
        /// Checks if there is an image defined for the item.
        /// </summary>
        public bool HasImage() {
            return !String.IsNullOrEmpty(imageURL);
        }

        /// <summary>
        /// Gets the display name of the bundle.
        /// </summary>
        public string DisplayName {
            get { return _displayName; }
        }

        private string _displayName;

        /// <summary>
        /// Gets the display description of the bundle.
        /// </summary>
        public string DisplayDescription {
            get { return _displayDescription; }
        }

        private string _displayDescription;
        
        public Dictionary<string, object> Properties {
            get { return properties; }
        }

        private Dictionary<string, object> properties;

        public Bundle(int id, string name, List<SpilBundlePriceData> prices, List<SpilBundleItemData> items, string imageURL, string displayName, string displayDescription, Dictionary<string, object> properties) {
            _Id = id;
            _Name = name;
            this.imageURL = imageURL;
            _displayName = displayName;
            _displayDescription = displayDescription;

            //Adding Prices for Bundle
            if (prices != null) {
                foreach (SpilBundlePriceData bundlePriceData in prices) {
                    _Prices.Add(new BundlePrice(bundlePriceData.currencyId, bundlePriceData.value));
                }
            }

            //Adding Items to Bundle
            if (items != null) {
                foreach (SpilBundleItemData bundleItemData in items) {
                    _Items.Add(new BundleItem(bundleItemData.id, bundleItemData.type, bundleItemData.amount));
                }
            }

            this.properties = properties;
        }
    }

    /// <summary>
    /// This is the business object that the developer can use to work with for the Bundle Price.
    /// </summary>
    public class BundlePrice {
        /// <summary>
        /// The currency Id assoctiated with the Bundle Price
        /// </summary>
        public int CurrencyId {
            get { return _CurrencyId; }
        }

        private int _CurrencyId;

        /// <summary>
        /// The value required in order to consume the bundle for the assoctiated currency
        /// </summary>
        public int Value {
            get { return _Value; }
        }

        private int _Value;

        public BundlePrice(int currencyId, int value) {
            _CurrencyId = currencyId;
            _Value = value;
        }
    }

    /// <summary>
    /// This is the business object that the developer can use to work with for the Items contained in a Bundle.
    /// </summary>
    public class BundleItem {
        /// <summary>
        /// The item Id assoctiated with the Bundle Item
        /// </summary>
        public int Id {
            get { return _Id; }
        }

        private int _Id;

        public string Type { 
            get { return _Type; }
        }

        private string _Type;
        
        /// <summary>
        /// The amount if items contained withing the Bundle Item
        /// </summary>
        public int Amount {
            get { return _Amount; }
        }

        private int _Amount;

        public BundleItem(int id, string type, int amount) {
            _Id = id;
            _Type = type;
            _Amount = amount;
        }
    }
}
using UnityEngine;
using System.Collections;
using System;

namespace SpilGames.Unity.Helpers.GameData {
    /// <summary>
    /// This is the business object that the developer can use to work with a Currency.
    /// </summary>
    public class Currency {
        /// <summary>
        /// The currency Id
        /// </summary>
        public int Id {
            get { return id; }
        }

        private int id;

        /// <summary>
        /// The currency Name
        /// </summary>
        public string Name {
            get { return name; }
        }

        private string name;

        /// <summary>
        /// The currency Type (Premium and Non-Premium)
        /// </summary>
        public int Type {
            get { return type; }
        }

        public int type;

        public string imageUrl; // Needs to be public so it's included when converting to json (for WebGL)

        /// <summary>
        /// Get the local image path of the currency. (disk cache)
        /// </summary>
        public string GetImagePath() {
            string imagePath = Spil.Instance.GetImagePath(imageUrl);

            if (imagePath != null) {
                return imagePath;
            }
            else {
                Spil.Instance.RequestImage(imageUrl, id, "currency");
                return null;
            }
        }

        /// <summary>
        /// Checks if there is an image defined for the currency.
        /// </summary>
        public bool HasImage() {
            return !String.IsNullOrEmpty(imageUrl);
        }

        /// <summary>
        /// Gets the display name of the currency.
        /// </summary>
        public string DisplayName {
            get { return displayName; }
        }

        private string displayName;

        /// <summary>
        /// Gets the display description of the currency.
        /// </summary>
        public string DisplayDescription {
            get { return displayDescription; }
        }

        private string displayDescription;

        /// <summary>
        /// Gets the limit of the currency. This represents how many of this currency can the player have.
        /// </summary>
        public int Limit {
            get { return limit; }
        }

        private int limit;
        
        public Currency(int id, string name, int type, string imageUrl, string displayName, string displayDescription, int limit) {
            this.id = id;
            this.name = name;
            this.type = type;
            this.imageUrl = imageUrl;
            this.displayName = displayName;
            this.displayDescription = displayDescription;
            this.limit = limit;
        }
    }
}
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
            get { return _Id; }
        }

        private int _Id;

        /// <summary>
        /// The currency Name
        /// </summary>
        public string Name {
            get { return _Name; }
        }

        private string _Name;

        /// <summary>
        /// The currency Type (Premium and Non-Premium)
        /// </summary>
        public int Type {
            get { return _Type; }
        }

        public int _Type;

        private string _imageURL;

        /// <summary>
        /// Get the local image path of the currency. (disk cache)
        /// </summary>
        public string GetImagePath() {
            string imagePath = Spil.Instance.GetImagePath(_imageURL);

            if (imagePath != null) {
                return imagePath;
            }
            else {
                Spil.Instance.RequestImage(_imageURL, _Id, "currency");
                return null;
            }
        }

        /// <summary>
        /// Checks if there is an image defined for the currency.
        /// </summary>
        public bool HasImage() {
            return !String.IsNullOrEmpty(_imageURL);
        }

        /// <summary>
        /// Gets the display name of the currency.
        /// </summary>
        public string DisplayName {
            get { return _displayName; }
        }

        private string _displayName;

        /// <summary>
        /// Gets the display description of the currency.
        /// </summary>
        public string DisplayDescription {
            get { return _displayDescription; }
        }

        private string _displayDescription;

        public Currency(int id, string name, int type, string imageUrl, string displayName, string displayDescription) {
            _Id = id;
            _Name = name;
            _Type = type;
            _imageURL = imageUrl;
            _displayName = displayName;
            _displayDescription = displayDescription;
        }
    }
}
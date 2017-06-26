using UnityEngine;
using System.Collections;
using System;

namespace SpilGames.Unity.Helpers.GameData {
    /// <summary>
    /// This is the business object that the developer can use to work with an Item.
    /// </summary>
    public class Item {
        /// <summary>
        /// The item Id
        /// </summary>
        public int Id {
            get { return _Id; }
        }

        private int _Id;

        /// <summary>
        /// The item Name
        /// </summary>
        public string Name {
            get { return _Name; }
        }

        private string _Name;

        /// <summary>
        /// The item Type (Consumable, Booster and Permanent)
        /// </summary>
        public int Type {
            get { return _Type; }
        }

        public int _Type;

        private string _imageURL;


        /// <summary>
        /// Get the local image path of the item. (disk cache)
        /// </summary>
        public string GetImagePath() {
            string imagePath = Spil.Instance.GetImagePath(_imageURL);

            if (imagePath != null) {
                return imagePath;
            }
            else {
                Spil.Instance.RequestImage(_imageURL, _Id, "item");
                return null;
            }
        }

        /// <summary>
        /// Checks if there is an image defined for the item.
        /// </summary>
        public bool HasImage() {
            return !String.IsNullOrEmpty(_imageURL);
        }

        /// <summary>
        /// Gets the display name of the item.
        /// </summary>
        public string DisplayName {
            get { return _displayName; }
        }

        private string _displayName;

        /// <summary>
        /// Gets the display description of the item.
        /// </summary>
        public string DisplayDescription {
            get { return _displayDescription; }
        }

        private string _displayDescription;

        public Item(int id, string name, int type, string imageURL, string displayName, string displayDescription) {
            _Id = id;
            _Name = name;
            _Type = type;
            _imageURL = imageURL;
            _displayName = displayName;
            _displayDescription = displayDescription;
        }
    }
}
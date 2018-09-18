using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using SpilGames.Unity.Base.SDK;

namespace SpilGames.Unity.Helpers.GameData {
    /// <summary>
    /// This is the business object that the developer can use to work with an Item.
    /// </summary>
    public class Item {
        /// <summary>
        /// The item Id
        /// </summary>
        public int Id {
            get { return id; }
        }

        private int id;

        /// <summary>
        /// The item Name
        /// </summary>
        public string Name {
            get { return name; }
        }

        private string name;

        /// <summary>
        /// The item Type (Consumable, Booster and Permanent)
        /// </summary>
        public int Type {
            get { return type; }
        }

        private int type;

        private string imageUrl;

        /// <summary>
        /// Get the local image path of the item. (disk cache)
        /// </summary>
        public string GetImagePath() {
            string imagePath = Spil.Instance.GetImagePath(imageUrl);

            if (imagePath != null) {
                return imagePath;
            } else {
                Spil.Instance.RequestImage(imageUrl, id, "item");
                return null;
            }
        }

        /// <summary>
        /// Checks if there is an image defined for the item.
        /// </summary>
        public bool HasImage() {
            return !String.IsNullOrEmpty(imageUrl);
        }

        /// <summary>
        /// Gets the display name of the item.
        /// </summary>
        public string ReportingName {
            get { return reportingName; }
        }

        private string reportingName;
        
        /// <summary>
        /// Gets the display name of the item.
        /// </summary>
        public string DisplayName {
            get { return displayName; }
        }

        private string displayName;

        /// <summary>
        /// Gets the display description of the item.
        /// </summary>
        public string DisplayDescription {
            get { return displayDescription; }
        }

        private string displayDescription;

        /// <summary>
        /// Tells if the item is a gacha.
        /// </summary>
        public bool IsGacha {
            get { return isGacha; }
        }

        private bool isGacha;

        /// <summary>
        /// If the item is a gacha, this list contains all the possible items the user might get.
        /// </summary>
        public List<GachaContent> Content {
            get { return content; }
        }

        private List<GachaContent> content;

        public Dictionary<string, object> Properties {
            get { return properties; }
        }

        private Dictionary<string, object> properties;
        
        /// <summary>
        /// Gets the limit of the item. This represents how many of this item can the player have.
        /// </summary>
        public int Limit {
            get { return limit; }
        }

        private int limit;
        
        public Item(int id, string name, int type, string imageUrl, string reportingName, string displayName, string displayDescription, bool isGacha, List<SpilGachaContent> content, Dictionary<string, object> properties, int limit) {
            this.id = id;
            this.name = name;
            this.type = type;
            this.imageUrl = imageUrl;
            this.reportingName = reportingName;
            this.displayName = displayName;
            this.displayDescription = displayDescription;
            this.isGacha = isGacha;

            this.content = new List<GachaContent>();
            if (content != null && content.Count > 0) {
                foreach (SpilGachaContent gachaContent in content) {
                    this.content.Add(new GachaContent(gachaContent.id, gachaContent.type, gachaContent.amount, gachaContent.weight, gachaContent.position, gachaContent.imageUrl));
                }
            }

            this.properties = properties;
            this.limit = limit;
        }
    }
}
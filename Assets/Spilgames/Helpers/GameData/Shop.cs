using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpilGames.Unity.Base.SDK;
using System;

namespace SpilGames.Unity.Helpers.GameData {
    /// <summary>
    /// This is the business object that the developer can use to work with for the InGame Shop.
    /// </summary>
    public class Shop {
        /// <summary>
        /// The Shop Tabs as defined in SLOT
        /// </summary>
        public List<Tab> Tabs {
            get { return _Tabs; }
        }

        private List<Tab> _Tabs = new List<Tab>();

        public Shop(List<SpilShopTabData> shop) {
            if (shop != null) {
                foreach (SpilShopTabData tab in shop) {
                    _Tabs.Add(new Tab(tab.name, tab.entries, tab.imageEntries, tab.hasActivePromotions));
                }
            }
        }
    }

    /// <summary>
    /// This is the business object that the developer can use to work with for the Tab contained in the Shop.
    /// </summary>
    public class Tab {
        /// <summary>
        /// The tab Name
        /// </summary>
        public string Name {
            get { return _Name; }
        }

        private string _Name;

        
        
        /// <summary>
        /// The Shop Entries as defined in SLOT
        /// </summary>
        public List<Entry> Entries {
            get { return _Entries; }
        }

        private List<Entry> _Entries = new List<Entry>();

        /// <summary>
        /// The tab images. Can have multiple images.
        /// </summary>
        public List<ImageEntry> ImageEntries {
            get { return _ImageEntries; }
        }

        private List<ImageEntry> _ImageEntries = new List<ImageEntry>();

        public bool HasActivePromotions {
            get { return _HasActivePromotions; }
        }

        private bool _HasActivePromotions;
        
        public Tab(string name, List<SpilShopEntryData> entries, List<SpilShopImageEntry> imageEntries, bool hasActivePromotions) {
            _Name = name;

            if (imageEntries != null) {
                foreach (SpilShopImageEntry spilShopImageEntry in imageEntries) {
                    ImageEntry imageEntry = new ImageEntry(spilShopImageEntry.name, spilShopImageEntry.imageUrl);
                    ImageEntries.Add(imageEntry);
                }
            }

            _HasActivePromotions = hasActivePromotions;
            
            if (entries != null) {
                foreach (SpilShopEntryData entry in entries) {
                    _Entries.Add(new Entry(entry.id, entry.type, entry.label, entry.position, entry.imageEntries));
                }
            }
        }
    }

    /// <summary>
    /// This is the business object that the developer can use to work with for the Entry contained in the Tab.
    /// </summary>
    public class Entry {
        /// <summary>
        /// The Id assoctiated with this shop entry
        /// </summary>
        public int Id {
            get { return _Id; }
        }

        private int _Id;

        /// <summary>
        /// The entry type
        /// </summary>
        public string Type {
            get { return _Type; }
        }

        private string _Type;
        
        /// <summary>
        /// The entry label
        /// </summary>
        public string Label {
            get { return _Label; }
        }

        private string _Label;

        /// <summary>
        /// The entry position as defined in SLOT
        /// </summary>
        public int Position {
            get { return _Position; }
        }

        private int _Position;

        /// <summary>
        /// The entry images. Can have multiple images.
        /// </summary>
        public List<ImageEntry> ImageEntries {
            get { return _ImageEntries; }
        }

        private List<ImageEntry> _ImageEntries = new List<ImageEntry>();
        
        public Entry(int id, string type, string label, int position, List<SpilShopImageEntry> imageEntries) {
            _Id = id;
            _Type = type;
            _Label = label;
            _Position = position;
            
            if (imageEntries != null) {
                foreach (SpilShopImageEntry imageEntry in imageEntries) {
                    _ImageEntries.Add(new ImageEntry(imageEntry.name, imageEntry.imageUrl));
                }
            }
        }
    }

    public class ImageEntry {

        public string Name {
            get { return _Name; }
        }

        private string _Name;

        public string ImageUrl {
            get { return _ImageUrl; }
        }

        private string _ImageUrl;

        public ImageEntry(string name, string imageUrl) {
            _Name = name;
            _ImageUrl = imageUrl;
        }
        
    }
}
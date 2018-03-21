using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace SpilGames.Unity.Base.SDK {
    public class PackageData {
        public int id;
        public string packageId;
        public List<PackageItemData> items;
        public string discountLabel;
    }

    public class PackageItemData {
        public string id;
        public string type;
        public string value;
    }
}
using System;
using UnityEngine;

namespace SpilGames.Unity.Helpers.AssetBundles {
    public class AssetBundle {
        public string Name {
            get { return name; }
        }

        private string name;
        
        public string Type {
            get { return type; }
        }

        private string type;
        
        public long EndDate {
            get { return endDate; }
        }

        private long endDate;
        
        public string Url {
            get { return url; }
        }

        private string url;
                  
        public Hash128 Hash {
            get { return hash; }
        }

        private Hash128 hash;
        
        public uint Version {
            get { return version; }
        }

        private uint version;
        
        public AssetBundle(string name, string type, long endDate, string url, string hash, int version) {
            this.name = name;
            this.type = type;
            this.endDate = endDate;
            this.url = url;
            if (hash != null && !hash.Equals("")) {
                this.hash = Hash128.Parse(hash);  
            }

            if (version > 0) {
                this.version = Convert.ToUInt32(version);
            }  
        }
        
        public bool IsValid() {
            return endDate > System.DateTime.Now.Millisecond;
        }
    }
}
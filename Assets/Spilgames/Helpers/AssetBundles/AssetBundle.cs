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
        
        public AssetBundle(string name, string type, long endDate, string url) {
            this.name = name;
            this.type = type;
            this.endDate = endDate;
            this.url = url;
        }
        
        public bool IsValid() {
            return endDate > System.DateTime.Now.Millisecond;
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Json;

namespace SpilGames.Unity.Helpers.GameData {
    /// <summary>
    /// This is the business object that the developer can use to work with Spil Game Data (Currencies, Items, Bundles, Shop).
    /// Much easier to work with than the raw data!
    /// </summary>
    public class SpilGameDataHelper {
        public List<Currency> Currencies = new List<Currency>();
        public List<Item> Items = new List<Item>();
        public List<Bundle> Bundles = new List<Bundle>();
        public Shop Shop;

        public SpilGameDataHelper(SpilUnityImplementationBase Instance) {
            string spilGameDataString = Instance.GetSpilGameDataFromSdk();
            if (spilGameDataString != null) {
                SpilGameData spilGameData = JsonHelper.getObjectFromJson<SpilGameData>(spilGameDataString);
                if (spilGameData != null) {
                    AddDataToHelper(spilGameData.currencies, spilGameData.items, spilGameData.bundles, spilGameData.shop);
                    SpilLogging.Log("Spil GameData created");
                } else {
                    SpilLogging.Log("Spil GameData not created, Instance.GetSpilGameDataFromsdk() did not return any data.");
                }
            }
        }

        public void RefreshData(SpilUnityImplementationBase Instance) {
            string spilGameDataString = Instance.GetSpilGameDataFromSdk();
            if (spilGameDataString != null) {
                SpilGameData spilGameData = JsonHelper.getObjectFromJson<SpilGameData>(spilGameDataString);
                if (spilGameData != null) {
                    AddDataToHelper(spilGameData.currencies, spilGameData.items, spilGameData.bundles, spilGameData.shop);
                    SpilLogging.Log("Spil GameData Refreshed");
                } else {
                    SpilLogging.Log("Spil GameData not created, Instance.GetSpilGameDataFromsdk() did not return any data.");
                }
            }
        }

        /// <summary>
        /// Helper method that returns the Bundle for a given bundleId
        /// Returns null if no bundle was found
        /// </summary>
        public Bundle GetBundle(int bundleId) {
            if (Bundles != null) {
                foreach (Bundle bundle in Bundles) {
                    if (bundle.Id == bundleId) {
                        return bundle;
                    }
                }
                return null;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Helper method that returns the Bundle for a given name
        /// Returns null if no bundle was found
        /// </summary>
        public Bundle GetBundle(string name) {
            if (Bundles != null) {
                foreach (Bundle bundle in Bundles) {
                    if (bundle.Name.Equals(name)) {
                        return bundle;
                    }
                }
                return null;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Helper method that returns the Item for a given itemId
        /// Returns null if no item was found
        /// </summary>
        public Item GetItem(int itemId) {
            if (Items != null) {
                foreach (Item item in Items) {
                    if (item.Id == itemId) {
                        return item;
                    }
                }
                return null;
            }
            return null;
        }

        /// <summary>
        /// Helper method that returns the Item for a given name
        /// Returns null if no item was found
        /// </summary>
        public Item GetItem(string name) {
            if (Items != null) {
                foreach (Item item in Items) {
                    if (item.Name.Equals(name)) {
                        return item;
                    }
                }
                return null;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Helper method that returns the Gacha for a given name
        /// Returns null if no gacha was found
        /// </summary>
        public Item GetGacha(string name) {
            if (Items != null) {
                foreach (Item gacha in Items) {
                    if (gacha.Name.Equals(name) && gacha.IsGacha) {
                        return gacha;
                    }
                }
                return null;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Helper method that returns the Gacha for a given id
        /// Returns null if no gacha was found
        /// </summary>
        public Item GetGacha(int gachaId) {
            if (Items != null) {
                foreach (Item gacha in Items) {
                    if (gacha.Id == gachaId && gacha.IsGacha) {
                        return gacha;
                    }
                }
                return null;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Helper method that returns all the defined gacha items.
        /// </summary>
        public List<Item> GetAllGachas() {
            List<Item> gachas = new List<Item>();
            if (Items != null) {
                foreach (Item gacha in Items) {
                    if (gacha.IsGacha) {
                        gachas.Add(gacha);
                    }
                }
            }

            return gachas;
        }

        /// <summary>
        /// Helper method that returns the Currency for a given currencyId
        /// Returns null if no currency was found
        /// </summary>
        public Currency GetCurrency(int currencyId) {
            if (Currencies != null) {
                foreach (Currency currency in Currencies) {
                    if (currency.Id == currencyId) {
                        return currency;
                    }
                }
                return null;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Helper method that returns the Currency for a given name
        /// Returns null if no currency was found
        /// </summary>
        public Currency GetCurrency(string name) {
            if (Currencies != null) {
                foreach (Currency currency in Currencies) {
                    if (currency.Name.Equals(name)) {
                        return currency;
                    }
                }
                return null;
            } else {
                return null;
            }
        }
        
        private void AddDataToHelper(List<SpilCurrencyData> currencies, List<SpilItemData> items, List<SpilBundleData> bundles, List<SpilShopTabData> shop) {
            Currencies.Clear();

            if (currencies != null) {
                foreach (SpilCurrencyData spilCurrencyData in currencies) {
                    Currencies.Add(new Currency(spilCurrencyData.id, spilCurrencyData.name, spilCurrencyData.type, spilCurrencyData.imageUrl, spilCurrencyData.displayName, spilCurrencyData.displayDescription, spilCurrencyData.limit));
                }
            }

            Items.Clear();

            if (items != null) {
                foreach (SpilItemData spilItemsData in items) {
                    Items.Add(new Item(spilItemsData.id, spilItemsData.name, spilItemsData.type, spilItemsData.imageUrl, spilItemsData.reportingName, spilItemsData.displayName, spilItemsData.displayDescription, spilItemsData.isGacha, spilItemsData.content, spilItemsData.properties, spilItemsData.limit));
                }
            }

            Bundles.Clear();

            if (bundles != null) {
                foreach (SpilBundleData spilBundleData in bundles) {
                    Bundles.Add(new Bundle(spilBundleData.id, spilBundleData.name, spilBundleData.prices, spilBundleData.items, spilBundleData.imageUrl, spilBundleData.displayName, spilBundleData.displayDescription));
                }
            }

            //Adding shop data to helper
            Shop = new Shop(shop);
        }

        public void SpilGameDataHandler() {
            UpdateSpilGameData();
        }

        public void UpdateSpilGameData() {
            string spilGameDataString = Spil.Instance.GetSpilGameDataFromSdk();
            if (spilGameDataString != null) {
                SpilGameData spilGameData = JsonHelper.getObjectFromJson<SpilGameData>(spilGameDataString);
                AddDataToHelper(spilGameData.currencies, spilGameData.items, spilGameData.bundles, spilGameData.shop);
                SpilLogging.Log("Spil Game Data Updated");
            }
        }
    }
}
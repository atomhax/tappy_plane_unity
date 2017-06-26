using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.GameData;

namespace SpilGames.Unity.Helpers.PlayerData {
    /// <summary>
    /// This is the business object that the developer can use to work with the Wallet.
    /// </summary>
    public class Wallet {
        public List<PlayerCurrency> Currencies {
            get { return currencies; }
        }

        public List<PlayerCurrency> currencies;

        public Wallet(List<PlayerCurrencyData> currencyData) {
            currencies = new List<PlayerCurrency>();

            // Adding currencies of the player
            if (currencyData != null) {
                foreach (PlayerCurrencyData playerCurrencyData in currencyData) {
                    currencies.Add(new PlayerCurrency(playerCurrencyData.id, playerCurrencyData.name, playerCurrencyData.type, playerCurrencyData.currentBalance, playerCurrencyData.delta, playerCurrencyData.imageUrl, playerCurrencyData.displayName, playerCurrencyData.displayDescription));
                }
            }
        }

        public void Add(int currencyId, int amount, string reason, string location, string reasonDetails = null, string transactionId = null) {
            Spil.Instance.AddCurrencyToWallet(currencyId, amount, reason, location, reasonDetails, transactionId);
        }

        public void Subtract(int currencyId, int amount, string reason, string location, string reasonDetails = null, string transactionId = null) {
            Spil.Instance.SubtractCurrencyFromWallet(currencyId, amount, reason, location, reasonDetails, transactionId);
        }
    }

    /// <summary>
    /// This is the business object that the developer can use to work with for the Player owned Currency.
    /// </summary>
    public class PlayerCurrency : Currency {
        public int CurrentBalance {
            get { return currentBalance; }
        }

        private int currentBalance;

        public int Delta {
            get { return delta; }
        }

        private int delta;

        public PlayerCurrency(int id, string name, int type, int currentBalance, int delta, string imageURL, string displayName, string displayDescription) : base(id, name, type, imageURL, displayName, displayDescription) {
            this.currentBalance = currentBalance;
            this.delta = delta;
        }
    }
}
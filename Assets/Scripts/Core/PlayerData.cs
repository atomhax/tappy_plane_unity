using UnityEngine;
using System.Collections;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;
public class PlayerData : MonoBehaviour {




	public static void AddToCoins(int id,int amount, string reason){
		Spil.SpilPlayerDataInstance.Wallet.Add (id, amount, reason);
	}

	public static void SpendCoins(int id, int amount, string reason){
		Spil.SpilPlayerDataInstance.Wallet.Subtract (id, amount, reason);
	}

	public static int GetCurrencyAmount(int currencyID){
		for (int i = 0; i < Spil.SpilPlayerDataInstance.Wallet.Currencies.Count; i++) {
			if(Spil.SpilPlayerDataInstance.Wallet.Currencies[i].Id == currencyID){
				return Spil.SpilPlayerDataInstance.Wallet.Currencies [i].CurrrentBalance;
			}
		}
		return 0;
	}



}

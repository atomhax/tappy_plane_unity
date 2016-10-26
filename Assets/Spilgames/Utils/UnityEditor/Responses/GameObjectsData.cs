#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpilGames.Unity.Implementations;
using SpilGames.Unity.Utils.UnityEditor;

namespace SpilGames.Unity.Utils.UnityEditor.Responses
{
	public class GameObjectsData : Data
	{

		public List<SpilCurrencyData> currencies;
		public List<SpilItemData> items;
		public List<SpilBundleData> bundles;
		public List<SpilShopTabData> shop;
		public List<SpilShopPromotionData> promotions;

		private bool updatedFromServer;

		public void ProcessGameObjects(ResponseEvent response){
			if(response.data.HasField("currencies")){
				currencies = new List<SpilCurrencyData>();

				JSONObject currenciesJSON = new JSONObject(response.data.GetField("currencies"));

				for(int i=0 ; i < currenciesJSON.Count; i++){
					SpilCurrencyData currency = JsonUtility.FromJson<SpilCurrencyData>(currenciesJSON.list[i].Print(false));
					currencies.Add(currency);
				}


			}

			if(response.data.HasField("items")){
				items = new List<SpilItemData>();

				JSONObject itemsJSON = new JSONObject(response.data.GetField("items"));

				for(int i=0 ; i < itemsJSON.Count; i++){
					SpilItemData item = JsonUtility.FromJson<SpilItemData>(itemsJSON.list[i].Print(false));
					items.Add(item);
				}
			}

			if(response.data.HasField("bundles")){
				bundles = new List<SpilBundleData>();

				JSONObject bundlesJSON = new JSONObject(response.data.GetField("bundles"));

				for(int i=0 ; i < bundlesJSON.Count; i++){
					SpilBundleData bundle = JsonUtility.FromJson<SpilBundleData>(bundlesJSON.list[i].Print(false));
					bundles.Add(bundle);
				}
			}

			if(response.data.HasField("shop")){
				shop = new List<SpilShopTabData>();

				JSONObject shopJSON = new JSONObject(response.data.GetField("shop"));

				for(int i=0 ; i < shopJSON.Count; i++){
					SpilShopTabData tab = JsonUtility.FromJson<SpilShopTabData>(shopJSON.list[i].Print(false));
					shop.Add(tab);
				}

			}

			if(response.data.HasField("promotions")){
				promotions = new List<SpilShopPromotionData>();

				JSONObject promotionsJSON = new JSONObject(response.data.GetField("promotions"));

				for(int i=0 ; i < promotionsJSON.Count; i++){
					SpilShopPromotionData bundle = JsonUtility.FromJson<SpilShopPromotionData>(promotionsJSON.list[i].Print(false));
					promotions.Add(bundle);
				}
			}

			updatedFromServer = true;

			Spil.Instance.RefreshSpilGameData();

		}

		public string GetGameObjects(){
			//ToDo Initalise values
			if(!updatedFromServer){
				string gameData = System.IO.File.ReadAllText (Application.streamingAssetsPath + "/defaultGameData.json");
				return gameData;
			} else{
				JSONObject json = new JSONObject();

				if(currencies != null && currencies.Count != 0){
					json.AddField("currencies", JsonUtility.ToJson(currencies));
				}

				if(items != null && items.Count != 0){
					json.AddField("items", JsonUtility.ToJson(items));
				}

				if(bundles != null && bundles.Count != 0){
					json.AddField("bundles", JsonUtility.ToJson(bundles));
				}

				if(shop != null && shop.Count != 0){
					json.AddField("shop", JsonUtility.ToJson(shop));
				}

				if(promotions != null && promotions.Count != 0){
					json.AddField("promotions", JsonUtility.ToJson(promotions));
				}

				PlayerPrefs.SetString("gameObjectsEditor", json.Print(false));

				return json.Print(false);
			}
		}	



	}
}
#endif


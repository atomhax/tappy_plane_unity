#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpilGames.Unity.Implementations;
using SpilGames.Unity.Utils.UnityEditor;
using SpilGames.Unity.Helpers;
using System.IO;

namespace SpilGames.Unity.Utils.UnityEditor.Responses
{
	public class GameObjectsResponse : Response
	{

		public List<SpilCurrencyData> currencies;
		public List<SpilItemData> items;
		public List<SpilBundleData> bundles;
		public List<SpilShopTabData> shop;
		public List<SpilShopPromotionData> promotions;

		private bool updatedFromServer;

		public void ProcessGameObjectsResponse(ResponseEvent response){
			if(response.data.HasField("currencies")){
				currencies = new List<SpilCurrencyData>();

				JSONObject currenciesJSON = response.data.GetField("currencies");

				for(int i=0 ; i < currenciesJSON.Count; i++){
					SpilCurrencyData currency = JsonHelper.getObjectFromJson<SpilCurrencyData>(currenciesJSON.list[i].Print(false));
					currencies.Add(currency);
				}


			}

			if(response.data.HasField("items")){
				items = new List<SpilItemData>();

				JSONObject itemsJSON = response.data.GetField("items");

				for(int i=0 ; i < itemsJSON.Count; i++){
					SpilItemData item = JsonHelper.getObjectFromJson<SpilItemData>(itemsJSON.list[i].Print(false));
					items.Add(item);
				}
			}

			if(response.data.HasField("bundles")){
				bundles = new List<SpilBundleData>();

				JSONObject bundlesJSON = response.data.GetField("bundles");

				for(int i=0 ; i < bundlesJSON.Count; i++){
					SpilBundleData bundle = JsonHelper.getObjectFromJson<SpilBundleData>(bundlesJSON.list[i].Print(false));
					bundles.Add(bundle);
				}
			}

			if(response.data.HasField("shop")){
				shop = new List<SpilShopTabData>();

				JSONObject shopJSON = response.data.GetField("shop");

				for(int i=0 ; i < shopJSON.Count; i++){
					SpilShopTabData tab = JsonHelper.getObjectFromJson<SpilShopTabData>(shopJSON.list[i].Print(false));
					shop.Add(tab);
				}

			}

			if(response.data.HasField("promotions")){
				promotions = new List<SpilShopPromotionData>();

				JSONObject promotionsJSON = response.data.GetField("promotions");

				for(int i=0 ; i < promotionsJSON.Count; i++){
					SpilShopPromotionData bundle = JsonHelper.getObjectFromJson<SpilShopPromotionData>(promotionsJSON.list[i].Print(false));
					promotions.Add(bundle);
				}
			}

			updatedFromServer = true;

			Spil.GameData.RefreshData(Spil.Instance);

		}

		public string GetGameObjects(){
			//ToDo Initalise values
			if(!updatedFromServer){
				try{
					string gameData = System.IO.File.ReadAllText (Application.streamingAssetsPath + "/defaultGameData.json");
					return gameData;
				} catch(FileNotFoundException e){
					Debug.Log("defaultGameData.json not found. Creating a placeholder!" + e.ToString());
					string placeholder = "{\"currencies\": [],\"promotions\": [],\"shop\": [],\"bundles\": [],\"items\": []}";
					return placeholder;
				}
			} else{
				return JsonHelper.getJSONFromObject(this);
			}
		}	



	}
}
#endif


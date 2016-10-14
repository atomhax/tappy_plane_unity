#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using SpilGames.Unity.Helpers;
using SpilGames.Unity.Utils;
using System.Collections.Generic;


namespace SpilGames.Unity.Utils.UnityEditor.Responses
{
	public class PackagesData : Data
	{

		public static List<PackageData> GamePackagesData;
		public static List<PromotionData> GamePromotionData;

		public static void ProcessPackagesData(ResponseEvent response){
			if(response.data != null){
				if(response.data.HasField("packages")){
					JSONObject json = new JSONObject();
					json.AddField("data", response.data.GetField("packages").Print(false));
					GamePackagesData = JsonHelper.getObjectFromJson<List<PackageData>>(json.GetField("data").str);
				}

				if(response.data.HasField("promotions")){
					JSONObject json = new JSONObject();
					json.AddField("data", response.data.GetField("promotions").Print(false));
					GamePromotionData = JsonHelper.getObjectFromJson<List<PromotionData>>(json.GetField("data").str);
				}

			}
		}

		public static string getPackage(string packageId){
			for(int i = 0; i < GamePackagesData.Count; i++){
				if(GamePackagesData[i].packageId.Equals(packageId)){
					return JsonHelper.getJSONFromObject(GamePackagesData[i]);
				}
			}

			return null;
		}

		public static string getPromotion(string packageId){
			for(int i = 0; i < GamePromotionData.Count; i++){
				if(GamePromotionData[i].packageId.Equals(packageId)){
					return JsonHelper.getJSONFromObject(GamePromotionData[i]);
				}
			}

			return null;
		}

	}
}
#endif
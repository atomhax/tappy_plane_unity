#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using SpilGames.Unity.Implementations;
using SpilGames.Unity.Helpers;
using SpilGames.Unity.Utils.UnityEditor;

namespace SpilGames.Unity.Utils.UnityEditor.Responses
{
	public class AdvertisementData : Data
	{
		#if UNITY_EDITOR

		public static bool DFPEnabled = false;
		public static bool FyberEnabled = false;
		public static bool ChartboostEnabled = false;

		public static void ProcessAdvertisement (ResponseEvent response)
		{

			if (response.action.Equals ("init")) {
				JSONObject provider = response.data.GetField ("providers");

				if (provider.HasField ("DFP")) {
					DFPEnabled = true;
					Debug.Log ("DFP Enabled");
				}

				if (provider.HasField ("Fyber")) {
					FyberEnabled = true;
					Debug.Log ("Fyber Enabled");
				}

				if (provider.HasField ("Chartboost") || provider.HasField ("ChartBoost")) {
					ChartboostEnabled = true;
					Debug.Log ("Chartboost Enabled");
				}
			} else if (response.action.Equals ("show")) {
				string provider = response.data.GetField ("provider").str;
				string adType = response.data.GetField("adType").str;

				int probability = Random.Range(0, 100);
				bool available = false;

				if(probability > 20){
					available = true;
				}

				if (provider.ToLower ().Trim ().Equals ("dfp")) {
					if(available){
						Debug.Log ("DFP Show");
						SpilUnityImplementationBase.fireAdAvailableEvent (adType);
						PlayInterstitial("DFP");
					} else {
						Debug.Log ("DFP Not Available");
						SpilUnityImplementationBase.fireAdNotAvailableEvent (adType);
					}

				} else if (provider.ToLower ().Trim ().Equals ("fyber")) {

					if(available){
						Debug.Log ("Fyber Show");
						SpilUnityImplementationBase.fireAdAvailableEvent (adType);
					} else {
						Debug.Log ("Fyber Not Available");
						SpilUnityImplementationBase.fireAdNotAvailableEvent (adType);
					}


				} else if (provider.ToLower ().Trim ().Equals ("chartboost")) {
					if(available){
						Debug.Log ("Chartboost Show");
						SpilUnityImplementationBase.fireAdAvailableEvent (adType);
						PlayInterstitial("Chartboost");
					} else {
						Debug.Log ("Chartboost Not Available");
						SpilUnityImplementationBase.fireAdNotAvailableEvent (adType);
					}
				}
			}
		}

		public static void PlayVideo ()
		{
			GameObject overlay = GameObject.CreatePrimitive (PrimitiveType.Cube);
			AdOverlay ad = overlay.AddComponent<AdOverlay> ();
			ad.provider = "Fyber";
			ad.adType = "rewardVideo";
			SpilUnityImplementationBase.fireAdStartedEvent();
		}

		public static void PlayInterstitial (string provider)
		{
			GameObject overlay = GameObject.CreatePrimitive (PrimitiveType.Cube);
			AdOverlay ad = overlay.AddComponent<AdOverlay> ();
			ad.provider = provider;
			ad.adType = "interstitial";
			SpilUnityImplementationBase.fireAdStartedEvent();
		}

		public static void PlayMoreApps(){
			GameObject overlay = GameObject.CreatePrimitive (PrimitiveType.Cube);
			AdOverlay ad = overlay.AddComponent<AdOverlay> ();
			ad.provider = provider;
			ad.adType = "moreApps";
			SpilUnityImplementationBase.fireAdStartedEvent();
		}

		public class AdOverlay : MonoBehaviour
		{
			public string provider;
			public string adType;

			void OnGUI ()
			{	
				if (GUI.Button (new Rect (10, 10, (Screen.width-20), (Screen.height-20)), provider + " " + adType + " is playing!")){
					AdFinished adFinished = new AdFinished();

					adFinished.network = provider;
					adFinished.type = adType;
					adFinished.reason = "close";

					if(adType.ToLower().Trim().Equals("rewardvideo")){
						adFinished.reward = new Reward();

						if(Spil.CurrencyName != null){
							adFinished.reward.currencyName = Spil.CurrencyName;
						}

						if(Spil.CurrencyId != null){
							adFinished.reward.currencyId = Spil.CurrencyId;
						}

						if(Spil.Reward != 0){
							adFinished.reward.reward = Spil.Reward;
						}

					}

					SpilUnityImplementationBase.fireAdFinishedEvent(JsonHelper.getJSONFromObject(adFinished));
					GameObject.Destroy(this.gameObject);
				}
					
			}
		}

		public class AdFinished{
			public string network;
			public string type;
			public string reason;
			public Reward reward;
		}

		public class Reward{
			public string currencyName;
			public string currencyId;
			public int reward;
		}
		#endif
	}

}
#endif
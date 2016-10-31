/*
 * Spil Games Unity SDK 2016
 * Version 2.2.4
 * 
 * If you have any questions, don't hesitate to e-mail us at info@spilgames.com
 * Be sure to check the github page for documentation and the latest updates
 * https://github.com/spilgames/spil_event_unity_plugin 
*/

using UnityEngine;
using SpilGames.Unity.Implementations;
using SpilGames.Unity.Utils;
using SpilGames.Unity.Helpers;
using System;

namespace SpilGames.Unity
{
	[HelpURL("http://www.spilgames.com/developers/integration/unity/unity-get-started/")]
	public class Spil : MonoBehaviour
	{

		public static SpilGameDataHelper GameData;
		public static PlayerDataHelper PlayerData;

		[Header("Android Settings")]

		#if UNITY_ANDROID || UNITY_EDITOR
		[SerializeField]
		private string projectId;

		/// <summary>
		/// The project ID assigned to your project by Spil games.
		/// You can get your project id from your contact person.
		/// The project ID is used for push notifications.
		/// </summary>
		public static string Project_ID { get; private set; }
		#endif

		[Header("iOS Settings")]
		[SerializeField]
		public string CustomBundleId;

		[Header("Editor Settings")]

		[SerializeField]
		private string spilUserIdEditor;
		public static string SpilUserIdEditor { get; private set; }

		[SerializeField]
		private string bundleIdEditor;
		public static string BundleIdEditor { get; private set; }

		[Header("Reward Video Settings")]

		[SerializeField]
		private string currencyName;
		public static string CurrencyName { get; private set; }

		[SerializeField]
		private string currencyId;
		public static string CurrencyId { get; private set; }

		[SerializeField]
		private int reward = 0;
		public static int Reward { get; private set; }
	

		[Header("Daily Bonus Settings")]

		[SerializeField]
		private int dbId;
		public static int DBId { get; private set; }

		[SerializeField]
		private string dbExternalId;
		public static string DBExternalId { get; private set; }

		[SerializeField]
		private int dbAmount;
		public static int DBAmount { get; private set; }

		public enum DailyBonusRewardType { CURRENCY, ITEM, EXTERNAL };
		public DailyBonusRewardType DBRewardType;

		public static Spil MonoInstance { get { return GameObject.Find ("SpilSDK").GetComponent<Spil> (); } }

		#if UNITY_EDITOR
			
		public static SpilUnityEditorImplementation Instance = new SpilUnityEditorImplementation ();


		#elif UNITY_ANDROID
		
			public static SpilAndroidUnityImplementation Instance = new SpilAndroidUnityImplementation();

		
		#elif UNITY_IPHONE
		
			/// <summary>
			/// Use this object to access all Spil related functionality.
			/// </summary>
			public static SpiliOSUnityImplementation Instance = new SpiliOSUnityImplementation();

			void Update() { Instance.SendNotificationTokenToSpil(); }

		#endif

		void Awake ()
		{
			#if UNITY_EDITOR
			if (String.IsNullOrEmpty (spilUserIdEditor)) {
				spilUserIdEditor = Guid.NewGuid ().ToString ();
			}
			SpilUserIdEditor = spilUserIdEditor;
			Debug.Log ("SpilSDK-Unity Using SpilUserIdEditor: " + SpilUserIdEditor);

			BundleIdEditor = bundleIdEditor;
			if (String.IsNullOrEmpty (bundleIdEditor)) {
				Debug.Assert (!String.IsNullOrEmpty (bundleIdEditor), "SpilSDK-Unity No BundleIdEditor set!");
			}

			CurrencyName = currencyName;
			CurrencyId = currencyId;
			Reward = reward;

			DBId = dbId;
			DBExternalId = dbExternalId;
			DBAmount = dbAmount;
			#endif

			#if UNITY_ANDROID || UNITY_EDITOR
			if(projectId != null){
				Project_ID = projectId;
			} else{
				Debug.LogError("Project ID not set!! Please set your Project Id with the id provided by the Spil representative!");
			}
			#endif

			#if UNITY_IOS
			if (!string.IsNullOrEmpty(CustomBundleId)) {
				Instance.SetCustomBundleId(CustomBundleId);
			}
			#endif

			Debug.Log ("SpilSDK-Unity Init");

			Instance.SpilInit ();
			DontDestroyOnLoad (gameObject);
			gameObject.name = "SpilSDK";

			Instance.UpdatePackagesAndPromotions ();

			GameData = new SpilGameDataHelper (Instance);
			PlayerData = new PlayerDataHelper (Instance);
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// Developers can subscribe to the Spil.Instance.AdStarted event.
		/// </summary>
		public void AdStart ()
		{
			SpilUnityImplementationBase.fireAdStartedEvent ();
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// Developers can subscribe to the Spil.Instance.AdFinished event.
		/// </summary>
		public void AdFinished (string response)
		{
			SpilUnityImplementationBase.fireAdFinishedEvent (response);
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// Developers can subscribe to the Spil.Instance.AdAvailable event.
		/// </summary>
		public void AdAvailable (string type)
		{
			SpilUnityImplementationBase.fireAdAvailableEvent (type);            
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// Developers can subscribe to the Spil.Instance.AdNotAvailable event.
		/// </summary>
		public void AdNotAvailable (string type)
		{
			SpilUnityImplementationBase.fireAdNotAvailableEvent (type);
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// Developers can subscribe to the Spil.Instance.ConfigUpdated event.
		/// </summary>
		public void ConfigUpdated ()
		{
			SpilUnityImplementationBase.fireConfigUpdatedEvent ();
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// </summary>
		public void OnResponseReceived (string response)
		{
			SpilUnityImplementationBase.OnResponseReceived (response);
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// </summary>
		public void SpilGameDataAvailable ()
		{
			SpilUnityImplementationBase.fireSpilGameDataAvailable ();
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// </summary>
		public void SpilGameDataError (string reason)
		{
			SpilUnityImplementationBase.fireSpilGameDataError (reason);
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// </summary>
		public void PlayerDataAvailable ()
		{
			SpilUnityImplementationBase.firePlayerDataAvailable ();
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// </summary>
		public void PlayerDataUpdated (string data)
		{
			SpilUnityImplementationBase.firePlayerDataUpdated (data);
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// </summary>
		public void PlayerDataError (string reason)
		{
			SpilUnityImplementationBase.firePlayerDataError (reason);
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// </summary>
		public void GameStateUpdated (string access)
		{
			SpilUnityImplementationBase.fireGameStateUpdated (access);
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// </summary>
		public void OtherUsersGameStateLoaded (string message)
		{
			SpilUnityImplementationBase.fireOtherUsersGameStateLoaded (message);
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// </summary>
		public void GameStateError (string message)
		{
			SpilUnityImplementationBase.fireGameStateError (message);
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// </summary>
		public void SplashScreenOpen ()
		{
			SpilUnityImplementationBase.fireSplashScreenOpen ();
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// </summary>
		public void SplashScreenNotAvailable ()
		{
			SpilUnityImplementationBase.fireSplashScreenNotAvailable ();
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// </summary>
		public void SplashScreenClosed ()
		{
			SpilUnityImplementationBase.fireSplashScreenClosed ();
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// </summary>
		public void SplashScreenOpenShop ()
		{
			SpilUnityImplementationBase.fireSplashScreenOpenShop ();
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// </summary>
		public void SplashScreenError (string error)
		{
			SpilUnityImplementationBase.fireSplashScreenError (error);
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// </summary>
		public void DailyBonusOpen ()
		{
			SpilUnityImplementationBase.fireDailyBonusOpen ();
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// </summary>
		public void DailyBonusNotAvailable ()
		{
			SpilUnityImplementationBase.fireDailyBonusNotAvailable ();
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// </summary>
		public void DailyBonusClosed ()
		{
			SpilUnityImplementationBase.fireDailyBonusClosed ();
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// </summary>
		public void DailyBonusReward (string reward)
		{
			SpilUnityImplementationBase.fireDailyBonusReward (reward);
		}

		/// <summary>
		/// This method is called by the native Spil SDK, it should not be used by developers.
		/// </summary>
		public void DailyBonusError (string error)
		{
			SpilUnityImplementationBase.fireDailyBonusError (error);
		}
	}
}
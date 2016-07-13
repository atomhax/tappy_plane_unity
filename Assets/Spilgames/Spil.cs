/*
 * Spil Games Unity SDK 2016
 * Version v2.0.3

 * 
 * If you have any questions, don't hesitate to e-mail us at info@spilgames.com
 * Be sure to check the github page for documentation and the latest updates
 * https://github.com/spilgames/spil_event_unity_plugin 
*/

using UnityEngine;
using SpilGames.Unity.Implementations;
using SpilGames.Unity.Utils;
using SpilGames.Unity.Helpers;

namespace SpilGames.Unity
{ 
    public class Spil : MonoBehaviour
    {
        #region Example code

            // Here's some example methods to show what can be done

            // Note: Be sure to include a defaultGameConfig.json file with your chartboost
            // id and signature in the /StreamingAssets directory. ChartBoost videos and 
            // more apps screens won't work without them. Instructions for creating a
            // defaultGameConfig.json are included in the SpilSDK documentation.

            // Call this method before showing ads, for instance in Awake()
            void AttachListeners()
            {
                // Make sure that any existing handlers are removed and add new ones

                // When we've requested an ad, if it is available it will call OnAdAvailable
                Spil.Instance.OnAdAvailable -= AdAvailableHandler;
                Spil.Instance.OnAdAvailable += AdAvailableHandler;

                // When we've requested an ad, if it is not available it will call OnAdNotAvailable
                Spil.Instance.OnAdNotAvailable -= AdNotAvailableHandler;
                Spil.Instance.OnAdNotAvailable += AdNotAvailableHandler;

                // When an ad starts it will first call OnAdStarted so music can be muted etc
                Spil.Instance.OnAdStarted -= AdStartedHandler;
                Spil.Instance.OnAdStarted += AdStartedHandler;

                // When an ad finishes or is dismissed it will call OnAdFinished so music can be re-enabled etc
                Spil.Instance.OnAdFinished -= AdFinishedHandler;
                Spil.Instance.OnAdFinished += AdFinishedHandler;			
				
				Spil.Instance.OnPlayerDataUpdated -= PlayerDataUpdatedHandler;
				Spil.Instance.OnPlayerDataUpdated += PlayerDataUpdatedHandler;

				Spil.Instance.OnPlayerDataAvailable -= PlayerDataAvailableHandler;
				Spil.Instance.OnPlayerDataAvailable += PlayerDataAvailableHandler;

				Spil.Instance.OnSpilGameDataAvailable -= SpilGameDataAvailableHandler;
				Spil.Instance.OnSpilGameDataAvailable += SpilGameDataAvailableHandler;

				Spil.Instance.OnPlayerDataError -= PlayerDataErrorHandler;
				Spil.Instance.OnPlayerDataError += PlayerDataErrorHandler;

				Spil.Instance.OnSpilGameDataError -= SpilGameDataErrorHandler;
				Spil.Instance.OnSpilGameDataError += SpilGameDataErrorHandler;
            }
            
            // Make sure you've called AttachListeners() before calling this method
            public void RequestRewardVideo()
            {
                // Because of all the event handlers we've attached using AttachListeners() this will cause an infinite 
                // loop of reward video's! Every time you close/dismiss the video AdFinishedHandler is called, which 
                // requests a new video, which calls AdAvailableHandler, which plays the video and so on.
                // So this is for testing only, easy to adapt for your own purposes though.
                Spil.Instance.SendrequestRewardVideoEvent();
            }

            // Be sure to call AttachListeners() before calling this method
            public void RequestMoreApps()
            {
                // Because of all the event handlers we've attached using AttachListeners() this will cause an infinite 
                // loop of more apps! Every time you close/dismiss the more apps screen AdFinishedHandler is called, which 
                // requests a new video, which should call AdAvailableHandler, which shows the more apps menu and so on.
                // So this is for testing only, easy to adapt for your own purposes though.
                Spil.Instance.RequestMoreApps();
            }

            // When an ad is available it can be shown
            void AdAvailableHandler(enumAdType adType)
            {
                if (adType == enumAdType.RewardVideo)
                {
                    Spil.Instance.PlayVideo();
                }
                else if(adType == enumAdType.MoreApps)
                {
                    Spil.Instance.PlayMoreApps();
                }

                // Interstitials aren't played on command but are automatically played by the SpilSDK 
                // when certain events are fired such as "levelComplete" or "playerDies".
                // Which events trigger interstitials is configured by Spil.
                // Interstitials do trigger OnAdStarted and OnAdFinished events when they play.
            }

            // When an ad is not available the UI may have to be updated,
            // for instance to hide a button.
            void AdNotAvailableHandler(enumAdType adType)
            {
                Debug.Log("Ad was not available");
            }

            void AdStartedHandler()
            {
                // Mute the game music etc.
            }

            void AdFinishedHandler(SpilAdFinishedResponse response)
            {
                // Re-enable the game music etc.

                // When an ad finishes we can immediately request a new one
                if (response.GetTypeAsEnum() == enumAdType.RewardVideo)
                {
                    Spil.Instance.SendrequestRewardVideoEvent();
                }
                if (response.GetTypeAsEnum() == enumAdType.MoreApps)
                {
                    Spil.Instance.RequestMoreApps();
                }

                // Reward video's may also send a reward in the response if the video wasn't dismissed
		        if (response.reward != null)
                {
			        int rewardAmount = response.reward.reward;
			        Debug.Log ("Rewarded " + rewardAmount + (response.reward.currencyName != null ? " " + response.reward.currencyName : " credits"));
		        }
            }
			
            private void PlayerDataUpdatedHandler(string reason)
			{
			}

			private void PlayerDataErrorHandler(SpilErrorMessage errorMessage)
			{			
			}

			private void SpilGameDataErrorHandler(SpilErrorMessage errorMessage)
			{
			}

			private void PlayerDataAvailableHandler()
			{
			}

			private void SpilGameDataAvailableHandler()
			{	
			}			
			

        #endregion

        #region SpilSDK code

            /// <summary>
            /// The project ID assigned to your project by Spil games.
            /// You can get your project id from your contact person.
            /// The project ID is used for push notifications.
            /// </summary>
            public static string Project_ID
            {
				get { return "492493592102"; }
            }

			public static SpilGameDataHelper SpilGameDataInstance;
			public static PlayerDataHelper SpilPlayerDataInstance;

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

            void Awake()
            {
                Debug.Log("SpilSDK-Unity Init");

				Instance.SpilInit(true);
                DontDestroyOnLoad(gameObject);
                gameObject.name = "SpilSDK";

                Instance.UpdatePackagesAndPromotions();

				SpilGameDataInstance = new SpilGameDataHelper(Instance);
				SpilPlayerDataInstance = new PlayerDataHelper(Instance);

				//AttachListeners ();

            }

            /// <summary>
            /// This method is called by the native Spil SDK, it should not be used by developers.
            /// Developers can subscribe to the Spil.Instance.AdStarted event.
            /// </summary>
            public void AdStart()
            {
                SpilUnityImplementationBase.fireAdStartedEvent();
            }

            /// <summary>
            /// This method is called by the native Spil SDK, it should not be used by developers.
            /// Developers can subscribe to the Spil.Instance.AdFinished event.
            /// </summary>
            public void AdFinished(string response)
            {
                SpilUnityImplementationBase.fireAdFinishedEvent(response);
            }

            /// <summary>
            /// This method is called by the native Spil SDK, it should not be used by developers.
            /// Developers can subscribe to the Spil.Instance.AdAvailable event.
            /// </summary>
            public void AdAvailable(string type)
            {
                SpilUnityImplementationBase.fireAdAvailableEvent(type);            
            }

            /// <summary>
            /// This method is called by the native Spil SDK, it should not be used by developers.
            /// Developers can subscribe to the Spil.Instance.AdNotAvailable event.
            /// </summary>
            public void AdNotAvailable(string type)
            {
                SpilUnityImplementationBase.fireAdNotAvailableEvent(type);
            }

            /// <summary>
            /// This method is called by the native Spil SDK, it should not be used by developers.
            /// </summary>
            public void OnResponseReceived(string response)
            {
                SpilUnityImplementationBase.OnResponseReceived(response);
            }
            
			/// <summary>
			/// This method is called by the native Spil SDK, it should not be used by developers.
			/// </summary>
			public void SpilGameDataAvailable()
			{
				SpilUnityImplementationBase.fireSpilGameDataAvailable();
			}
			
			/// <summary>
			/// This method is called by the native Spil SDK, it should not be used by developers.
			/// </summary>
			public void SpilGameDataError(string reason)
			{
				SpilUnityImplementationBase.fireSpilGameDataError(reason);
			}
			
			/// <summary>
			/// This method is called by the native Spil SDK, it should not be used by developers.
			/// </summary>
			public void PlayerDataAvailable()
			{
				SpilUnityImplementationBase.firePlayerDataAvailable();
			}
			
			/// <summary>
			/// This method is called by the native Spil SDK, it should not be used by developers.
			/// </summary>
			public void PlayerDataUpdated(string reason)
			{
				SpilUnityImplementationBase.firePlayerDataUpdated(reason);
			}
			
			/// <summary>
			/// This method is called by the native Spil SDK, it should not be used by developers.
			/// </summary>
			public void PlayerDataError(string reason)
			{
				SpilUnityImplementationBase.firePlayerDataError(reason);
			}
			
        #endregion
    }
}
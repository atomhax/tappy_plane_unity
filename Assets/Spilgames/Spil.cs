/*
 * Spil Games Unity SDK 2016
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
        #region Example code for the developer
                   
            void AttachListeners()
            {
                // Make sure that any existing handlers are removed and add new ones

                Instance.OnAdAvailable -= AdAvailableHandler;
                Instance.OnAdAvailable += AdAvailableHandler;

                Instance.OnAdNotAvailable -= AdNotAvailableHandler;
                Instance.OnAdNotAvailable += AdNotAvailableHandler;

				Instance.OnSpilGameDataAvailable -= SpilGameDataAvailableHandler;
				Instance.OnSpilGameDataAvailable += SpilGameDataAvailableHandler;

            }

            private void AdAvailableHandler(enumAdType adType)
            {
                Instance.PlayVideo();
            }

            private void AdNotAvailableHandler(enumAdType adType)
            {
                Debug.Log("PixelWizard AdNotAvailable");
            }

			private void SpilGameDataAvailableHandler()
			{
				Debug.Log("SpilGameData Available");
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
                get { return "127433475057"; }
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

                Instance.SpilInit();
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
			public void PlayerDataUpdated()
			{
				SpilUnityImplementationBase.firePlayerDataUpdated();
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
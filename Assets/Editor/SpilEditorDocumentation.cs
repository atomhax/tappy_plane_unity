using UnityEngine;
using System.Collections;
using UnityEditor;

public class SpilEditorDocumentation : EditorWindow
{

	private int tabSelected = 0;

	[MenuItem ("Spil SDK/Documentation", false, 1)]
	static void Init () {
		SpilEditorDocumentation window = (SpilEditorDocumentation)EditorWindow.GetWindow (typeof(SpilEditorDocumentation));
		window.autoRepaintOnSceneChange = true;
		window.titleContent.text = "Documentation";
		window.Show ();
	}

	void OnGUI(){
		GUILayout.BeginArea(new Rect(0, 10, 200, position.height));
		GUILayout.BeginVertical(); {
			{
				if (GUILayout.Toggle (tabSelected == 0, "General", EditorStyles.toolbarButton)) {
					tabSelected = 0;
				}
				if (GUILayout.Toggle (tabSelected == 1, "Event Tracking", EditorStyles.toolbarButton)) {
					tabSelected = 1;
				}
				if (GUILayout.Toggle (tabSelected == 2, "Game Config", EditorStyles.toolbarButton)) {
					tabSelected = 2;
				}
				if (GUILayout.Toggle (tabSelected == 3, "Advertisement", EditorStyles.toolbarButton)) {
					tabSelected = 3;
				}
				if (GUILayout.Toggle (tabSelected == 4, "Push Notification", EditorStyles.toolbarButton)) {
					tabSelected = 4;
				}
				if (GUILayout.Toggle (tabSelected == 5, "User ID", EditorStyles.toolbarButton)) {
					tabSelected = 5;
				}
				if (GUILayout.Toggle (tabSelected == 6, "Wallet, Shop & Inventory", EditorStyles.toolbarButton)) {
					tabSelected = 6;
				}
				if (GUILayout.Toggle (tabSelected == 7, "IAP Packages and promotions", EditorStyles.toolbarButton)) {
					tabSelected = 7;
				}
				if (GUILayout.Toggle (tabSelected == 8, "Customer support", EditorStyles.toolbarButton)) {
					tabSelected = 8;
				}
				if (GUILayout.Toggle (tabSelected == 9, "Game State", EditorStyles.toolbarButton)) {
					tabSelected = 9;
				}
			}
		GUILayout.EndVertical();
		GUILayout.EndArea();

		GUILayout.BeginArea(new Rect(210, 10, position.width - 210, position.height));
		GUILayout.BeginVertical();
		EditorStyles.label.wordWrap = true;
			switch (tabSelected) {
				case 0:
					DrawGeneral ();
					break;
				case 1:
					DrawEventTracking ();
					break;
				case 2:
					DrawGameConfig ();
					break;
				case 3:
					DrawAdvertisement ();
					break;
				case 4:
					DrawPushNotifications ();
					break;
				case 5:
					DrawUserId ();
					break;
				case 6:
					DrawWalletShopInventory ();
					break;
				case 7:
					DrawIAPPackages ();
					break;
				case 8:
					DrawCustomerSupport ();
					break;
				case 9:
					DrawGameState ();
					break;
			}
		GUILayout.EndVertical();
		GUILayout.EndArea();

		}
	}

	private void DrawGeneral(){
		GUILayout.Label("Introduction", EditorStyles.boldLabel);
		GUILayout.Label("The Spil Game Unity Plugin contains various features that let Spil Games get detailed user behavior insights, get the highest ad revenues and tweak your game. The Unity plugin helps developers to integrate all required SDK’s as quick as possible with minimum effort.",  EditorStyles.wordWrappedLabel);
		GUILayout.Label("");
		GUILayout.Label("You can find more general information about the Spil SDK here:",  EditorStyles.wordWrappedLabel);
		if (GUILayout.Button ("Implementing The Spil SDK", GUILayout.Width (400))) {
			Application.OpenURL("http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/");
		}
	}

	private void DrawEventTracking(){
		GUILayout.Label("Event tracking is an integral part of the Spil SDK. Using event tracking helps us get insight into various user behaviours and provide better experiences for the user. The SDK comes with a default list of events that can be tracked and if you need more please discuss it with our Spil Games representative.",  EditorStyles.wordWrappedLabel);
		GUILayout.Label("");
		GUILayout.Label("You can find more event tracking information here:",  EditorStyles.wordWrappedLabel);
		if (GUILayout.Button ("Spil SDK Event tracking", GUILayout.Width (400))) {
			Application.OpenURL("http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/");
		}
	}

	private void DrawGameConfig(){
		GUILayout.Label("The Spil SDK provides methods to request a game config file (JSON) from the Spil backend. Contact your Spil Games contact person to discuss which features in your game should be configurable via a game config file.",  EditorStyles.wordWrappedLabel);
		GUILayout.Label("");
		GUILayout.Label("You can find more game config information here:",  EditorStyles.wordWrappedLabel);
		if (GUILayout.Button ("Spil SDK Game Config", GUILayout.Width (400))) {
			Application.OpenURL("http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-game-config/");
		}
	}

	private void DrawAdvertisement(){
		GUILayout.Label("The Spil SDK gives you the possiblity to implement advertisement into your app quick and easy. The SDK supports Interstitials, Reward Videos and More Apps functionalities. For the exact information required to enable the advertising networks please contact your SpilGames representative",  EditorStyles.wordWrappedLabel);
		GUILayout.Label("");
		GUILayout.Label("You can find more advertisement information here:",  EditorStyles.wordWrappedLabel);
		if (GUILayout.Button ("Spil SDK Advertisement", GUILayout.Width (400))) {
			Application.OpenURL("http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-advertisement-2/");
		}
	}

	private void DrawPushNotifications(){
		GUILayout.Label("The Spil SDK automatically asks users for their permission to receive push notifications for iOS. Also the Unity Spil Plugin automatically collects the GCM Id or the ‘device’ token and sends it to the Spil servers. By default, using the Spil SDK, Spil Games can send textual push notification without you having to implement any additional logic.",  EditorStyles.wordWrappedLabel);
		GUILayout.Label("");
		GUILayout.Label("You can find more push notification information here:",  EditorStyles.wordWrappedLabel);
		if (GUILayout.Button ("Spil SDK Advertisement", GUILayout.Width (400))) {
			Application.OpenURL("http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-push-notifications/");
		}
	}

	private void DrawUserId(){
		GUILayout.Label("When a user contacts Spil customer support he/she may be asked for a Spil user id. Spil currently does not require registration for users and so cannot link an email to a Spil user id. All users are essentially guest users and thus have an anonymous user-id.",  EditorStyles.wordWrappedLabel);
		GUILayout.Label("");
		GUILayout.Label("You can find more Spil User Id information here:",  EditorStyles.wordWrappedLabel);
		if (GUILayout.Button ("Spil SDK Advertisement", GUILayout.Width (400))) {
			Application.OpenURL("http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-user-id/");
		}
	}

	private void DrawWalletShopInventory(){
		GUILayout.Label("Wallet: \nThe wallet feature is holding a users virtual balance of a particular currency. E.g. the user has 100 coins in his wallet. A wallet can contain multiple currencies, e.g. coins and diamonds.", EditorStyles.wordWrappedLabel);
		GUILayout.Label("In-game shop: \nA user can buy items or bundles (a pack of multiple items) with his virtual currency. Let’s say a user can buy a sword with 100 coins.", EditorStyles.wordWrappedLabel);
		GUILayout.Label("Inventory: \nWhen a user bought an item in the in-game shop it will be added to his personal inventory.",  EditorStyles.wordWrappedLabel);

		GUILayout.Label("");
		GUILayout.Label("You can find more Wallet, Inventory or Shop information here:",  EditorStyles.wordWrappedLabel);
		if (GUILayout.Button ("Spil SDK Advertisement", GUILayout.Width (400))) {
			Application.OpenURL("http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-wallet-shop-inventory/");
		}
	}

	private void DrawIAPPackages(){
		GUILayout.Label("The Packages and Promotions feature of the Spil SDK (not to be confused with the information contained in the Shop feature) are related to the IAP contained inside your game. The SDK requests automatically the information when the application starts.",  EditorStyles.wordWrappedLabel);
		GUILayout.Label("");
		GUILayout.Label("You can find more IAP Packages information here:",  EditorStyles.wordWrappedLabel);
		if (GUILayout.Button ("Spil SDK Advertisement", GUILayout.Width (400))) {
			Application.OpenURL("http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/sdk-iap-packages-promotions/");
		}
	}

	private void DrawCustomerSupport(){
		GUILayout.Label("Zendesk is integrated within the Spil SDK and can be used to offer customer support features.",  EditorStyles.wordWrappedLabel);
		GUILayout.Label("");
		GUILayout.Label("You can find more Customer Support integration information here:",  EditorStyles.wordWrappedLabel);
		if (GUILayout.Button ("Spil SDK Advertisement", GUILayout.Width (400))) {
			Application.OpenURL("http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-customer-support/");
		}
	}

	private void DrawGameState(){
		GUILayout.Label("The Spil SDK also allows the saving of custom data blobs, e.g. game states, that can be associated to a specific user. These game states come in two flavors, public and private. Private game states can be saved at any time, and that game state will be associated with the Spil guest user id. If you plan on saving a public game state, then you are required to provide a custom user id and provider.",  EditorStyles.wordWrappedLabel);
		GUILayout.Label("");
		GUILayout.Label("You can find more Game State information here:",  EditorStyles.wordWrappedLabel);
		if (GUILayout.Button ("Spil SDK Advertisement", GUILayout.Width (400))) {
			Application.OpenURL("http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-game-state/");
		}
	}

}


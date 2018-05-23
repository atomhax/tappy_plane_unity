using UnityEngine;
using System.Collections;
using SpilGames.Unity;
using SpilGames.Unity.Helpers.GameData;
using UnityEngine.UI;
public class TabController : MonoBehaviour {

	public Text tabTitle;

	public Transform listOfEntryButtonsParent;
	
	public GameObject entryButtonPrefab;

	public BundleDisplayPanelController bundleDisplayPanel;

	public void SetupTab(ShopPanelController shopPanelController, Tab tab){
		tabTitle.text = tab.Name;
		foreach(Entry entry in tab.Entries){
			if (entry.Id == 100405) {
				
			}
			
			bool hasItem = false;
			if (entry.Type.Equals("BUNDLE")) {
				Bundle bundle = Spil.GameData.GetBundle (entry.Id);
				for (int i = 0; i < bundle.Items.Count; i++) {
					Item gacha  = Spil.GameData.GetGacha(bundle.Items[i].Id);
					bool isGacha = false;
					if (gacha != null) {
						isGacha = Spil.GameData.GetGacha(bundle.Items[i].Id).IsGacha;
					}
				
					if(Spil.PlayerData.InventoryHasItem(bundle.Items[i].Id) && !isGacha){
						hasItem = true;
					}
				}	
			}

			if (!hasItem) {
				CreateBundleButton (shopPanelController, entry);
			}
		}
	}
		
	void CreateBundleButton(ShopPanelController shopPanelController, Entry entry){
		if (entry.Id == 100405) {
			if (shopPanelController.gameController.backgroundRuin == null) {
				return;
			}
		}

		if (entry.Id == 100406) {
			if (shopPanelController.gameController.backgroundTown == null) {
				return;
			}
		}
		
		GameObject newButton = (GameObject)Instantiate (entryButtonPrefab);
		newButton.transform.SetParent (listOfEntryButtonsParent);
		newButton.GetComponent<EntryButtonController> ().SetupButton (entry,this);
	}
		
	public void CloseTab(){
		bundleDisplayPanel.gameObject.SetActive(false);
		gameObject.SetActive (false);
		transform.parent.gameObject.SetActive (false);
	}

}

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

	public void SetupTab(Tab tab){
		tabTitle.text = tab.Name;
		foreach(Entry entry in tab.Entries){
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
				CreateBundleButton (entry);
			}
		}
	}
		
	void CreateBundleButton(Entry entry){
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

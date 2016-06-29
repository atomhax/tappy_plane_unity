using UnityEngine;
using System.Collections;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;
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
			Bundle bundle = Spil.SpilGameDataInstance.GetBundle (entry.BundleId);
			for (int i = 0; i < bundle.Items.Count; i++) {
				if(Spil.SpilPlayerDataInstance.InventoryHasItem(bundle.Items[i].Id)){
					hasItem = true;
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
		gameObject.SetActive (false);
		transform.parent.gameObject.SetActive (false);
	}



	public void ShowBundleInfo(Bundle bundle){
		
	}



}

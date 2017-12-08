using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers.GameData;

public class EntryButtonController : MonoBehaviour {

	Bundle bundle;

	public Text buttonLabel;

	public GameObject saleImage;

	TabController parentTabController;

	public void SetupButton(Entry entry, TabController parent){
		parentTabController = parent;
		bundle = Spil.GameData.GetBundle (entry.BundleId);
		buttonLabel.text = entry.Label;

		Promotion promotion = Spil.GameData.GetPromotion(entry.BundleId);
		if (promotion != null) {
			saleImage.SetActive(true);
		}
	}

	public void EntryButtonPressed(){
		parentTabController.bundleDisplayPanel.SetupBundleDisplayPanel (bundle);
	}



}

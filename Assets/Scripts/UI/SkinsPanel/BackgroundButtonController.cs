using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;

public class BackgroundButtonController : MonoBehaviour {

	public SkinSelectPanelController skinSelectPanelController;

	public Image backgroundImage, lockImage;

	public int position;

	public int itemID;

	public bool defaultSkin = false;

	public bool owned = false;

	public Color selectedColor, unselectedColor;

	void OnEnable(){
		SetupButton ();
	}

	public void SetupButton(){
		if (PlayerPrefs.GetInt ("Background", 0) == position) {
			backgroundImage.color = selectedColor;
		} else {
			backgroundImage.color = unselectedColor;
		}

		if(defaultSkin){
			lockImage.enabled = false;
			return;
		}
		if(Spil.SpilPlayerDataInstance.InventoryHasItem(itemID)){
			lockImage.enabled = false;
			owned = true;
		}
			
	}

	public void ButtonClicked(){
		if (owned) {
			PlayerPrefs.SetInt ("Background", position);
			skinSelectPanelController.UpdateButtons ();
		}
	}

}

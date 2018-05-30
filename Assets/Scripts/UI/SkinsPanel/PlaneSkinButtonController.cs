using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;


public class PlaneSkinButtonController : MonoBehaviour {

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

	public void SetupButton() {
		if (PlayerPrefs.GetInt ("Skin", 0) == position) {
			backgroundImage.color = selectedColor;
		} else {
			backgroundImage.color = unselectedColor;
		}

		if(defaultSkin){
			lockImage.enabled = false;
			return;
		}
		
		if(Spil.PlayerData.InventoryHasItem(itemID)){
			lockImage.enabled = false;
			owned = true;
		}
		
		if (position == 3) {
			if (skinSelectPanelController.gameController.goldPlaneController == null) {
				gameObject.SetActive(false);
				return;
			}
		}
	}
		
	public void ButtonClicked(){
		if (owned) {
			PlayerPrefs.SetInt ("Skin", position);
			skinSelectPanelController.UpdateButtons ();
		}
	}


}

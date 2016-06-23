using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;


public class PlaneSkinButtonController : MonoBehaviour {

	public SkinSelectPanelController skinSelectPanelController;

	public Image backgroundImage, planeImage, lockImage;

	public int position;

	public int itemID;

	public bool defaultSkin = false;

	public bool owned = false;

	public Color selectedColor, unselectedColor;

	void OnEnable(){
		SetupButton ();
	}

	public void SetupButton(){
		Debug.Log ("Settingup button with skin: " + PlayerPrefs.GetInt ("Skin", 0).ToString());
		if (PlayerPrefs.GetInt ("Skin", 0) == position) {
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
		Debug.Log ("Button Setup " + gameObject.name);
	}
		
	public void ButtonClicked(){
		Debug.Log ("Pressed");
		if (owned) {
			Debug.Log ("Pressed and owned");
			PlayerPrefs.SetInt ("Skin", position);
			skinSelectPanelController.UpdateButtons ();
		}
	}


}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpilGames.Unity;
using SpilGames.Unity.Utils;
using SpilGames.Unity.Helpers;

public class SkinSelectPanelController : MonoBehaviour {

	public BackgroundButtonController[] backgroundButtons;

	public PlaneSkinButtonController[] skinButtons;

	public GameController gameController;

	public void UpdateButtons(){
		for (int i = 0; i < backgroundButtons.Length; i++) {
			backgroundButtons [i].SetupButton ();
		}
		for (int i = 0; i < skinButtons.Length; i++) {
			skinButtons [i].SetupButton ();
		}
	}

	public void CloseSkinsPanel(){
		gameController.UpdateSkins ();
		SavePrivateGameState();
		gameObject.SetActive (false);
	}

	public void SavePrivateGameState(){
		int backgroundId = PlayerPrefs.GetInt("Background",0);
		int skinId = PlayerPrefs.GetInt("Skin",0);

		PrivateGameState gameState = new PrivateGameState();
		gameState.setBackground(backgroundId);
		gameState.setSkin(skinId);

		string gameStateJson = JsonHelper.getJSONFromObject(gameState);
		Spil.Instance.SetPrivateGameState(gameStateJson);
	}

}

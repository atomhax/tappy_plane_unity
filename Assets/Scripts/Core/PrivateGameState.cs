using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrivateGameState {


	public int Background;
	public int Skin;

	public void setBackground(int backgroundId){
		this.Background = backgroundId;
	}

	public int getBackground(){
		return Background;
	}

	public void setSkin(int skingId){
		this.Skin = skingId;
	}

	public int getSkin(){
		return Skin;
	}

}

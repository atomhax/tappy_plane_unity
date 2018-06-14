using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrivateGameState {


	public int Background;
	public int Skin;
	public float Speed = 1f;

	public void setBackground(int backgroundId){
		this.Background = backgroundId;
	}

	public int getBackground(){
		return Background;
	}

	public void setSkin(int skingId){
		this.Skin = skingId;
	}
	
	public void setSpeed(float speed){
		this.Speed = speed;
	}

	public int getSkin(){
		return Skin;
	}
	
	public float getSpeed(){
		return Speed;
	}

}

﻿using UnityEngine;
using System.Collections;

public class StageTrigger_Link : MonoBehaviour {
	public string jumpSceneName;
	public string jumpLabelName;

	public int jumpDir = 0;
	public bool jumpInput = true;
	public float jumpDelayTime = 0.0f;

	public bool save = true;
	public bool sePlay = true;

	Transform playerTrfm;
	PlayerController playerCtrl;

	void Awake(){
		playerTrfm = PlayerController.GetTransform ();
		playerCtrl = playerTrfm.GetComponent<PlayerController> ();
	}

	void OnTriggerEnter2D_PlayerEvent(GameObject go){
		if(!jumpInput){
			Jump();
		}
	}

	public void Jump(){
		if(jumpSceneName==""){
			jumpSceneName = Application.loadedLevelName;
		}

		PlayerController.checkPointEnabled = true;
		PlayerController.checkPointLabelName = jumpLabelName;
		PlayerController.checkPointSceneName = jumpSceneName;
		PlayerController.checkPointHp = PlayerController.nowHp;
		if(save){
			SaveData.SaveGamePlay();
		}

		playerCtrl.ActionMove (0.0f);
		playerCtrl.activeSts = false;

		Invoke ("JumpWork", jumpDelayTime);
	}

	void JumpWork(){
		playerCtrl.activeSts = true;

		if(Application.loadedLevelName == jumpSceneName){
			GameObject[] stageLinkList = GameObject.FindGameObjectsWithTag("EventTrigger");
			foreach(GameObject stageLink in stageLinkList){
				if(stageLink.GetComponent<StageTrigger_CheckPoint>().labelName == jumpLabelName){
					playerTrfm.position = stageLink.transform.position;
					playerCtrl.groundY = playerTrfm.position.y;
					Camera.main.transform.position = new Vector3(playerTrfm.position.x,playerTrfm.position.y,-10.0f);
					break;
				}
			}
		}else{
			PlayerController.startFadeTime = 0.5f;
			Application.LoadLevel(jumpSceneName);
		}
	}
}

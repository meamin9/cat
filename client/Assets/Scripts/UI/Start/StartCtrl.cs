﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCtrl : MonoBehaviour {
	public GameObject startBtn;
	void Awake() {
	}

	// Use this for initialization
	void Start () {
		Game.Net.Instance.RegisterProto<gamedef.SessionConnected> ((msg, ses) => {
			startBtn.SetActive(true);
			Debug.Log("Connected");
			
		});
	}

	public void onClickStartBtn() {
	}
}

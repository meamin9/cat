using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCtrl : MonoBehaviour {
	public GameObject startBtn;

	void Start () {
		Net.Instance.RegisterProto<gamedef.SessionConnected> ((msg, ses) => {
			startBtn.SetActive(true);
			Debug.Log("Connected");
		});
	}

	public void onClickStartBtn() {
	}
}

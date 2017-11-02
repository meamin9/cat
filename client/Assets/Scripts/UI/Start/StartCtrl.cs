using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUI: UICom {
	public GameObject startBtn;

	void Start () {
        Moudle.PlayerReset();
        Net.Instance.EventConnected += () => {
            Debug.Log("connected");
        };
	}

    public void autoLogin() {

    }

	public void onClickStartBtn() {
	}
}

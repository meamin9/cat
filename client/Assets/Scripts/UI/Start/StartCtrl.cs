using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCtrl: MonoBehaviour {
	void Start () {
        Moudle.PlayerReset();
        Net.Instance.EventConnected += () => {
            Debug.Log("connected");
        };
		UIManager.I.Show<LoginUI>(LoginUI.Define);
	}

    public void autoLogin() {

    }

	public void onClickStartBtn() {
	}
}

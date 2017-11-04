using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCtrl: MonoBehaviour {
	void Start () {
        Moudle.PlayerReset();
		UIManager.I.Show<LoginUI>(LoginUI.Define);
	}

    void OnEnable() {
        Net.Instance.EventConnected += handleConnected;
    }

    void OnDisable() {
        Net.Instance.EventConnected -= handleConnected;
    }

    static void handleConnected() {
        Debug.Log("connected");
    }

    public void autoLogin() {

    }

	public void onClickStartBtn() {
	}
}

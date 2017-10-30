using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginUI: MonoBehaviour {
    public GameObject LoginBtn;
    public GameObject AccountInput;
    public GameObject PwdInput;

    void Start () {
        Game.Net.Instance.RegisterProto<gamedef.SessionConnected> ((msg, ses) => {
            startBtn.SetActive(true);
            Debug.Log("Connected");
        });
    }

    public void onClickStartBtn() {
    }
}

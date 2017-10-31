using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginUI: MonoBehaviour {
    public GameObject LoginBtn;
    public GameObject AccountInput;
    public GameObject PwdInput;

    void Start () {
        Net.Instance.RegisterProto<gamedef.SessionConnected> ((msg, ses) => {
            Debug.Log("Connected");
        });
    }

    public void onClickStartBtn() {
    }
}

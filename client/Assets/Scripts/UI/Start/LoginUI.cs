using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI: UICom {
	public static readonly UIDefine Define = new UIDefine();

    public GameObject LoginBtn;
    public InputField AccountInput;
    public InputField PwdInput;

    void Start () {
    }

    public void login() {
        var name = AccountInput.text.Trim();
        var pwd = AccountInput.text.Trim();
        LoginOP.AccountLogin(name, pwd);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI: UICom {
	public static readonly UIDefine Define = new UIDefine();

	public Button LoginBtn;
    public InputField AccountInput;
    public InputField PwdInput;

    void Start () {
		//LoginBtn.onClick.AddListener(Login);
    }

    public void Login() {
        var name = AccountInput.text.Trim();
		var pwd = PwdInput.text.Trim();
		Debug.Log(name + " " + pwd);
        LoginOP.AccountLogin(name, pwd);
    }
}

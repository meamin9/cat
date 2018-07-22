using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Start ctrl.进入开始界面
///// </summary>
//public class StartCtrl: MonoBehaviour {
//	void Start () {
//        Moudle.PlayerReset();
//		UIMgr.Instance.Show<LoginUI>(LoginUI.Define);
//	}

//    void OnEnable() {
//        Net.Instance.EventConnected += handleConnected;
//    }

//    void OnDisable() {
//        Net.Instance.EventConnected -= handleConnected;
//    }

//    static void handleConnected() {
//        Debug.Log("connected");
//		var msg = new proto.Echo();
//		msg.content = "hello";
//		var sid = Net.Instance.Send<proto.Echo>(msg);
//        Net.Instance.AddResponse<proto.SCResponse>(sid, (_, ev) => {
//			Debug.Log("send Echo Resturn");
//		});
//    }

//    public void autoLogin() {

//    }

//	public void onClickStartBtn() {
//	}
//}

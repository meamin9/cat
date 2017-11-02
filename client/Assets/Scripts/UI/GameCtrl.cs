using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 游戏模块控制,这个脚本要在所有脚本组件前加载
public class GameCtrl: MonoBehaviour {

    void Awake() {
        Moudle.Initialize();
    }

    void Start() {
        Net.Instance.P.Start("127.0.0.1:7200");
    }

    void Update() {
        Net.Instance.Poll();
    }
}

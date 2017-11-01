using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 游戏模块控制
public class GameCtrl: MonoBehaviour {

    void Awake() {
        Moudle.Initialize();
    }

    void Start() {
        Moudle.Start();
    }

    void Update() {
        Moudle.Update();
    }
}

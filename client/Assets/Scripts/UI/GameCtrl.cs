using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class GameCtrl: MonoBehaviour {

    void Awake() {
        Moudle.Initialize();
    }

    void Start () {
        Net.Instance.P.Start("127.0.0.1:7200");
    }

    void Update () {
        Net.Instance.Poll();
    }
}

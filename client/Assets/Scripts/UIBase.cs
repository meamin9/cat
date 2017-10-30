using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class UIBase: MonoBehaviour {
    public UIMode Mode;
    List<GameObject> _subUIs;
    public void AddSubUI(GameObject ui) {
        _subUIs.Add(ui);
    }

    public void OnShow() {

    }
}

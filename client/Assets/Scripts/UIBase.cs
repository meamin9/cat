/// <summary>
/// 作为组件添加到prefab上，提供ui prefab基本参数设定
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase: MonoBehaviour {
    public UIDefine Define;
	public string Name;
    List<GameObject> _subUIs;

    public void AddSubUI(GameObject ui) {
        _subUIs.Add(ui);
    }

	public void Refresh() {
	}

    public void OnShow() {
	}

	public void OnHide(){
	}

	public void onDestroy(){
	}
}

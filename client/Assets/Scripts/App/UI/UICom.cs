using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDefine 
{
	public string PrefabPath = "UI/LoginUI";
	public bool HideKill = true;
	public int Id = 0;
}

/// <summary>
/// 作为组件添加到prefab上，提供ui prefab基本参数设定
/// </summary>
public class UICom: MonoBehaviour 
{
    UIDefine _define;
    public UIDefine Define 
    {
        get { return _define; }
    }
}

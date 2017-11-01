/// <summary>
/// 作为组件添加到prefab上，提供ui prefab基本参数设定
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICom: MonoBehaviour {
    UIDefine _define;
    UIDefine Define 
    {
        get { return _define; }
    }

    public override int GetHashCode()
    {
        return _define.Id;
    }
}

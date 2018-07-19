using System;
using UnityEngine;
using Util;

class Asset : Singleton<Asset>
{
    public Asset() {
    }

    public GameObject Load(string name) {
        var path = string.Format("Prefab/{}", name);
        var req = Resources.LoadAsync(path);
        //if (req.)
        return new GameObject();
    }
}

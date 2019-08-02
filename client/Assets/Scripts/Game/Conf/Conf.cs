using System.Collections.Generic;
using UnityEngine;
using AM.Base;
using LitJson;
using System.Collections;

namespace AM.Game
{
    public static class Conf
    {
        public static IEnumerator LoadBaseConfig()
        {
            var t = new TimeDebug();
            yield return AssetMgr.Instance.LoadAsync(SceneConf.AssetPath, SceneConf.Parse);
            t.Print("load config Over");
        }
    }

}

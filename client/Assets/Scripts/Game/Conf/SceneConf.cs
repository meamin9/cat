using System.Collections.Generic;
using UnityEngine;
using AM.Base;
using LitJson;



namespace AM.Game
{
    /// <summary>
    /// 场景配置
    /// </summary>
    public class SceneConf
    {
        public const string AssetPath = "SceneConf.json";
        
        public int Id { get; }
        public int Type { get; }
        public string Name { get; }
        public string Asset { get; }

        public static Dictionary<int, SceneConf> All { get; private set; }
        public static void Parse(Object info)
        {
            All = JsonMapper.ToObject<Dictionary<int, SceneConf>>((info as TextAsset).text);
        }
    }



}

using System.Collections.Generic;
using UnityEngine;
using Base;


namespace Game
{
    /// <summary>
    /// 场景配置
    /// </summary>
    public class SceneEntry
    {
        #region config data
        public int Id;
        public int Type;
        public string Name;
        public string Asset;

        public override int GetHashCode() {
            return Id;
        }
        #endregion


    }



}

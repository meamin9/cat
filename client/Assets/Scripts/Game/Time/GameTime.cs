using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Base;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 对UnityEngine.Time的扩展和封装
    /// </summary>
    public static class GameTime
    {
        public static float timeOffset { get; set; }
        public static float time { get { return Time.time + timeOffset; }}

        public static float realtime { get { return Time.realtimeSinceStartup + timeOffset; } }

        public static void Init()
        {
            timeOffset = System.DateTime.Now.Ticks/10000f - Time.realtimeSinceStartup;
            MonoProxy.Instance.Adapter.update += Update;
        }

        private static void Update()
        {
            Timer.Update();

        }
    }
}

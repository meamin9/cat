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
    public static class TimeExt
    {
        public static float TimeOffset { get; set; }
        public static float GameTime { get { return Time.time + TimeOffset; }}

        public static float Realtime { get { return Time.realtimeSinceStartup + TimeOffset; } }

        public static void Init()
        {
            TimeOffset = System.DateTime.Now.Ticks/10000f - Time.realtimeSinceStartup;
            MonoProxy.Instance.Adapter.update += Update;
        }

        private static void Update()
        {
            Timer.Update();

        }
    }
}

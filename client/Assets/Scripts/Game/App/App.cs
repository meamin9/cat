//using System;
using System.Reflection;
using UnityEngine;
using Automata.Game;

namespace Automata.Game
{
    public class App
    {
        private Random _rand;
    }

    public class Moudle
    {
        static bool inited = false;

        public static void Initialize()
        {
            if (inited)
            {
                return;
            }

            Debug.Log("init Proto message");
            //Net.Instance.Init();
            Debug.Log("init Proto finished");

            Debug.Log("init UIManager");
            //UIMgr.Create();

            inited = true;
        }

        // 角色重置（重等）清掉player相关数据
        public static void PlayerReset()
        {

        }

        static void initProto()
        {
            //Cellnet.SessionEvent.Init(); // Session事件注册
            //Cellnet.MessageMetaSet.StaticInit(Assembly.GetExecutingAssembly(), "proto");
        }
    }

    enum GenderType
    {
        Male = 0,
        Female = 1,
    }

    enum JobType
    {
        None = 0,
    }

}




using System;
using System.Reflection;

namespace Game {
}

public class Moudle
{
    static bool inited = false;

    public static void Initialize() {
        if (inited) {
            return;
        }
        InitProto();
        UIManager.Init();
    }

    public static void Start() {
        Net.Instance.P.Start("127.0.0.1:7200");
    }

    public static void Update () {
        Net.Instance.Poll();
    }

	public static void InitProto() {
		Cellnet.SessionEvent.Init(); // Session事件注册
		Cellnet.MessageMetaSet.StaticInit(Assembly.GetExecutingAssembly(), "proto");
	}
}


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
        initProto();
        UIManager.Init();
    }

    // 角色重置（重等）清掉player相关数据
    public static void PlayerReset() {

    }

    static void initProto() {
		Cellnet.SessionEvent.Init(); // Session事件注册
		Cellnet.MessageMetaSet.StaticInit(Assembly.GetExecutingAssembly(), "proto");
	}
}


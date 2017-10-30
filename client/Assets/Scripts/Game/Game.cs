using System;
using System.Reflection;

namespace Game {
	public class Moudle
	{
        static bool inited = false;

        public static void Initialize() {
            if (inited) {
                return;
            }
            InitProto();
        }

		public static void InitProto() {
			Cellnet.SessionEvent.Init(); // Session事件注册
			Cellnet.MessageMetaSet.StaticInit(Assembly.GetExecutingAssembly(), "proto");
		}
	}
}


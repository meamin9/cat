using System;
using System.Reflection;

namespace Game {
	public class G
	{
		public static void InitProto() {
			var c = Cellnet.SessionEvent.Connected;
			if (c == null) {
			}
			//Cellnet.MessageMetaSet.StaticInit(Assembly.GetExecutingAssembly(), "gamedef");
		}
	}
}


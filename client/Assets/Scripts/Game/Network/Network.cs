using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AM.Base;

// 网络
namespace AM.Game
{
	public static class Network
	{
		static Connector _conn;
		static List<MsgEvent> _recvList = new List<MsgEvent>();
		static Dictionary<UInt16, Action<object>> _protoHandles = new Dictionary<UInt16, Action<object>>();

		public static void Connect() {
			_conn = new Connector("127.0.0.1", 7001);
			Task.Run(_conn.Connect);
			MonoProxy.Instance.Adapter.update += Update;
		}

		public static void Update() {
			if (!_conn.IsReady) {
				return;
			}
			_conn.Session.RecvBox.Poll(ref _recvList);
			var n = _recvList.Count;
			if (n > 0) {
				for (var i = 0; i < n; ++i) {
					var evt = _recvList[i];
					try {
						_protoHandles[evt.MsgId](evt.Msg);
					}
					finally {
						Log.Error("协议处理出错：{}", evt.MsgId);
					}
				}
				_recvList.Clear();
			}
		}

		public static void RegProto<T>(Action<T> handle) where T: class {
			var meta = MsgMetaSet.MetaByType(typeof(T));
			if (meta.IsEmpty()) {
				Log.Error("unknown protocol: {}", typeof(T));
				return;
			}
			if (_protoHandles.ContainsKey(meta.MsgId)) {
				Log.Error("proto already registed! :{}", meta.MsgId);
				return;
			}
			_protoHandles[meta.MsgId] = (Action<object>)handle;
		}

		public static void Send(object msg)
		{
			_conn.Session.Send(new SendEvent(msg));
		}
	}
}


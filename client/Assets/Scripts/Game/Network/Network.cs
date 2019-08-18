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

        // 使用类似请求响应的方式发服务端，不加超时处理了
        static uint _sid; 
        static Dictionary<uint, Action<object>> _rspHandles = new Dictionary<uint, Action<object>>();

        public static void Connect() {
			_conn = new Connector(GameSetting.Instance.ServerIp, GameSetting.Instance.Port);
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
                    var msg = evt.Msg as Proto.ISession;

					try {
						_protoHandles[evt.MsgId](evt.Msg);
					}
					finally {
						Log.Error("协议处理出错：{0}", evt.MsgId);
					}
				}
				_recvList.Clear();
			}
		}

		public static void RegProto(UInt16 msgId, Action<object> handle) {
            _protoHandles.Add(msgId, handle);
		}

		public static void Send(UInt16 msgId, object msg)
		{
			_conn.Session.Send(new SendEvent { MsgId=msgId, Msg=msg});
		}

        public static void Send(UInt16 msgId, Proto.ISession msg, Action<object> callback)
        {
            msg.Session = ++_sid;
            if (_rspHandles.ContainsKey(msg.Session)) {
                Log.Error("not get proto respond:msgId={0}", msgId);
            }
            _rspHandles[msg.Session] = callback;
            Send(msgId, msg);
        }
	}
}


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Base;

// 网络
namespace Game {
    public static class Network {
        static Connector _conn;
        static List<MsgEvent> _recvList = new List<MsgEvent>();
        static Dictionary<UInt16, Action<object>> _notifyHandles = new Dictionary<UInt16, Action<object>>();
        // 使用类似请求响应的方式发服务端，不加超时处理了
        static uint _sid;
        static Dictionary<uint, Action<object>> _reqHandles = new Dictionary<uint, Action<object>>();

        public static void Connect() {
            _conn = new Connector(Setting.Game.ServerIp, Setting.Game.Port);
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
                Action<object> cb;
                for (var i = 0; i < n; ++i) {
                    var evt = _recvList[i];
                    try {
                        var msg = evt.Msg as Proto.ISession;
                        if (msg != null) {
                            if (_reqHandles.TryGetValue(msg.Session, out cb)) {
                                cb(msg);
                                continue;
                            } else {
                                Log.Error("没有找到对应的协议回调:{0}", evt.MsgId);
                            }
                        }
                        if (_notifyHandles.TryGetValue(evt.MsgId, out cb)) {
                            cb(msg);
                        }
                    } finally {
                        Log.Error("协议处理出错：{0}", evt.MsgId);
                    }
                }
                _recvList.Clear();
            }
        }

        public static void RegProto(UInt16 msgId, Action<object> handle) {
            _notifyHandles.Add(msgId, handle);
        }

        public static void Send(UInt16 msgId, object msg) {
            _conn.Session.Send(new SendEvent { MsgId = msgId, Msg = msg });
        }

        public static void Send(UInt16 msgId, Proto.ISession msg, Action<object> callback) {
            msg.Session = ++_sid;
            if (_reqHandles.ContainsKey(msg.Session)) {
                Log.Error("not get proto respond:msgId={0}", msgId);
            }
            _reqHandles[msg.Session] = callback;
            Send(msgId, msg);
        }
    }
}


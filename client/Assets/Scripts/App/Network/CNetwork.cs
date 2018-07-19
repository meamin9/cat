using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Util;

// 网络
namespace NSNetwork
{
    // 网络单例
    class CNetwork : Singleton<CNetwork>
    {
        Connector _peer;
        List<MsgEvent> _recvList = new List<MsgEvent>();
        Dictionary<UInt16, Action<object>> _protoHandles = new Dictionary<UInt16, Action<object>>();

        public Session Session {
            get {
                return _peer.Session;
            }
        }

        public Task StartConnect() {
            _peer = new Connector("127.0.0.1", 7001);
            return _peer.Start();
        }

        public void Update() {
            Session.RecvBox.Poll(ref _recvList);
            var n = _recvList.Count;
            for (var i = 0; i < n; ++i) {
                var evt = _recvList[i];
                try {
                    _protoHandles[evt.MsgId](evt.Msg);
                }
                finally {
                    Log.Errorf("协议处理出错：{}", evt.MsgId);
                }
            }
            _recvList.Clear();

        }

        public void RegProto<T>(Action<T> handle) where T: class {
            var meta = MsgMetaSet.MetaByType(typeof(T));
            if (meta.IsEmpty()) {
                Log.Errorf("unknown protocol: {}", typeof(T));
                return;
            }
            if (_protoHandles.ContainsKey(meta.MsgId)) {
                Log.Errorf("proto already registed! :{}", meta.MsgId);
                return;
            }
            _protoHandles[meta.MsgId] = (Action<object>)handle;
        }
    }
}


using Cellnet;
using System;

public class Net {
    #region Singleton
    static readonly Net _instance = new Net();
	public static Net Instance { get { return _instance; } }
    #endregion

    #region Event
    public event Action EventConnected;
    public event Action EventConnectError;
    #endregion

    Peer _peer;
	EventDispatcher _protod;

	public Net() {
		_protod = new EventDispatcher();
		var codec = new ProtobufCodec();
		_peer = new Connector(_protod.Queue, codec);
        // 注册Session事件
        RegisterProto<gamedef.SessionConnected>((msg, ses) => {
            if (EventConnected != null) {
                EventConnected.Invoke();
            }
        });
        RegisterProto<gamedef.SessionConnectError>((msg, ses) => {
            if (EventConnectError != null) {
                EventConnectError.Invoke();
            }
        });
	}


	public Peer P { 
        get {
			return _peer;
		}
	}

	public MessageMeta RegisterProto<T>(Action<T, Session> cb) where T:class {
		return Subscribe.RegisterMessage<T>(_protod, cb);
	}

	public void Poll() {
		P.Queue.Poll();
	}
}


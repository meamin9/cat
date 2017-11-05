using Cellnet;
using System;
using System.Reflection;

public class Net {
    #region Singleton
    static readonly Net _instance = new Net();
	public static Net Instance { get { return _instance; } }
    #endregion

    #region Event
    public event Action EventConnected;
    public event Action EventConnectError;
    #endregion

    Connector _peer;
	EventDispatcher _protod;

	public Net() {
		_protod = new EventDispatcher();
		var codec = new ProtobufCodec();
		_peer = new Connector(_protod.Queue, codec);
	}
	public void Init() {
		// 初始化协议消息和session事件
		SessionEvent.Init();
		MessageMetaSet.StaticInit(Assembly.GetExecutingAssembly(), "proto");
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

    public Connector P { 
        get {
			return _peer;
		}
	}

	public MessageMeta RegisterProto<T>(Action<T, Session> cb) where T:class {
		return Subscribe.RegisterMessage<T>(_protod, cb);
	}

	public void RegisterResponse(UInt16 tag, Action<SessionEvent> cb) {
		_protod.AddSid(tag, cb);
	}

	public void Poll() {
		P.Queue.Poll();
	}

    public UInt16 Send<T>(T msg) where T : class {
        var ret = P.Ses.Send<T>(msg);
        if (ret < 0) {
            UnityEngine.Debug.LogError("send net message error" + msg);
        }
        return ret;
    }
}


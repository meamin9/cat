using Cellnet;
using System;


public class Net {
	static readonly Net _instance = new Net();
	public static Net Instance {
		get {
			return _instance;
		}
	}
	Peer _peer;
	EventDispatcher _protod;

	public Net() {
		_protod = new EventDispatcher();
		var codec = new ProtobufCodec();
		_peer = new Connector(_protod.Queue, codec);
	}

	// public void Connect() {
	// 	var codec = new ProtobufCodec();
	// 	_peer = new Connector(_protod.Queue, codec); // 直接新建一个connect
	// }

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


using System;
using System.Net;
using System.Net.Sockets;

namespace Game
{
	public class Connector
	{
		private string _addr;
		private Int32 _port;
		private TcpClient _tcp;
		private Session _session;
		public bool IsReady { get; private set; }

		public Connector(string addr, Int32 port) {
			_addr = addr;
			_port = port;
			_session = new Session();
		}

		public Session Session => _session;

		public void Connect()
		{
			if (_tcp != null) {
				_tcp.Close();
				_tcp = null;
				_session = new Session();
				IsReady = false;
			}
			int[] connectTime = { 1000, 2000, 4000 };
			for (var i = 0; i < connectTime.Length; ++i) {
				try {
					IPAddress[] ipaddrs = Dns.GetHostAddresses(_addr);
					if (ipaddrs.Length > 0) {
						_tcp = new TcpClient(AddressFamily.InterNetwork);
						_tcp.Connect(ipaddrs, _port);
						IsReady = true;
						_session.RecvBox.Post(MsgEvent.Connected);
						_session.Start(_tcp.GetStream());
						break;
					}
				}
				catch (SocketException e) {
					Log.Error("connect error:{}", e.ErrorCode);
				}
				catch (Exception e){
					Log.Error("connect other error{}", e);
				}
				if (_tcp != null) {
					_tcp.Close();
					_tcp = null;
				}
				System.Threading.Thread.Sleep(connectTime[i]);
			}
		}

	}
}

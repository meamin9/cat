using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Network
{
    class Connector
    {
        private string _addr;
        private Int32 _port;
        private TcpClient _tcp;
        private int _reconTimes = 4;
        private int[] _reconnElpased = {500, 1000, 2000, 4000};
        private Session _session;
        private bool _isRunning = false;

        public Connector(string addr, Int32 port) {
            _addr = addr;
            _port = port;
        }

        public Stream GetStream() {
            return _tcp.GetStream();
        }

        public Session Session {
            get {
                return _session;
            }
        }

        public bool Connect()
        {
            int i = 0; //连接次数=1+重连次数
            for (; i <= _reconTimes; ++i) {
                try {
                    IPAddress[] ipaddrs = Dns.GetHostAddresses(_addr);
                    if (ipaddrs.Length > 0) {
                        _tcp = new TcpClient(AddressFamily.InterNetwork);
                        _tcp.Connect(ipaddrs, _port);
                        break;
                    }
                }
                catch (SocketException e) {
                    Log.Errorf("connect error:{}", e.ErrorCode);
                }
                catch (Exception e){
                    Log.Errorf("connect other error{}", e);
                }
                if (_tcp != null) {
                    _tcp.Close();
                    _tcp = null;
                }
                var slptime = _reconnElpased[i];
                System.Threading.Thread.Sleep(slptime);
            }
            return _tcp != null && _tcp.Connected;
        }

        public async Task Start() {
            var ok = await Task.Run((Func<bool>)Connect);
            if (ok) {
                _session = new Session(this);
                _session.Start();
            }
        }

        public void Close() {
            lock (this) {
                if (! _isRunning) {
                    return;
                }
                _isRunning = false;
            }
            _tcp.GetStream().Close();
            _session.Wait();
            _tcp.Close();
            _tcp = null;
        }
    }
}

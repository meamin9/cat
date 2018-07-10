using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace CNet
{
    class Connector
    {
        private string addr;
        private Int32 port;
        private TcpClient peer;
        private int reconnTimes;
        
        public Connector(string addr) {
            this.SetAddr(addr);
            this.reconnTimes = 3;
        }

        public void SetAddr(string addr)
        {
            var strs = addr.Split(':');
            if (strs.Length == 2) {
                this.addr = strs[0];
                this.port = Convert.ToInt32(strs[1]);
            }
            else if (strs.Length == 1)
            {
                this.addr = strs[0];
            }
            else
            {
                //TODO: log
            }
        }

        public async Task<bool> Connect()
        {
            int i = 0;
            for (; i < this.reconnTimes; ++i) {
                IPAddress[] ipaddrs = await Dns.GetHostAddressesAsync(this.addr);
                if (ipaddrs.Length == 0) {
                    continue;
                }
                this.peer = new TcpClient(AddressFamily.InterNetwork);
                await this.peer.ConnectAsync(ipaddrs, this.port);
                return true;
            }
            return true;

        }

        public void Start() {
            var task = Dns.GetHostAddressesAsync(this.addr);
        }
        public void Stop() { }
    }
}

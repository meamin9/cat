using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace CNet
{
    interface ISession {
        void Start();
        void Close();
        void Send();
    }

    class Session
    {
        private NetworkStream stream;
        Session(NetworkStream stream) {
            this.stream = stream;
        }

        public void Start(string addr)
        {
            //this.stream.ReadAsync()
        }

        public void Send(object msg) {

        }

    }
}

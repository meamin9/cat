using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Google.Protobuf;

namespace CNet
{
    struct NetEvent {
        UInt16 msgId;
        IMessage msg;
    }



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

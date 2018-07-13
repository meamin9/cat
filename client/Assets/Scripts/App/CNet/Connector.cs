using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using System.IO;

namespace CNet
{
    class Connector
    {
        private string addr;
        private Int32 port;
        private TcpClient tcp;
        private int reconnTimes = 4;
        private float[] reconnElpased = {0.5f, 1, 2, 4};

        private Task readTask;

        public Connector(string addr) {
            this.SetAddr(addr);
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
                Log.Errorf("addr error:{}", addr);
            }
        }

        public bool Connect()
        {
            int i = -1; //连接次数=1+重连次数
            for (; i < this.reconnTimes; ++i) {
                try {
                    IPAddress[] ipaddrs = Dns.GetHostAddresses(this.addr);
                    if (ipaddrs.Length > 0) {
                        this.tcp = new TcpClient(AddressFamily.InterNetwork);
                        this.tcp.Connect(ipaddrs, this.port);
                        break;
                    }
                }
                catch (SocketException e) {
                    Log.Errorf("connect error:{}", e.ErrorCode);
                }
                catch (Exception e){
                    Log.Errorf("connect other error{}", e);
                }
                if (this.tcp != null) {
                    this.tcp.Close();
                    this.tcp = null;
                }
            }
            return this.tcp != null && this.tcp.Connected;
        }

        public async Task Start() {
            var ok = await Task.Run(() =>this.Connect());
            if (ok) {
                this.readTask = Task.Run(() => this.ReadPackage(this.tcp.GetStream()));
            }
        }

        public void Send<T>(T msg) where T : class{
            // 发包如果出错了
            // TODO: 这里要保证按顺序写
            Task.Run(() =>
               WritePackage<T>(this.tcp.GetStream(), msg)
            );
        }

        public void Close() {
            //TODO: lock
            if (this.tcp == null) {
                return;
            }
            this.tcp.Close();
            this.tcp.GetStream().Close();
            this.tcp = null;
        }


        struct MsgEvent {
            public IMessage Msg;
            public Int16 MsgId;
            public MsgEvent(IMessage msg, Int16 msgId) {
                this.Msg = msg;
                this.MsgId = msgId;
            }

        }
        // 简单起见，直接读，写死protobuf协议
        const int maxPackageSize = 1024 * 4;
        public void ReadPackage(NetworkStream stream) {
            // 2 bytes len
            var buffer = new byte[2];
            var dataBuff = new byte[maxPackageSize - 2];
            while (true) {
                // header 2
                if (ReadBytes(stream, buffer, buffer.Length) < buffer.Length) {
                    break;
                }
                int sz = (buffer[0] << 8) + buffer[1];
                if (sz < 2 || sz > maxPackageSize) {
                    break;
                }
                // msgId 2
                if (ReadBytes(stream, buffer, buffer.Length) < buffer.Length) {
                    break;
                }
                UInt16 msgId = (UInt16)((buffer[0] << 8) + buffer[1]);
                // data
                if (ReadBytes(stream, dataBuff, dataBuff.Length) < dataBuff.Length) {
                    break;
                }
                var msg = DecodeMsg(msgId, dataBuff);
                if (msg != null) { // 如果收到未注册的协议时，可能会decode失败

                }
            }
            //
        }

        private int ReadBytes(Stream stream, byte[] buffer, int sz) {
            int n = 0;
            while (n < sz) {
                var m = stream.Read(buffer, 0, sz-n);
                if (m == 0) { // EOS
                    break;
                }
                n += m;
            }
            return n;
        }

        private IMessage DecodeMsg(UInt16 msgId, byte[] data) {
            var meta = MsgMetaSet.MetaById(msgId);
            if (meta.MsgId == 0) {
                Log.Errorf("meiyou Found {}", msgId);
                return null;
            }
            var msg = (IMessage)Activator.CreateInstance(meta.MsgType);
            msg.MergeFrom(data);
            return msg;
        }

        public void WritePackage<T>(NetworkStream stream, T msg) where T: class {
            var dataStream = new MemoryStream();
            // 长度占位
            dataStream.WriteByte(0);
            dataStream.WriteByte(0);
            // 大端写入msgid
            var meta = MsgMetaSet.MetaByType(typeof(T));
            UInt16 id = meta.MsgId;
            dataStream.WriteByte((byte)(id >> 8));
            dataStream.WriteByte((byte)id);
            // 数据， 先不做codec
            ((IMessage)msg).WriteTo(dataStream);
            // 大端写入长度
            var len = dataStream.Length - 2;
            if (len > maxPackageSize) {
                Log.Errorf("package is too long {}", len);
                return;
            }
            var buffer = dataStream.GetBuffer();
            UInt16 sz = (UInt16)len;
            buffer[0] = (byte)(sz >> 8);
            buffer[1] = (byte)sz;

            stream.Write(buffer, 0, sz);
        }

        private byte[] EncodeMsg(IMessage msg) {
            var stream = new MemoryStream();
            msg.WriteTo(stream);
            return stream.ToArray();
        }

    }
}

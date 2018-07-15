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
    // 网络事件
    struct MsgEvent
    {
        public object Msg;
        public UInt16 MsgId;
        public MsgEvent(object msg, UInt16 msgId) {
            this.Msg = msg;
            this.MsgId = msgId;
        }
        static public MsgEvent Connected = new MsgEvent(null, 1);
        static public MsgEvent Disconnected = new MsgEvent(null, 2);
    }
    // 发送
    struct SendEvent {
        public object Msg;
        public SendEvent(object msg) {
            this.Msg = msg;
        }

    }

    class Session
    {
        private MailBox<MsgEvent> recvBox = new MailBox<MsgEvent>();
        private MailBox<SendEvent> sendBox = new MailBox<SendEvent>();
        private Stream stream;
        private Task sendTask;
        private Task recvTask;
        const int maxPackageSize = 1024;
        public Session(Stream stream) {
            this.stream = stream;
        }

        public void Start() {
            recvBox.Post(MsgEvent.Connected);
            this.sendTask = Task.Run((Action)WriteProcess);
            this.recvTask = Task.Run((Action)ReadProcess);
        }

        private void ReadProcess() {
            // 2 bytes len
            var buffer = new byte[2];
            var dataBuff = new byte[maxPackageSize - 2];
            while (true) {
                // header 2
                if (Util.ReadBytes(stream, buffer, buffer.Length) < buffer.Length) {
                    break;
                }
                int sz = (buffer[0] << 8) + buffer[1];
                if (sz < 2 || sz > maxPackageSize) {
                    break;
                }
                // msgId 2
                if (Util.ReadBytes(stream, buffer, buffer.Length) < buffer.Length) {
                    break;
                }
                UInt16 msgId = (UInt16)((buffer[0] << 8) + buffer[1]);
                // data
                if (Util.ReadBytes(stream, dataBuff, dataBuff.Length) < dataBuff.Length) {
                    break;
                }
                var msg = Util.DecodeMsg(msgId, dataBuff);
                if (msg != null) { // 如果收到未注册的协议时，可能会decode失败
                    recvBox.Post(new MsgEvent(msg, msgId));
                }
            }
        }

        private void WriteProcess() {
            var list = new List<SendEvent>();
            while (true) {
                sendBox.Peek(ref list);
                var n = list.Count;
                for (var i = 0; i < n; ++i) {
                    var msg = list[i].Msg;
                    var dataStream = new MemoryStream();
                    // 长度占位
                    dataStream.WriteByte(0);
                    dataStream.WriteByte(0);
                    // 大端写入msgid
                    var meta = MsgMetaSet.MetaByType(msg.GetType());
                    UInt16 id = meta.MsgId;
                    dataStream.WriteByte((byte)(id >> 8));
                    dataStream.WriteByte((byte)id);
                    // 数据， 先不做codec
                    Util.EncodeMsg(msg, dataStream);
                    // 大端写入长度
                    var len = dataStream.Length - 2;
                    if (len > maxPackageSize)
                    {
                        Log.Errorf("package is too long {}", len);
                        continue;
                    }
                    var buffer = dataStream.GetBuffer();
                    UInt16 sz = (UInt16)len;
                    buffer[0] = (byte)(sz >> 8);
                    buffer[1] = (byte)sz;
                    stream.Write(buffer, 0, sz);
                }
            }
        }

        public void Send(object msg) {
            sendBox.Post(new SendEvent(msg));
        }

        public void WaitClose() {
            stream.Close();
            sendTask.Wait();
            recvTask.Wait();
        }


    }
}

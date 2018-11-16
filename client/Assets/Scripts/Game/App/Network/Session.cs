using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using NSUnityUtil;
using Automata.Base;

namespace Automata.Game
{
    // 网络事件
    public struct MsgEvent
    {
        public object Msg;
        public UInt16 MsgId;
        public MsgEvent(object msg, UInt16 msgId) {
            Msg = msg;
            MsgId = msgId;
        }
        static public MsgEvent Connected = new MsgEvent(null, 1);
        static public MsgEvent Disconnected = new MsgEvent(null, 2);
    }
    // 发送
    public struct SendEvent {
        public object Msg;
        public SendEvent(object msg) {
            Msg = msg;
        }

    }

    public class Session
    {
        private MailBox<MsgEvent> _recvBox = new MailBox<MsgEvent>();
        private MailBox<SendEvent> _sendBox = new MailBox<SendEvent>();
        private Stream _stream;
        private Connector _peer;
        private Task _sendTask;
        private Task _recvTask;
        const int MAX_PACKAGE_SIZE = 1024;
        public Session(Connector peer) {
            _peer = peer;
            _stream = peer.GetStream();
        }

        public MailBox<MsgEvent> RecvBox {
            get {
                return _recvBox;
            }
        }

        public void Start() {
            _recvBox.Post(MsgEvent.Connected);
            _sendTask = Task.Run((Action)WriteProcess);
            _recvTask = Task.Run((Action)ReadProcess);
        }

        private void ReadProcess() {
            // 2 bytes len
            var buffer = new byte[2];
            var dataBuff = new byte[MAX_PACKAGE_SIZE - 2];
            try
            {
                while (true)
                {
                    // header 2
                    if (Util.ReadBytes(_stream, buffer, buffer.Length) < buffer.Length)
                    {
                        break;
                    }
                    int sz = (buffer[0] << 8) + buffer[1];
                    if (sz < 2 || sz > MAX_PACKAGE_SIZE)
                    {
                        break;
                    }
                    // msgId 2
                    if (Util.ReadBytes(_stream, buffer, buffer.Length) < buffer.Length)
                    {
                        break;
                    }
                    UInt16 msgId = (UInt16)((buffer[0] << 8) + buffer[1]);
                    // data
                    if (Util.ReadBytes(_stream, dataBuff, dataBuff.Length) < dataBuff.Length)
                    {
                        break;
                    }
                    var msg = Util.DecodeMsg(msgId, dataBuff);
                    if (msg != null)
                    { // 如果收到未注册的协议时，可能会decode失败
                        _recvBox.Post(new MsgEvent(msg, msgId));
                    }
                }
            }
            finally {
                _peer.Close();
                _recvBox.Post(MsgEvent.Disconnected);
            }
        }

        private void WriteProcess() {
            var list = new List<SendEvent>();
            try
            {
                while (true)
                {
                    _sendBox.Peek(ref list);
                    var n = list.Count;
                    for (var i = 0; i < n; ++i)
                    {
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
                        if (len > MAX_PACKAGE_SIZE)
                        {
                            Log.Errorf("package is too long {}", len);
                            continue;
                        }
                        var buffer = dataStream.GetBuffer();
                        UInt16 sz = (UInt16)len;
                        buffer[0] = (byte)(sz >> 8);
                        buffer[1] = (byte)sz;
                        _stream.Write(buffer, 0, sz);
                    }
                }
            }
            finally {
                _peer.Close();
            }
        }

        public void Send(object msg) {
            _sendBox.Post(new SendEvent(msg));
        }

        public void Wait() {
            _sendTask.Wait();
            _recvTask.Wait();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;

namespace AM.Game
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
        private LockQueue<MsgEvent> _recvBox = new LockQueue<MsgEvent>();
        private LockQueue<SendEvent> _sendBox = new LockQueue<SendEvent>();
        private NetworkStream _stream;
        private Task _sendTask;
        private Task _recvTask;
        const int HEAD_SIZE = 4;

        public LockQueue<MsgEvent> RecvBox => _recvBox;

        public void Start(NetworkStream stream) {
            _stream = stream;
            _sendTask = Task.Run((Action)WriteProcess);
            _recvTask = Task.Run((Action)ReadProcess);
        }

        private void ReadProcess() {
            var buffSize = 1024;
            var buff = new byte[buffSize];
            try {
                while (true) {
                    // 长度和消息id占 4字节
                    if (_stream.Read(buff, 0, HEAD_SIZE) < HEAD_SIZE) {
                        break;
                    }
                    int dataLength = (buff[0] << 8) + buff[1];
                    UInt16 msgId = (UInt16)((buff[2] << 8) + buff[3]);
                    if (dataLength > buffSize) {
                        buffSize = dataLength - dataLength % 1024 + 1024;
                        buff = new byte[buffSize];
                        Log.Warn($"network warn: buff resized to {buffSize}, msgId={msgId}");
                    }
                    // data
                    if (_stream.Read(buff, 0, dataLength) < dataLength) {
                        break;
                    }
                    var msg = Codec.DecodeMsg(msgId, buff, dataLength);
                    if (msg != null) { // 如果收到未注册的协议时，可能会decode失败
                        _recvBox.Post(new MsgEvent(msg, msgId));
                    }
                    else {
                        Log.Warn($"未知协议{msgId}");
                    }
                }
            }
            finally {
                _recvBox.Post(MsgEvent.Disconnected);
            }
        }

        private void WriteProcess() {
            var list = new List<SendEvent>();
            try
            {
                var stream = new MemoryStream(1024);
                while (true)
                {
                    list.Clear();
                    _sendBox.Peek(ref list);
                    var n = list.Count;
                    for (var i = 0; i < n; ++i)
                    {
                        var msg = list[i].Msg;
                        // 长度占位
                        stream.WriteByte(0);
                        stream.WriteByte(0);
                        // 大端写入msgid
                        var meta = MsgMetaSet.MetaByType(msg.GetType());
                        UInt16 id = meta.MsgId;
                        stream.WriteByte((byte)(id >> 8));
                        stream.WriteByte((byte)id);
                        Codec.EncodeMsg(msg, stream);
                        // 大端写入长度
                        var dataSize = (UInt16)stream.Length - HEAD_SIZE;
                        var buffer = stream.GetBuffer();
                        buffer[0] = (byte)(dataSize >> 8);
                        buffer[1] = (byte)dataSize;
                        _stream.Write(buffer, 0, (int)stream.Length);
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.SetLength(0);
                    }
                }
            }
            finally {
                _recvBox.Post(MsgEvent.Disconnected);
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

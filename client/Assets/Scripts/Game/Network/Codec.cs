using System;
using System.IO;
using Google.Protobuf;

namespace AM.Game
{
    public class Codec
    {
        static public object DecodeMsg(UInt16 msgId, byte[] data, int length)
        {
            var meta = MsgMetaSet.MetaById(msgId);
            if (meta.IsEmpty()) {
                return null;
            }
            var msg = (IMessage)Activator.CreateInstance(meta.MsgType);
            msg.MergeFrom(data, 0, length);
            return msg;
        }

        static public void EncodeMsg(object msg, Stream stream) {
            ((IMessage)msg).WriteTo(stream);
        }
    }
}

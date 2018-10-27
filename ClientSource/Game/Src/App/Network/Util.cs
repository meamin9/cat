using System;
using System.IO;
using Google.Protobuf;

namespace Automata.Game
{
    public class Util
    {
        static public int ReadBytes(Stream stream, byte[] buffer, int sz)
        {
            int n = 0;
            while (n < sz)
            {
                var m = stream.Read(buffer, 0, sz - n);
                if (m == 0)
                { // EOS
                    break;
                }
                n += m;
            }
            return n;
        }

        static public object DecodeMsg(UInt16 msgId, byte[] data)
        {
            var meta = MsgMetaSet.MetaById(msgId);
            if (meta.IsEmpty()) {
                return null;
            }
            // codec
            var msg = (IMessage)Activator.CreateInstance(meta.MsgType);
            msg.MergeFrom(data);
            return msg;
        }

        static public void EncodeMsg(object msg, Stream stream) {
            ((IMessage)msg).WriteTo(stream);
        }
    }
}

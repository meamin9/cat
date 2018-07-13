using System;
using System.Collections.Generic;
using System.IO;
using Google.Protobuf;

namespace CNet {
    interface ICodec {
        void Encode<T>(T msg, Stream stream) where T : class;
        void Decode<T>(T msgId, byte[] data) where T : class;

        string Name();

    }

    class ProtobufCodec : ICodec {
        public void Encode<T>(T msg, Stream stream) where T : class {
            var protomsg = (IMessage)msg;
            protomsg.WriteTo(new CodedOutputStream(stream));
        }

        public void Decode<T>(T msg, byte[] data) where T : class {
            var protomsg = (IMessage)msg;
            protomsg.MergeFrom(data);
        }

        public string Name() {
            return "protobuf";
        }
    }


    class Codec {
        private static List<ICodec> codecList = new List<ICodec>();

        public static void RegCodec(ICodec codec) {
            codecList.Add(codec);
        }
        public static ICodec GetCodec(string name) {
            return codecList.Find(c => c.Name() == name);
        }
    }
}

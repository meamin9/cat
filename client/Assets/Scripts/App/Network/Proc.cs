using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Google.Protobuf;

namespace CNet
{

    // 简单起见，直接读，写死protobuf协议
    class Proc
    {
        const int maxPackageSize = 1024 * 4;
        static public async Task<bool> ReadPackage(NetworkStream stream) {
            // 2 bytes len
            var headerBuff = new byte[2];
            var idBuff = new byte[2];
            var dataBuff = new byte[maxPackageSize - 2];
            while (true) {
                // header 2
                await stream.ReadAsync(headerBuff, 0, headerBuff.Length);
                int len = Convert.ToInt16(headerBuff.ToString());
                if (len < 2 || len > maxPackageSize) {
                    return false;
                }
                // msgId 2
                await stream.ReadAsync(idBuff, 0, idBuff.Length);
                Int16 msgId = Convert.ToInt16(idBuff.ToString());
                if (msgId < 0) {
                    return false;
                }
                // data
                await stream.ReadAsync(dataBuff, 0, len-2);
                var msg = DecodeMsg(msgId, dataBuff);
            }
        }

        static private IMessage DecodeMsg(uInt16 msgId, byte[] data) {
            var typ = MsgMetaSet.MetaById(msgId);
            var msg = Activator.CreateInstance(typ);
            var protomsg = (IMessage)msg;
            protomsg.MergeFrom(data);
            return protomsg;
        }

        //public async Task<bool> WritePackage(NetworkStream stream) {

        //}
    }
}

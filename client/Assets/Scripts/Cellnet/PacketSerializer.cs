using System;
using System.IO;

namespace Cellnet
{
    class PacketSerializer
    {
        public static PacketHeader ReadHeader(PacketStream ps, UInt16 tag)
        {
            PacketHeader header;
            using (BinaryReader reader = new BinaryReader(ps.ToStream()))
            {
                header.Tag = reader.ReadUInt16();
                header.MsgID = reader.ReadUInt32();
                header.DataSize = (Int32)(reader.ReadUInt32());
            }
            return header;
        }

        public static PacketStream WriteFull(PacketHeader header, byte[] data) 
        {
            PacketStream ps = new PacketStream(header.DataSize + PacketHeader.HeaderSize, null, null);
            using (BinaryWriter writer = new BinaryWriter(ps.ToStream()))
            {
                writer.Write(header.Tag);
                writer.Write(header.MsgID);
                writer.Write(header.DataSize);
                writer.Write(data);
                writer.Flush();
            }
            return ps;
        }
    }

}
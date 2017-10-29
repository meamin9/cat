﻿
namespace Cellnet
{
    public class StringHash
    {
        static bool _init;
        readonly static uint _crcPoly = 0x04c11db7;

        static uint[] _crcTable = new uint[256];

        static void InitCRCTable()
        {
            uint c = 0;
            uint j = 0;

            for (uint i = 0; i < _crcTable.Length; i++)
            {
                c = i << 24;

                for (j = 8; j != 0; j = j - 1)
                {
                    if ((c & 0x80000000) != 0)
                    {
                        c = (c << 1) ^ _crcPoly;
                    }
                    else
                    {
                        c = c << 1;
                    }

                    _crcTable[i] = c;
                }
            }
        }

        public static uint Hash(string msgType)
        {
            if (!_init)
            {
                InitCRCTable();
                _init = true;
            }


            uint hash = 0;
            uint b = 0;

            for (int i = 0; i < msgType.Length; i++)
            {
                b = (uint)(msgType[i]);
                hash = ((hash >> 8) & 0x00FFFFFF) ^ _crcTable[(hash ^ b) & 0x000000FF];
            }

            return hash;
        }
    }

}
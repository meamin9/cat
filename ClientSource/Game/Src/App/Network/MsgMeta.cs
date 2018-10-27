﻿using System;
using System.Collections.Generic;

namespace Automata.Game {
    // 先用反射来实现，后面需要时再改成非反射
    public struct MsgMeta { // 不支持codec了，直接用protobuf
        public Type MsgType;
        public UInt16 MsgId;

        public MsgMeta(Type msgType, UInt16 id) {
            this.MsgType = msgType;
            this.MsgId = id;
        }

        public bool IsEmpty() {
            return MsgId == 0;
        }

        public static MsgMeta Empty;
    }

    public class MsgMetaSet {

        private static Dictionary<UInt16, MsgMeta> metaById = new Dictionary<UInt16, MsgMeta>();
        private static Dictionary<Type, MsgMeta> metaByType = new Dictionary<Type, MsgMeta>();

        public static void RegMsg(UInt16 msgId, Type msgType) {
            if (metaById.ContainsKey(msgId)) {
                Log.Errorf("proto metamsg resgisted repeated: {}", msgType);
                return;
            }
            var meta = new MsgMeta(msgType, msgId);
            metaById[msgId] = meta;
            metaByType[msgType] = meta;
        }

        public static MsgMeta MetaByType(Type type) {
            try {
                return metaByType[type];
            }
            catch (KeyNotFoundException) {
                return MsgMeta.Empty;
            }
        }

        public static MsgMeta MetaById(UInt16 msgId) {
            try {
                return metaById[msgId];
            } catch (KeyNotFoundException) {
                return MsgMeta.Empty;
            }
        }
    }
}

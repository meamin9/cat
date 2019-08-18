using System;
using System.Collections.Generic;
using Google.Protobuf;

namespace AM.Game
{
	public struct MsgMeta {
		public UInt16 MsgId;
		public Type MsgType;
		public Func<IMessage> Creator;

		public bool IsEmpty() {
			return MsgId == 0;
		}

		public static MsgMeta Empty;
	}

	public class MsgMetaSet {

		private static Dictionary<UInt16, MsgMeta> metaById = new Dictionary<UInt16, MsgMeta>();
		private static Dictionary<Type, MsgMeta> metaByType = new Dictionary<Type, MsgMeta>();

		public static void RegMsg(UInt16 msgId, Type msgType, Func<IMessage> creator) {
			var meta = new MsgMeta() {
				MsgId = msgId,
				MsgType = msgType,
				Creator = creator
			};
			metaById.Add(msgId, meta);
			metaByType.Add(msgType, meta);
		}

		public static MsgMeta MetaByType(Type type) {
			try {
				return metaByType[type];
			}
			catch (KeyNotFoundException) {
				return MsgMeta.Empty;
			}
		}

		public static MsgMeta MetaById(ushort msgId) {
			try {
				return metaById[msgId];
			} catch (KeyNotFoundException) {
				return MsgMeta.Empty;
			}
		}
	}
}

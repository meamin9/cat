using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json;

namespace AM.Base {
    public class SerializeBehaviour : MonoBehaviour {
            public void Serialize<T>(T data) {
#if DEBUG
                if (Application.isPlaying) {
                    throw new Exception("Cant call this in game application!Just only for Editor");
                }
#endif
                using (var mr = new MemoryStream())
                using (var bw = new BsonDataWriter(mr)) {
                    JsonSerializer.CreateDefault().Serialize(bw, data);
                    contents = mr.GetBuffer();
                }
            }
            public T Deserialize<T>() {
                using (var mr = new MemoryStream(contents))
                using (var br = new BsonDataReader(mr)) {
                    return JsonSerializer.CreateDefault().Deserialize<T>(br);
                }
            }
            public object Deserialize(Type type) {
                using (var mr = new MemoryStream(contents))
                using (var br = new BsonDataReader(mr)) {
                    return JsonSerializer.CreateDefault().Deserialize(br, type);
                }
            }
            [HideInInspector]
            [SerializeField]
            byte[] contents;
        }
}

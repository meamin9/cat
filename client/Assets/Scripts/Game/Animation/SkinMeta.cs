using Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game {
    public class SkinMeta {
        public string rootBone;
        public string[] bones;

        private static Dictionary<string, SkinMeta> _skinMetas = new Dictionary<string, SkinMeta>();

        public static SkinMeta GetMeta(string path, GameObject go) {
            if (_skinMetas.TryGetValue(path, out SkinMeta meta)) {
                return meta;
            }
            var serialize = go.GetComponent<SerializeData>();
            Log.ErrorIf(serialize == null, $"Not Found SerializeData in {go.name}");
            meta = serialize.Deserialize<SkinMeta>();
            Log.Info(JsonConvert.SerializeObject(meta.bones));
            _skinMetas[path] = meta;
            return meta;
        }

    }
}

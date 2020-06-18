#if UNITY_EDITOR
using Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Tools {
    [CreateAssetMenu(fileName = nameof(Setting), menuName = nameof(Setting), order = 1)]
    public class Setting : ScriptableObject {

        private static Setting mInstance;
        public static Setting Instance {
            get {
                if (mInstance == null) {
                    mInstance = Resources.Load<Setting>("Config/" + nameof(Setting));
                }
                return mInstance;
            }
        }

        public void OnValidate() {
            var path = Path.Combine(AssetMgr.DefaultDir, nameof(Setting) + ".json");
            //var instance = Resources.Load<Setting>("Config/" + nameof(Setting));
            var json = JsonConvert.SerializeObject(Instance.appSetting);
            File.WriteAllText(path, json);
        }

        public Base.AppSetting appSetting;

    }
}
#endif
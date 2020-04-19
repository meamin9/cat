using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using AM.Game;
using UnityEditor;
using System.IO;

namespace Tools {
    public class GenSkinMeta {

        public void Reset() {
            
        }
        public void GenerateSkinData() {
            var path = "Assets/Resources/Appearence/";
            var fbxs = Directory.GetFiles(path, "*_Preview.FBX", SearchOption.AllDirectories);
            foreach(var fbx in fbxs) {
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(fbx);
            }
            //var smr = GetComponent<SkinnedMeshRenderer>();
            var bones = smr.bones;

        }
    }
}

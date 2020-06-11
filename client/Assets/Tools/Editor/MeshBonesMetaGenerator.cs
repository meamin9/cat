using System;
using System.Collections.Generic;
using UnityEngine;
using Base;

namespace Tools {

    [RequireComponent(typeof(SerializeData))]
    public class MeshBonesMetaGenerator : MonoBehaviour {

        public void Reset() {
            GenSkinMeta();
        }

        public void GenSkinMeta() {
            var smr = GetComponent<SkinnedMeshRenderer>();
            if (smr == null) {
                Debug.LogError("Not found skinnedMeshRenderer");
                return;
            }
            var meta = new Game.SkinMeta();
            var root = transform.parent;
            meta.rootBone = GetRelativePath(smr.rootBone, root);
            var bones = new string[smr.bones.Length];
            for(var i = 0; i < bones.Length; ++i) {
                bones[i] = GetRelativePath(smr.bones[i], root);
            }
            var serialize = GetComponent<SerializeData>();
            serialize.Serialize(meta);
            enabled = false;
        }

        public string GetRelativePath(Transform node, Transform root) {
            var list = new List<string>();
            while (node != null && node != root) {
                list.Add(node.name);
                node = node.parent;
            }
            var path = string.Join("/", list);
            Debug.Log(path);
            return path;
        }
    }
}

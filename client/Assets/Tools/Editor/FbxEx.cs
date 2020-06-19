using System;
using System.Collections.Generic;
using UnityEngine;
using Base;
using UnityEditor;
using System.IO;

namespace Tools {

    public static class FbxHelper {
        [MenuItem("Assets/Create/Custom/FbxPrefabs")]
        public static void CreateFbxPrefabs() {
            var obj = UnityEditor.Selection.activeObject;
            if (obj == null) {
                Debug.LogError("选中fbx");
                return;
            }
            var path = AssetDatabase.GetAssetPath(obj);
            var ext = Path.GetExtension(path);
            if (ext.ToLower() != ".fbx") {
                Debug.LogError("选中fbx");
                return;
            }
            var name = Path.GetFileNameWithoutExtension(path).Replace("_Preview", ""); ;
            var dir = Path.GetDirectoryName(path);
            var parentDir = Path.GetDirectoryName(dir);

            var prefabDir = Path.Combine(parentDir, "Prefabs");
            if (!Directory.Exists(prefabDir)) {
                Directory.CreateDirectory(prefabDir);
            }
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var go = GameObject.Instantiate<GameObject>(asset);
            //mesh skin
            var smr = go.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null) {
                GenSkinMeta(smr.gameObject);
                var mesh = smr.sharedMesh;
                var m = GameObject.Instantiate<Mesh>(mesh);
                smr.sharedMesh = m;
                AssetDatabase.CreateAsset(m, Path.Combine(prefabDir, name + ".mesh"));
                var mat = AssetDatabase.LoadAssetAtPath<Material>(Path.Combine(parentDir, "Materials", name + ".mat"));
                if (mat != null) {
                    smr.sharedMaterial = mat;
                }
                else {
                    Debug.LogError($"cant find material {smr.gameObject.name}");
                }
                if (!PrefabUtility.SaveAsPrefabAsset(smr.gameObject, Path.Combine(prefabDir, name + "_Skin.prefab"))) {
                    Debug.LogError("Skin Prefab failed");
                }
            }
            //skeleton
            GameObject.DestroyImmediate(smr.gameObject);
            var anim = go.GetComponent<Animator>();
            if (anim == null) {
                anim = go.AddComponent<Animator>();
            }
            anim.avatar = null;
            anim.runtimeAnimatorController = null;
            anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            if (!PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(prefabDir, name + "_Skeleton.prefab"))) {
                Debug.LogError("Skeleton Prefab failed");
            }
            GameObject.DestroyImmediate(go);
            //animation
            var animDir = Path.Combine(Path.GetDirectoryName(dir), "Anims");
            if (!Directory.Exists(animDir)) {
                Directory.CreateDirectory(animDir);
            }
            var all = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (var a in all) {
                if (a.GetType() == typeof(AnimationClip) && !a.name.StartsWith("__preview__")) {
                    var clip = GameObject.Instantiate(a as AnimationClip);
                    var n = a.name.Replace(name + "_", "");
                    AssetDatabase.CreateAsset(clip, Path.Combine(animDir, n + ".anim"));
                }
            }
            AssetDatabase.Refresh();
        }

        [MenuItem("CONTEXT/SkinnedMeshRenderer/GenSkinMeta")]
        public static void CreatePrefabs() {
            GenSkinMeta(UnityEditor.Selection.activeObject as GameObject);
        }

        public static void GenSkinMeta(GameObject go) {
            var smr = go.GetComponent<SkinnedMeshRenderer>();
            if (smr == null) {
                Debug.LogError("Not found skinnedMeshRenderer");
                return;
            }
            var meta = new Game.SkinMeta();
            var root = go.transform.parent;
            meta.rootBone = GetRelativePath(smr.rootBone, root);
            var bones = new string[smr.bones.Length];
            for (var i = 0; i < bones.Length; ++i) {
                bones[i] = GetRelativePath(smr.bones[i], root);
            }
            meta.bones = bones;
            var serialize = go.GetComponent<SerializeData>() ?? go.AddComponent<SerializeData>();
            serialize.Serialize(meta);
            Debug.Log($"gen bones info finished : {bones.Length}");
        }

        public static string GetRelativePath(Transform node, Transform root) {
            var list = new List<string>();
            while (node != null && node != root) {
                list.Insert(0, node.name);
                node = node.parent;
            }
            var path = string.Join("/", list);
            Debug.Log(path);
            return path;
        }
    }
}

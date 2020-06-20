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
            var objs = UnityEditor.Selection.objects;
            foreach(var obj in objs) {
                var path = AssetDatabase.GetAssetPath(obj);
                CreateByFbx(path);
            }
            //if (obj == null) {
            //    Debug.LogError("选中fbx");
            //    return;
            //}
        }
        public static void CreateByFbx(string path) { 
            var ext = Path.GetExtension(path);
            if (ext.ToLower() != ".fbx") {
                Debug.LogWarning($"not fbx {path}");
                return;
            }
            var fbxName = Path.GetFileNameWithoutExtension(path).Replace("_Preview", "");
            var dir = Path.GetDirectoryName(path);
            var parentDir = Path.GetDirectoryName(dir);
            var dirName = Path.GetFileName(parentDir);

            //animation
            var animDir = Path.Combine(Path.GetDirectoryName(dir), "Anims");
            if (!Directory.Exists(animDir)) {
                Directory.CreateDirectory(animDir);
            }

            var matDir = Path.Combine(Path.GetDirectoryName(dir), "Materials");
            if (!Directory.Exists(matDir)) {
                Directory.CreateDirectory(matDir);
            }

            var all = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (var a in all) {
                if (a.GetType() == typeof(AnimationClip) && !a.name.StartsWith("__preview__")) {
                    var clip = GameObject.Instantiate(a as AnimationClip);
                    var n = a.name.Replace(fbxName + "_", "");
                    if (n.EndsWith(".com")) {
                        n = fbxName;
                    }
                    AssetDatabase.CreateAsset(clip, Path.Combine(animDir, n + ".anim"));
                }
                else if (a.GetType() == typeof(Material)) {
                    var mat = GameObject.Instantiate(a as Material);
                    var n = a.name.Replace(fbxName, dirName);
                    AssetDatabase.CreateAsset(mat, Path.Combine(matDir, n + ".mat"));
                }
            }
            // prefabs
            var prefabDir = Path.Combine(parentDir, "Prefabs");
            if (!Directory.Exists(prefabDir)) {
                Directory.CreateDirectory(prefabDir);
            }
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var go = GameObject.Instantiate<GameObject>(asset);
            //mesh skin
            var smrs = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var smr in smrs) {
                GenSkinMeta(smr.gameObject);
                var mesh = smr.sharedMesh;
                var m = GameObject.Instantiate<Mesh>(mesh);
                smr.sharedMesh = m;
                var skinName = smr.gameObject.name.Replace(fbxName, dirName);
                AssetDatabase.CreateAsset(m, Path.Combine(prefabDir, skinName + ".mesh"));
                var mat = AssetDatabase.LoadAssetAtPath<Material>(Path.Combine(matDir, skinName + ".mat"));
                if (mat != null) {
                    smr.sharedMaterial = mat;
                } else if (smr.sharedMaterial != null) {
                    mat = AssetDatabase.LoadAssetAtPath<Material>(Path.Combine(matDir, smr.sharedMaterial.name + ".mat"));
                    if (mat != null) {
                        smr.sharedMaterial = mat;
                    }
                    else {
                        Debug.LogWarning($"cant find material {smr.sharedMaterial.name}");
                    }
                }
                if (!PrefabUtility.SaveAsPrefabAsset(smr.gameObject, Path.Combine(prefabDir, skinName + "_Skin.prefab"))) {
                    Debug.LogError("Skin Prefab failed");
                }
                GameObject.DestroyImmediate(smr.gameObject);
            }
            //skeleton, 没有mesh就不导skeleton了
            if (smrs.Length > 0) {
                var anim = go.GetComponent<Animator>();
                if (anim == null) {
                    anim = go.AddComponent<Animator>();
                }
                anim.avatar = null;
                anim.runtimeAnimatorController = null;
                anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                if (!PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(prefabDir, dirName + "_Skeleton.prefab"))) {
                    Debug.LogError("Skeleton Prefab failed");
                }
            }
            GameObject.DestroyImmediate(go);
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

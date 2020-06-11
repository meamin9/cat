﻿using Base;
using System.Collections;
using UnityEngine;

namespace Game {
    /// <summary>
    /// 外观，包括骨骼，皮肤，挂节点
    /// </summary>
    public class Avatar {
        public GameObject gameObject;

        private Transform mSkeleton;
        private GameObject mSkin;
        private SkinnedMeshRenderer mSMR;

        /// <summary>
        /// 挂机点
        /// </summary>
        private Transform[] mAttachPositions;

        public AnimationController AnimCtrl { get; private set; }


        private string mSkeletonPath;
        private string mSkinPath;

        public Transform transform => gameObject.transform;

        public Avatar(Transform parent, string skeletonPath, string skinPath) {
            gameObject = new GameObject();
            gameObject.transform.SetParent(parent, false);
            gameObject.transform.localPosition = Vector3.zero;
            Load(skeletonPath, skinPath);
        }

        private void Load(string skeletonPath, string skinPath) {
            mSkeletonPath = skeletonPath;
            mSkinPath = skinPath;
            var waitForSkin = 2;
            AssetMgr.Instance.LoadAsync(skeletonPath, (asset) => {
                mSkeleton = GameObject.Instantiate((GameObject)asset).transform;
                mSkeleton.SetParent(gameObject.transform, false);
                mSkeleton.localPosition = Vector3.zero;
                AfterSkeletonLoaded();
                --waitForSkin;
                if (waitForSkin == 0) {
                    ResetSkinMeta();
                }
            });
            AssetMgr.Instance.LoadAsync(skinPath, (asset) => {
                mSkin = GameObject.Instantiate((GameObject)asset);
                mSkin.transform.SetParent(gameObject.transform, false);
                mSkin.transform.localPosition = Vector3.zero;
                mSMR = mSkin.GetComponentInChildren<SkinnedMeshRenderer>();
                --waitForSkin;
                if (waitForSkin == 0) {
                    ResetSkinMeta();
                }
            });
        }
        private void ResetSkinMeta() {
            var meta = SkinMeta.GetMeta(mSkinPath, mSMR.gameObject);
            mSMR.rootBone = mSkeleton.Find(meta.rootBone);
            var bones = new Transform[meta.bones.Length];
            for(var i = 0; i < bones.Length; ++i) {
                bones[i] = mSkeleton.Find(meta.bones[i]);
            }
            mSMR.bones = bones;
        }

        private void AfterSkeletonLoaded() {
            if (mAttachPositions != null) {
                InitAttachPositions();
            }
            AnimCtrl = AnimationController.Create(mSkeleton.gameObject);
        }

        public void Destory() {
            AnimCtrl?.Destory();
            GameObject.Destroy(gameObject);
        }

        #region 挂机点
        /// <summary>
        /// 需要时再初始化
        /// </summary>
        private void InitAttachPositions() {
            var attachNames = new string[] {
                "LeftHand",
                "RigthHand",
            };
            mAttachPositions = new Transform[attachNames.Length + 1];
            mAttachPositions[0] = gameObject.transform;
            for (var i = 0; i < attachNames.Length; ++i) {
                mAttachPositions[i+1] = mSkeleton.Find(attachNames[i]);
            }
        }

        public void Attach(int attachIndex, Transform obj) {
            if (mAttachPositions != null) {
                InitAttachPositions();
            }
            obj.SetParent(mAttachPositions[attachIndex], false);
        }

        public void Detach(int attchIndex) {

        }
        #endregion

    }
}

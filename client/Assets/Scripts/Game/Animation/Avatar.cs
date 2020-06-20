using Base;
using System;
using System.Collections;
using UnityEngine;

namespace Game {
    /// <summary>
    /// 骨骼，皮肤，挂节点, 动画
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
        public AnimationEventAdapter AnimEventAdapter { get; private set; }


        private string mSkeletonPath;
        private string mSkinPath;

        public bool isLoaded;

        public Transform transform => gameObject.transform;

        public Avatar(Transform parent, string skeletonPath, string skinPath, System.Action finishCb) {
            gameObject = new GameObject();
            gameObject.transform.SetParent(parent, false);
            gameObject.transform.localPosition = Vector3.zero;

            

            Load(skeletonPath, skinPath, finishCb);
        }

        private void Load(string skeletonPath, string skinPath, Action finishCb) {
            mSkeletonPath = skeletonPath;
            mSkinPath = skinPath;
            var waitForSkin = skinPath == null ? 1 : 2;
            AssetMgr.Instance.LoadAsync(skeletonPath, (asset) => {
                mSkeleton = GameObject.Instantiate((GameObject)asset).transform;
                mSkeleton.SetParent(gameObject.transform, false);
                mSkeleton.localPosition = Vector3.zero;
                AfterSkeletonLoaded();
                --waitForSkin;
                if (waitForSkin == 0) {
                    ResetSkinMeta();
                    isLoaded = true;
                    finishCb?.Invoke();
                }
            });
            if (skinPath != null) {
                AssetMgr.Instance.LoadAsync(skinPath, (asset) => {
                    mSkin = GameObject.Instantiate((GameObject)asset);
                    mSkin.transform.SetParent(gameObject.transform, false);
                    mSkin.transform.localPosition = Vector3.zero;
                    mSMR = mSkin.GetComponentInChildren<SkinnedMeshRenderer>();
                    --waitForSkin;
                    if (waitForSkin == 0) {
                        ResetSkinMeta();
                        isLoaded = true;
                        finishCb?.Invoke();
                    }
                });
            }
        }
        private void ResetSkinMeta() {
            if (mSkinPath == null) {
                return;
            }
            var meta = SkinMeta.GetMeta(mSkinPath, mSMR.gameObject);
            mSMR.rootBone = mSkeleton.Find(meta.rootBone);
            var bones = new Transform[meta.bones.Length];
            for(var i = 0; i < bones.Length; ++i) {
                bones[i] = mSkeleton.Find(meta.bones[i]);
            }
            mSMR.bones = bones;
        }

        private void AfterSkeletonLoaded() {
            AnimCtrl = AnimationController.Create(mSkeleton.gameObject);
            AnimEventAdapter = gameObject.AddComponent<AnimationEventAdapter>();
            if (mAttachPositions != null) {
                InitAttachPositions();
            }
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

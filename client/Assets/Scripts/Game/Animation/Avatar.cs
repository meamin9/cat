using Base;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.Animation {
    /// <summary>
    /// 外观，包括骨骼，皮肤，挂节点
    /// </summary>
    public class Avatar {
        //private
        private Transform mSkeleton;
        private GameObject mSkin;

        private GameObject gameObject;


        public void LoadSkeleton(string skeletonPath) {
            AssetMgr.Instance.LoadAsync(skeletonPath, (asset) => {
                mSkeleton = GameObject.Instantiate((GameObject)asset).transform;
                mSkeleton.SetParent(gameObject.transform);
                mSkeleton.localPosition = Vector3.zero;
            });
        }

        public void LoadSkin(string skinPath) {
            AssetMgr.Instance.LoadAsync(skinPath, (asset) => {
                mSkin = GameObject.Instantiate((GameObject)asset);
                mSkin.transform.SetParent(gameObject.transform);
                mSkin.transform.position = Vector3.zero;
                var mesh = mSkin.GetComponent<SkinnedMeshRenderer>();
            });

        }
    }
}

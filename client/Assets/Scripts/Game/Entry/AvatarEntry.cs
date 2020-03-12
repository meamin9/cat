using AM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AM.Game
{
    /// <summary>
    /// 实体配置
    /// </summary>
    public class AvatarEntry
    {
        #region config data
        public int Id;
        public string SkeletonPath;
        public string SkinPath;
        public string ControllerPath;

        public override int GetHashCode() {
            return Id;
        }
        #endregion

        public void LoadAvatar() {
            var avatar = new GameObject();
            AssetMgr.Instance.LoadAsync(SkeletonPath, (asset) => {
                //entity.gameObject = GameObject.Instantiate<GameObject>(asset as GameObject);
            });

        }

        private IEnumerator DoLoadAvatar() {
            yield return AssetMgr.Instance.LoadAsync(SkeletonPath, (asset) => {
                //entity.gameObject = GameObject.Instantiate<GameObject>(asset as GameObject);
            });
            yield return AssetMgr.Instance.LoadAsync(SkinPath, (asset) => {
                //entity.gameObject = GameObject.Instantiate<GameObject>(asset as GameObject);
            });
        }
    }
}

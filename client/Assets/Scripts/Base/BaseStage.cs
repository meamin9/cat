using System.Collections;
using Automata.Adapter;
using UnityEngine;

namespace Automata.Base
{
    public class BaseStage
    {
        public static Coroutine Initialize()
        {
            AssetMgr.Instance.LoadAssetsTable();
            return MonoProxy.Instance.StartCoroutine(LoadBaseAssets());
        }

        private static IEnumerator LoadBaseAssets()
        {
            yield return AssetMgr.Instance.LoadAssetManifest();
#if UNITY_EDITOR
            var asset = Resources.Load("Config/" + nameof(BaseConfig));
            BaseConfig.Load(asset);
            if (!BaseConfig.Instance.LoadInResource)
            {
                yield return AssetMgr.Instance.LoadAssetAsync(nameof(BaseConfig) + ".asset", (req) => {
                    BaseConfig.Load(req.Asset);
                });
            }
#else
            yield return AssetMgr.Instance.LoadAssetAsync("AppConfig.asset", (req) => {
                BaseConfig.Load(req.Asset);
            });
#endif
        }

    }
}

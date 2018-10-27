using Automata.Base;
using System.Collections;
using Automata.Adapter;

namespace Automata.Game
{
    public class GameStage
    {
        public void Enter()
        {
            Log.Infof("GameStart");
            RegistAllUI();
            UIMgr.Instance.Show(UIID.Loading);
            MonoProxy.Instance.StartCoroutine(LoadPrimaryEntry());
        }

        public void RegistAllUI()
        {
            UIMgr.Instance.RegistUICreator(UIID.Loading, UIMgr.GenUICreator<LoadingUI>());
        }

        public IEnumerator LoadPrimaryEntry()
        {
            yield return AssetMgr.Instance.LoadAsync(SceneEntry.JsonPath, SceneEntry.Load);
        }
    }
}


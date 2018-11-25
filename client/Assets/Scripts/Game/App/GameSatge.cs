using Automata.Base;
using System.Collections;
using Automata.Adapter;

namespace Automata.Game
{
    public class GameStage
    {
        public static void Enter()
        {
            Log.Infof("GameStart");
            MonoProxy.Instance.StartCoroutine(Start());
        }

        private static IEnumerator Start()
        {
            yield return BaseStage.Initialize();
            yield return AssetMgr.Instance.LoadAsync(AppSetting.AssetPath, AppSetting.Load);

            InitAllMgr();
            RegistAllUI();
            //UIMgr.Instance.Show(UIID.Loading);
            UIMgr.Instance.Show(UIID.Joystick);
            MonoProxy.Instance.StartCoroutine(LoadPrimaryEntry());
        }
        public static void InitAllMgr()
        {
            //InputMgr.Instance
            CameraMgr.Instance.Initialize();

        }

        public static void RegistAllUI()
        {
            UIMgr.Instance.RegistUICreator(UIID.Loading, UIMgr.GenUICreator<LoadingUI>());
            UIMgr.Instance.RegistUICreator(UIID.Joystick, UIMgr.GenUICreator<JoystickUI>());
        }

        public static IEnumerator LoadPrimaryEntry()
        {
            
            yield return AssetMgr.Instance.LoadAsync(SceneEntry.AssetPath, Table<SceneEntry>.LoadDict);
            yield return SceneMgr.Instance.SwitchScene(100);
            EntityMgr.Instance.CreatePlayer("");
        }
    }
}


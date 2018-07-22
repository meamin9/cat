using UnityEngine;

class Launcher : MonoBehaviour
{
    public void Start() {
        AppStageMgr.Instance.EnterStage(new InitStage());
    }
}

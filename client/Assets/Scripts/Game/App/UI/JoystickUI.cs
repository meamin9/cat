using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Automata.Base;

namespace Automata.Game
{
    public class JoystickUI : IBaseUI
    {
        public int Id { get { return UIID.Joystick; } }
        public string PrefabPath { get { return "JoystickUI.prefab"; } }
        public GameObject gameObject { get; set; }
        public void OnAttach()
        {
            _joystick = gameObject.GetComponent<ETCJoystick>();
            _joystick.onMove.AddListener(OnMove);

        }
        public void OnDetach() { }
        public void OnShow() { }
        public void OnHide() { }

        public ETCJoystick _joystick;

        private void OnMove(Vector2 offset)
        {
            EntityMgr.Player.Move(new Vector3(offset.x, 0, offset.y)*0.05f);
        }

    }

}

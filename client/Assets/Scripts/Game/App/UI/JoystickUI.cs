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

        }
        public void OnDetach() { }
        public void OnShow() { }
        public void OnHide() { }

        public ETCJoystick _joystick;

    }

}

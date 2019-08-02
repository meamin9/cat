using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AM.Base;

namespace AM.Game
{
    public class JoystickUI : BaseUI
    {
        public override void OnAttach()
        {
            _joystick = gameObject.GetComponent<ETCJoystick>();
            _joystick.onMove.AddListener(OnMove);

        }
        public override void OnDetach() { }
        public override void OnShow() { }
        public override void OnHide() { }

        public ETCJoystick _joystick;

        private void OnMove(Vector2 offset)
        {
            //EntityManager.Player.MoveCtrl.Move(new Vector3(offset.x, 0, offset.y)*0.05f);
        }

    }

}
